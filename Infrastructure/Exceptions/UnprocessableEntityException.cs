public class UnprocessableEntityException : Exception
{
    public int StatusCode { get; }

    public UnprocessableEntityException() : base() 
    {
        StatusCode = 422;
    }

    public UnprocessableEntityException(string message = "Unprocessable Entity") : base(message) 
    {
        StatusCode = 422;
    }
}