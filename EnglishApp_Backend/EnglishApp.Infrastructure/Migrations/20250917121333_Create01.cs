using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "verificationCodes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "verificationCodes");
        }
    }
}
