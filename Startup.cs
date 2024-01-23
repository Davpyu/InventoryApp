
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using FluentValidation.AspNetCore;
using StackExchange.Redis;
using DotNetService.Models;
using Microsoft.EntityFrameworkCore;
using DotNetService.Infrastructure.Integrations.Http;
using DotNetService.Constants.Logger;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Middlewares;
using DotNetService.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using NATS.Client.Hosting;
using NATS.Client.Core;
using System.Security.Policy;
using DotNetService.Infrastructure.Events;
using DotNetService.Infrastructure;
using DotNetService.Infrastructure.Queues;
using DotNetService.Infrastructure.BackgroundHosted;

namespace DotNetService
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {   
            var cooldownBreak = int.Parse(Configuration["CircuitBreaker:External:Cooldown"] ?? "5");
            var AllowedBroken = int.Parse(Configuration["CircuitBreaker:External:AllowedBroken"] ?? "5");

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(AllowedBroken, TimeSpan.FromMinutes(cooldownBreak));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile("Log/" + DateTime.Now.ToString("yyyy-MM-dd") + "-default.log", 
                    fileLoggerOpts => {
                        fileLoggerOpts.Append = true;
                        fileLoggerOpts.FilterLogEntry = (msg) => {
                            return msg.LogLevel == LogLevel.Information && !(msg.LogName == LoggerConstant.INTEGRATION || msg.LogName == LoggerConstant.INTEGRATION);
                        };   
                    }
                );
                loggingBuilder.AddFile("Log/" + DateTime.Now.ToString("yyyy-MM-dd") + "-integration.log", 
                    fileLoggerOpts => {
                        fileLoggerOpts.Append = true;
                        fileLoggerOpts.FilterLogEntry = (msg) => {
                            return msg.LogLevel == LogLevel.Information && msg.LogName == LoggerConstant.INTEGRATION;
                        };   
                    }
                );
                loggingBuilder.AddFile("Log/" + DateTime.Now.ToString("yyyy-MM-dd") + "-error.log", 
                    fileLoggerOpts => {
                        fileLoggerOpts.Append = true;
                        fileLoggerOpts.FilterLogEntry = (msg) => {
                            return msg.LogLevel == LogLevel.Error;
                        };   
                    }
                );
                loggingBuilder.AddFile("Log/" + DateTime.Now.ToString("yyyy-MM-dd") + "-activity.log", 
                    fileLoggerOpts => {
                        fileLoggerOpts.Append = true;
                        fileLoggerOpts.FilterLogEntry = (msg) => {
                            return msg.LogLevel == LogLevel.Information && msg.LogName == LoggerConstant.ACTIVITY;
                        };   
                    }
                );
            });

            // TODO: Default Configuration AWS S3
            // services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            // services.AddAWSService<IAmazonS3>();

            // TODO: Install Minio for using this line of code 
            // if (bool.Parse(Configuration["Minio:IsEnable"] ?? "false")) {
            //     services.AddMinio(configureClient => configureClient
            //         .WithEndpoint(Configuration["Minio:Endpoint"])
            //         .WithSSL(bool.Parse(Configuration["Minio:IsUseSSL"] ?? "false"))
            //         .WithCredentials(Configuration["Minio:ClientId"], Configuration["Minio:ClientSecret"]));
            // }

            Services(services);
            
            Repositories(services);

            Integrations(services);

            Listeners(services);

            // Queue Servicee
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton(ctx =>
            {
                if (!int.TryParse(Configuration["Queue:Capacity"], out var queueCapacity))
                    queueCapacity = 100;
                    
                return new BackgroundTaskQueue(queueCapacity);
            });

            // Hosted Background Service
            services.AddHostedService<NATsListener>();

            services.AddHealthChecks();

            services.AddNats(1000, options => {

                    var opts = new NatsOpts {
                        Url = Configuration["Nats:Url"],
                        AuthOpts = new NatsAuthOpts{
                            Username = Configuration["Nats:Username"],
                            Password = Configuration["Nats:Password"],
                        },
                        Name = Configuration["Nats:Server"]
                    };

                    return opts;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "AllowOrigin",
                    builder => {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    });
            });
            
            services.AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
            
            services.AddDbContext<DBContext1>(options => options
                .UseSqlServer(Configuration["ConnectionString:DefaultConnection1"] ?? ""));

            services.AddHttpContextAccessor();

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });
            
            services.AddControllers(
                options => {
                    options.Filters.Add<ValidatorAttribute>();
                    options.Filters.Add<PublishNATsEndpointCallEvent>();
                }
            ).AddNewtonsoftJson(
                options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                }
            );

            var circuitBreakerPolicy = GetCircuitBreakerPolicy();

            services.AddHttpClient<HttpIntegration>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(2)) // Circuit Breaker Cooldown time
                .AddPolicyHandler(circuitBreakerPolicy);

            var IsRedisEnable = bool.Parse(Configuration["Redis:IsEnable"]);
            var redisConnectionString = Configuration["Redis:Host"] + ':' + Configuration["Redis:Port"] + ",password=" + Configuration["Redis:Password"];   
            if (IsRedisEnable) {
                services.AddDataProtection()
                    .SetApplicationName(Configuration["App:Name"] ?? "DotNetService")
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(60))
                    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnectionString), Configuration["App:DataProtectionKey"]);
                
                services.AddStackExchangeRedisCache(options => options.ConfigurationOptions = new ConfigurationOptions
                {
                    AllowAdmin = true,
                    Password = Configuration["Redis:Password"],
                    EndPoints = { Configuration["Redis:Host"] + ':' + Configuration["Redis:Port"] },
                    Ssl = false
                });
            } else {
                services.AddDistributedMemoryCache();
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // DONT CHANGE THIS ORDERED MIDDLEWARE
            app.UseMiddleware<HandlerException>();
            app.UseMiddleware<CircuitBreakerMiddleware>();
            // ------
            
            app.UseMiddleware<HandlerException>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseResponseCaching();

            app.UseEndpoints(x =>
            {
                x.MapHealthChecks("/health").AllowAnonymous();
                x.MapControllers();
            });
        }
    }
}
