namespace DotNetService.Constants.Event
{
    public enum NATsEventCommon
    {
        ALL,
    }

    public enum NATsEventStatusEnum
    {
        ALL = NATsEventCommon.ALL,
        INFO,
        SUCCESS,
        FAILED
    }

    public enum NATsEventActionEnum
    {
        // Common action
        ALL = NATsEventCommon.ALL,
        DEBUG,
        CREATE,
        UPDATE,
        DELETE,
        UPDATE_STATUS,

        // Specific action
        LOGIN
    }

    public enum NATsEventModuleEnum
    {
        ALL = NATsEventCommon.ALL,
        LOGGER,
        AUTH,
        USER

        // Add more module here
    }
}