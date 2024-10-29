namespace DotNetService.Infrastructure.Exceptions
{
    public class UnauthenticatedException : Exception
    {

        public UnauthenticatedException(string message = "Unauthenticated") : base(message)
        {
            // message can use in here
        }

    }
}