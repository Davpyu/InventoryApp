
using DotNetService.Constants.Event;
using DotNetService.Domain.Logging.Listeners;
using DotNetService.Infrastructure.Integrations.NATs;

namespace DotNetService.Infrastructure.BackgroundHosted
{
    public class NATsTask(
        IServiceScopeFactory serviceScopeFactory,
        NATsIntegration natsIntegration
        )
    {
        public void Listen()
        {
            /** Init all task listeners here */

            /*==================== Logging ====================*/
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_POST_SUBJECT
            );
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_GET_SUBJECT
            );
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_PUT_SUBJECT
            );
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_PATCH_SUBJECT
            );
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_DELETE_SUBJECT
            );
            natsIntegration.InitListenTask<LoggingNATsListen>(serviceScopeFactory,
                EndpointCallEventConstant.SUBS_OPTIONS_SUBJECT
            );

            /*==================== Other Module ====================*/

        }

        public void ListenAndReply()
        {
            /** Init all task listeners here */

            /*==================== Logging ====================*/
            natsIntegration.InitListenAndReplyTask<LoggingNATsListenAndReply>(serviceScopeFactory,
                natsIntegration.Subject(
                    NATsEventModuleEnum.LOGGER,
                    NATsEventActionEnum.DEBUG,
                    NATsEventStatusEnum.INFO
                )
            );

            /*==================== Other Module ====================*/
        }
    }
}
