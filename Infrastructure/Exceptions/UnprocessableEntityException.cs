namespace DotNetService.Infrastructure.Exceptions;

public class UnprocessableEntityException : Exception
{
    public int StatusCode { get; }

    public UnprocessableEntityException(string message = "Unprocessable Entity") : base(message)
    {
        StatusCode = 422;
    }
}