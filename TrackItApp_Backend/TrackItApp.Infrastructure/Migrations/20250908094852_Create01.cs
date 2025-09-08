using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackItApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_verificationCodes_UserID",
                table: "verificationCodes");

            migrationBuilder.AddColumn<bool>(
                name: "IsTwoFactorEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_verificationCodes_UserID_DeviceID",
                table: "verificationCodes",
                columns: new[] { "UserID", "DeviceID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_verificationCodes_UserID_DeviceID",
                table: "verificationCodes");

            migrationBuilder.DropColumn(
                name: "IsTwoFactorEnabled",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_verificationCodes_UserID",
                table: "verificationCodes",
                column: "UserID");
        }
    }
}
