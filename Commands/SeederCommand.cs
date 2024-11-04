
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Infrastructure.Seeders;
using DotNetService.Models;
using RuangDeveloper.AspNetCore.Command;

namespace DotNetService.Commands
{
  public class SeederCommand(
      IServiceProvider serviceProvider
  ) : ICommand
  {
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public string Name => "seed";

    public string Description => "Seed the database with initial data";

    public void Execute(string[] args)
    {
    }

    public async Task ExecuteAsync(string[] args)
    {

      var fileNames = args.ToList();

      using (var scope = _serviceProvider.CreateScope())
      {
        var inventDBContext = scope.ServiceProvider.GetRequiredService<IamDBContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeederCommand>>();

        Console.WriteLine("-------------------------- Seed Started -------------------------");

        if (fileNames.Count > 0)
        {
          for (var i = 0; i < fileNames.Count; i++)
          {
            /* -------------------------- Insert seed data here ------------------------- */
            var type = Type.GetType("DotNetService.Infrastructure.Seeders." + fileNames[i]);
            if (type != null)
            {
              var seederType = Activator.CreateInstance(type) as ISeeder;
              if (seederType != null)
              {
                await seederType.Seed(inventDBContext, logger);
              }
            }
            else
            {
              throw new DataNotFoundException($"seeder of {fileNames[i]} not found");
            }
          }
        }
        // When it doesnt have any args, seed all data accordingly
        else
        {
          /* -------------------------- Insert seed data here ------------------------- */
          await new UserSeeder().Seed(inventDBContext, logger);
          await new RoleSeeder().Seed(inventDBContext, logger);
          await new PermissionSeeder().Seed(inventDBContext, logger);
        }

        Console.WriteLine("-------------------------- Seed Finish --------------------------");
      }
    }
  }
}
