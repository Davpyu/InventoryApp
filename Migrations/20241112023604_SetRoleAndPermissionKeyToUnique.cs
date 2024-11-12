using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetService.Migrations
{
    public partial class SetRoleAndPermissionKeyToUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_roles_key",
                table: "roles",
                column: "key",
                unique: true,
                filter: "[key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_key",
                table: "permissions",
                column: "key",
                unique: true,
                filter: "[key] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_key",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_permissions_key",
                table: "permissions");
        }
    }
}
