using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackItApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSessions_UserID",
                table: "UserSessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "UserSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserID_DeviceID",
                table: "UserSessions",
                columns: new[] { "UserID", "DeviceID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSessions_UserID_DeviceID",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "UserSessions");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserID",
                table: "UserSessions",
                column: "UserID");
        }
    }
}
