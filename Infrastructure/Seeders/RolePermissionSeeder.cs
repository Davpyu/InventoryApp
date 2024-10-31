using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetService.Infrastructure.Helpers;
using DotNetService.Models;
using BC = BCrypt.Net.BCrypt;

namespace DotNetService.Infrastructure.Seeders
{

  class RolePermissionJson {


    [JsonPropertyName("permissionKey")]
    public string PermissionKey { get; set; }

    [JsonPropertyName("roleKey")]
    public string RoleKey { get; set; }
    
  }

  public class RolePermissionSeeder : ISeeder
  {
    public async Task Seed(IamDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding User Role Permissions...");
      var jsonPath = "Seeders/RolePermission.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);

      var datas = JsonSerializer.Deserialize<List<RolePermissionJson>>(jsonString, JsonSerializeSeeder.options);
      var formatedData = new List<RolePermission>();

      foreach (var data in datas)
      {
        var permission = dbContext.Permissions.Where(x => x.Key == data.PermissionKey).FirstOrDefault();
        var role = dbContext.Roles.Where(x => x.Key == data.RoleKey).FirstOrDefault();

        var newRolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            PermissionId = permission.Id,
            RoleId = role.Id,
            CreatedAt = DateTime.Now
        };
        formatedData.Add(newRolePermission);
      }

      try
      {
        await dbContext.Database.BeginTransactionAsync();
        await dbContext.RolePermissions.AddRangeAsync(formatedData);
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