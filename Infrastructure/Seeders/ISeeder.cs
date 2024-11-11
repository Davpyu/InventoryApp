using DotNetService.Infrastructure.Databases;

namespace DotNetService.Infrastructure.Seeders
{
  public interface ISeeder
  {
    Task Seed(IamDBContext dbContext, ILogger logger);
  }
}
