using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnglishApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_userTypes_UserTypeID",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_verificationCodes_Users_UserID",
                table: "verificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_verificationCodes",
                table: "verificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userTypes",
                table: "userTypes");

            migrationBuilder.RenameTable(
                name: "verificationCodes",
                newName: "VerificationCodes");

            migrationBuilder.RenameTable(
                name: "userTypes",
                newName: "UserTypes");

            migrationBuilder.RenameIndex(
                name: "IX_verificationCodes_UserID_DeviceID",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_UserID_DeviceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCodes",
                table: "VerificationCodes",
                column: "VerificationCodeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTypes",
                table: "UserTypes",
                column: "UserTypeID");

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    DictionaryID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DictionaryName = table.Column<string>(type: "text", nullable: false),
                    DictionaryDescription = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryID);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWConfidences",
                columns: table => new
                {
                    ConfidenceID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfidenceNumber = table.Column<int>(type: "integer", nullable: false),
                    ConfidencePeriod = table.Column<int>(type: "integer", nullable: false),
                    DictionaryID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWConfidences", x => x.ConfidenceID);
                    table.ForeignKey(
                        name: "FK_DWConfidences_Dictionaries_DictionaryID",
                        column: x => x.DictionaryID,
                        principalTable: "Dictionaries",
                        principalColumn: "DictionaryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryWords",
                columns: table => new
                {
                    WordID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WordText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextReview = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pronunciation = table.Column<string>(type: "text", nullable: true),
                    Sources = table.Column<string>(type: "text", nullable: true),
                    DictionaryID = table.Column<int>(type: "integer", nullable: false),
                    ConfidenceID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryWords", x => x.WordID);
                    table.ForeignKey(
                        name: "FK_DictionaryWords_DWConfidences_ConfidenceID",
                        column: x => x.ConfidenceID,
                        principalTable: "DWConfidences",
                        principalColumn: "ConfidenceID");
                    table.ForeignKey(
                        name: "FK_DictionaryWords_Dictionaries_DictionaryID",
                        column: x => x.DictionaryID,
                        principalTable: "Dictionaries",
                        principalColumn: "DictionaryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWDetails",
                columns: table => new
                {
                    WordDetailID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    WordImage = table.Column<string>(type: "text", nullable: true),
                    Example = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Arabic = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    WordID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWDetails", x => x.WordDetailID);
                    table.ForeignKey(
                        name: "FK_DWDetails_DictionaryWords_WordID",
                        column: x => x.WordID,
                        principalTable: "DictionaryWords",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DWReviewHistories",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OldConfidenceNumber = table.Column<int>(type: "integer", nullable: false),
                    NewConfidenceNumber = table.Column<int>(type: "integer", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WordID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DWReviewHistories", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_DWReviewHistories_DictionaryWords_WordID",
                        column: x => x.WordID,
                        principalTable: "DictionaryWords",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedByUserID",
                table: "Dictionaries",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryWords_ConfidenceID",
                table: "DictionaryWords",
                column: "ConfidenceID");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryWords_DictionaryID",
                table: "DictionaryWords",
                column: "DictionaryID");

            migrationBuilder.CreateIndex(
                name: "IX_DWConfidences_DictionaryID",
                table: "DWConfidences",
                column: "DictionaryID");

            migrationBuilder.CreateIndex(
                name: "IX_DWDetails_WordID",
                table: "DWDetails",
                column: "WordID");

            migrationBuilder.CreateIndex(
                name: "IX_DWReviewHistories_WordID",
                table: "DWReviewHistories",
                column: "WordID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_UserTypeID",
                table: "Users",
                column: "UserTypeID",
                principalTable: "UserTypes",
                principalColumn: "UserTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCodes_Users_UserID",
                table: "VerificationCodes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_UserTypeID",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCodes_Users_UserID",
                table: "VerificationCodes");

            migrationBuilder.DropTable(
                name: "DWDetails");

            migrationBuilder.DropTable(
                name: "DWReviewHistories");

            migrationBuilder.DropTable(
                name: "DictionaryWords");

            migrationBuilder.DropTable(
                name: "DWConfidences");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCodes",
                table: "VerificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTypes",
                table: "UserTypes");

            migrationBuilder.RenameTable(
                name: "VerificationCodes",
                newName: "verificationCodes");

            migrationBuilder.RenameTable(
                name: "UserTypes",
                newName: "userTypes");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_UserID_DeviceID",
                table: "verificationCodes",
                newName: "IX_verificationCodes_UserID_DeviceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_verificationCodes",
                table: "verificationCodes",
                column: "VerificationCodeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userTypes",
                table: "userTypes",
                column: "UserTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_userTypes_UserTypeID",
                table: "Users",
                column: "UserTypeID",
                principalTable: "userTypes",
                principalColumn: "UserTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_verificationCodes_Users_UserID",
                table: "verificationCodes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
