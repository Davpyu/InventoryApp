using DotNetService.Infrastructure.Databases;
using DotNetService.Models;

namespace DotNetService.Infrastructure.Seeders
{
  public interface ISeeder
  {
    Task Seed(IamDBContext dbContext, ILogger logger);
  }
}
