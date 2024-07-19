
using DotNetService.Constants.Logger;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Shareds;
using NATS.Client.Core;
using NATS.Client;
using DotNetService.Constants.Event;

namespace DotNetService.Infrastructure.Integrations.NATs
{
    public class NATsIntegration(
        ILoggerFactory loggerFactory,
        NatsConnection natsConnection
        )
    {
        private readonly NatsConnection _natsConnection = natsConnection;
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);

        public string Subject(
            NATsEventModuleEnum modul,
            NATsEventActionEnum action,
            NATsEventStatusEnum status
        )
        {
            string subject = $"{modul}.{action}.{status}";

            subject = subject.Replace(NATsEventCommon.ALL.ToString(), ">");

            return subject.ToLower();
        }

        public async Task UnSub<T>(INatsSub<T> sub)
        {
            _logger.LogInformation("Stop Subscription Of Subject : {Subject}", sub.Subject);
            await sub.UnsubscribeAsync();
        }

        public async Task Publish<T>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : {Subject} | Data : {Data}", subject, data);
            await _natsConnection.PublishAsync(subject, data);
        }

        public async Task<R> PublishAndGetReply<T, R>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : {Subject} | Data : {Data}", subject, data);
            try
            {
                var msg = await _natsConnection.RequestAsync<T, string>(subject, data);
                var repliedData = msg.Data;

                _logger.LogInformation("Get Reply With Subject : {Subject} | Reply : {Reply}", subject, repliedData);
                return Utils.JsonDeserialize<R>(repliedData);
            }
            catch (NatsException e)
            {
                _logger.LogError("Error StackTrace: {StackTrace}", e.StackTrace);
                throw new ServiceUnavailableException();
            }
        }
    }
}
