using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackItApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "verificationCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "verificationCodes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
