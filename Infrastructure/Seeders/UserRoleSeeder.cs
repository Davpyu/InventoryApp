using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Models;
using BC = BCrypt.Net.BCrypt;

namespace DotNetService.Infrastructure.Seeders
{

  class UserRoleJson {

    public string Email { get; set; }

    [JsonPropertyName("roleKey")]
    public string RoleKey { get; set; }
    
  }

  public class UserRoleSeeder : ISeeder
  {
    public async Task Seed(IamDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding User Roles...");
      var jsonPath = "Seeders/UserRole.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);

      var datas = JsonSerializer.Deserialize<List<UserRoleJson>>(jsonString, JsonSerializeSeeder.options);
      var formatedData = new List<UserRole>();

      foreach (var data in datas)
      {
        var user = dbContext.Users.Where(x => x.Email == data.Email).FirstOrDefault();
        var role = dbContext.Roles.Where(x => x.Key == data.RoleKey).FirstOrDefault();

        var newUserRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            CreatedAt = DateTime.Now
        };
        formatedData.Add(newUserRole);
      }

      try
      {
        await dbContext.Database.BeginTransactionAsync();
        await dbContext.UserRoles.AddRangeAsync(formatedData);
        await dbContext.SaveChangesAsync();
        await dbContext.Database.CommitTransactionAsync();
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error while seeding Users");
        await dbContext.Database.RollbackTransactionAsync();
      }

      logger.LogInformation("Seeding Users complete");
    }
  }
}