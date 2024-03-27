namespace DotNetService.Constants.Event
{
    public enum NATsEventStatusEnum
    {
        INFO,
        SUCCESS,
        FAILED
    }

    public enum NATsEventActionEnum
    {
        // Common action
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
        LOGGER,
        AUTH,
        USER

        // Add more module here
    }
}