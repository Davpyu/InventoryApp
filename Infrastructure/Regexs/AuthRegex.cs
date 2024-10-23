
namespace DotNetService.Infrastructure.Regexs
{
    public class AuthRegex
    {
        public const string PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
    }
}