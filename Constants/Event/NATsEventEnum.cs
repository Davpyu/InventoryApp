namespace DotNetService.Constants.Event
{
    public enum NATsEventCommonEnum
    {
        ALL,
    }

    public enum NATsEventStatusEnum
    {
        ALL = NATsEventCommonEnum.ALL,
        INFO,
        SUCCESS,
        FAILED
    }

    public enum NATsEventActionEnum
    {
        // Common action
        ALL = NATsEventCommonEnum.ALL,
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
        ALL = NATsEventCommonEnum.ALL,
        LOGGER,
        AUTH,
        USER

        // Add more module here
    }
}