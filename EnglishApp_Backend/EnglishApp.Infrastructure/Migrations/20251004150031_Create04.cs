using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionaries_Users_CreatedByUserID",
                table: "Dictionaries");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWords_DWConfidences_ConfidenceID",
                table: "DictionaryWords");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWords_Dictionaries_DictionaryID",
                table: "DictionaryWords");

            migrationBuilder.DropForeignKey(
                name: "FK_DWConfidences_Dictionaries_DictionaryID",
                table: "DWConfidences");

            migrationBuilder.DropForeignKey(
                name: "FK_DWDetails_DictionaryWords_WordID",
                table: "DWDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DWReviewHistories_DictionaryWords_WordID",
                table: "DWReviewHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_UserTypeID",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserID",
                table: "UserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCodes_Users_UserID",
                table: "VerificationCodes");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "VerificationCodes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "DeviceID",
                table: "VerificationCodes",
                newName: "DeviceId");

            migrationBuilder.RenameColumn(
                name: "VerificationCodeID",
                table: "VerificationCodes",
                newName: "VerificationCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_UserID_DeviceID",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_UserId_DeviceId");

            migrationBuilder.RenameColumn(
                name: "UserTypeID",
                table: "UserTypes",
                newName: "UserTypeId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserSessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "DeviceID",
                table: "UserSessions",
                newName: "DeviceId");

            migrationBuilder.RenameColumn(
                name: "UserSessionID",
                table: "UserSessions",
                newName: "UserSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserID_DeviceID",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId_DeviceId");

            migrationBuilder.RenameColumn(
                name: "UserTypeID",
                table: "Users",
                newName: "UserTypeId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                newName: "IX_Users_UserTypeId");

            migrationBuilder.RenameColumn(
                name: "WordID",
                table: "DWReviewHistories",
                newName: "WordId");

            migrationBuilder.RenameColumn(
                name: "ReviewID",
                table: "DWReviewHistories",
                newName: "ReviewId");

            migrationBuilder.RenameIndex(
                name: "IX_DWReviewHistories_WordID",
                table: "DWReviewHistories",
                newName: "IX_DWReviewHistories_WordId");

            migrationBuilder.RenameColumn(
                name: "WordID",
                table: "DWDetails",
                newName: "WordId");

            migrationBuilder.RenameColumn(
                name: "WordDetailID",
                table: "DWDetails",
                newName: "WordDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_DWDetails_WordID",
                table: "DWDetails",
                newName: "IX_DWDetails_WordId");

            migrationBuilder.RenameColumn(
                name: "DictionaryID",
                table: "DWConfidences",
                newName: "DictionaryId");

            migrationBuilder.RenameColumn(
                name: "ConfidenceID",
                table: "DWConfidences",
                newName: "ConfidenceId");

            migrationBuilder.RenameIndex(
                name: "IX_DWConfidences_DictionaryID",
                table: "DWConfidences",
                newName: "IX_DWConfidences_DictionaryId");

            migrationBuilder.RenameColumn(
                name: "DictionaryID",
                table: "DictionaryWords",
                newName: "DictionaryId");

            migrationBuilder.RenameColumn(
                name: "ConfidenceID",
                table: "DictionaryWords",
                newName: "ConfidenceId");

            migrationBuilder.RenameColumn(
                name: "WordID",
                table: "DictionaryWords",
                newName: "WordId");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryWords_DictionaryID",
                table: "DictionaryWords",
                newName: "IX_DictionaryWords_DictionaryId");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryWords_ConfidenceID",
                table: "DictionaryWords",
                newName: "IX_DictionaryWords_ConfidenceId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserID",
                table: "Dictionaries",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "DictionaryID",
                table: "Dictionaries",
                newName: "DictionaryId");

            migrationBuilder.RenameIndex(
                name: "IX_Dictionaries_CreatedByUserID",
                table: "Dictionaries",
                newName: "IX_Dictionaries_CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_Users_CreatedByUserId",
                table: "Dictionaries",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWords_DWConfidences_ConfidenceId",
                table: "DictionaryWords",
                column: "ConfidenceId",
                principalTable: "DWConfidences",
                principalColumn: "ConfidenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWords_Dictionaries_DictionaryId",
                table: "DictionaryWords",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "DictionaryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWConfidences_Dictionaries_DictionaryId",
                table: "DWConfidences",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "DictionaryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWDetails_DictionaryWords_WordId",
                table: "DWDetails",
                column: "WordId",
                principalTable: "DictionaryWords",
                principalColumn: "WordId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWReviewHistories_DictionaryWords_WordId",
                table: "DWReviewHistories",
                column: "WordId",
                principalTable: "DictionaryWords",
                principalColumn: "WordId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_UserTypeId",
                table: "Users",
                column: "UserTypeId",
                principalTable: "UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCodes_Users_UserId",
                table: "VerificationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionaries_Users_CreatedByUserId",
                table: "Dictionaries");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWords_DWConfidences_ConfidenceId",
                table: "DictionaryWords");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryWords_Dictionaries_DictionaryId",
                table: "DictionaryWords");

            migrationBuilder.DropForeignKey(
                name: "FK_DWConfidences_Dictionaries_DictionaryId",
                table: "DWConfidences");

            migrationBuilder.DropForeignKey(
                name: "FK_DWDetails_DictionaryWords_WordId",
                table: "DWDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DWReviewHistories_DictionaryWords_WordId",
                table: "DWReviewHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_UserTypeId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCodes_Users_UserId",
                table: "VerificationCodes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "VerificationCodes",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "VerificationCodes",
                newName: "DeviceID");

            migrationBuilder.RenameColumn(
                name: "VerificationCodeId",
                table: "VerificationCodes",
                newName: "VerificationCodeID");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_UserId_DeviceId",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_UserID_DeviceID");

            migrationBuilder.RenameColumn(
                name: "UserTypeId",
                table: "UserTypes",
                newName: "UserTypeID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserSessions",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "UserSessions",
                newName: "DeviceID");

            migrationBuilder.RenameColumn(
                name: "UserSessionId",
                table: "UserSessions",
                newName: "UserSessionID");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId_DeviceId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserID_DeviceID");

            migrationBuilder.RenameColumn(
                name: "UserTypeId",
                table: "Users",
                newName: "UserTypeID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserTypeId",
                table: "Users",
                newName: "IX_Users_UserTypeID");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "DWReviewHistories",
                newName: "WordID");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "DWReviewHistories",
                newName: "ReviewID");

            migrationBuilder.RenameIndex(
                name: "IX_DWReviewHistories_WordId",
                table: "DWReviewHistories",
                newName: "IX_DWReviewHistories_WordID");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "DWDetails",
                newName: "WordID");

            migrationBuilder.RenameColumn(
                name: "WordDetailId",
                table: "DWDetails",
                newName: "WordDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_DWDetails_WordId",
                table: "DWDetails",
                newName: "IX_DWDetails_WordID");

            migrationBuilder.RenameColumn(
                name: "DictionaryId",
                table: "DWConfidences",
                newName: "DictionaryID");

            migrationBuilder.RenameColumn(
                name: "ConfidenceId",
                table: "DWConfidences",
                newName: "ConfidenceID");

            migrationBuilder.RenameIndex(
                name: "IX_DWConfidences_DictionaryId",
                table: "DWConfidences",
                newName: "IX_DWConfidences_DictionaryID");

            migrationBuilder.RenameColumn(
                name: "DictionaryId",
                table: "DictionaryWords",
                newName: "DictionaryID");

            migrationBuilder.RenameColumn(
                name: "ConfidenceId",
                table: "DictionaryWords",
                newName: "ConfidenceID");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "DictionaryWords",
                newName: "WordID");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryWords_DictionaryId",
                table: "DictionaryWords",
                newName: "IX_DictionaryWords_DictionaryID");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryWords_ConfidenceId",
                table: "DictionaryWords",
                newName: "IX_DictionaryWords_ConfidenceID");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Dictionaries",
                newName: "CreatedByUserID");

            migrationBuilder.RenameColumn(
                name: "DictionaryId",
                table: "Dictionaries",
                newName: "DictionaryID");

            migrationBuilder.RenameIndex(
                name: "IX_Dictionaries_CreatedByUserId",
                table: "Dictionaries",
                newName: "IX_Dictionaries_CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_Users_CreatedByUserID",
                table: "Dictionaries",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWords_DWConfidences_ConfidenceID",
                table: "DictionaryWords",
                column: "ConfidenceID",
                principalTable: "DWConfidences",
                principalColumn: "ConfidenceID");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryWords_Dictionaries_DictionaryID",
                table: "DictionaryWords",
                column: "DictionaryID",
                principalTable: "Dictionaries",
                principalColumn: "DictionaryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWConfidences_Dictionaries_DictionaryID",
                table: "DWConfidences",
                column: "DictionaryID",
                principalTable: "Dictionaries",
                principalColumn: "DictionaryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWDetails_DictionaryWords_WordID",
                table: "DWDetails",
                column: "WordID",
                principalTable: "DictionaryWords",
                principalColumn: "WordID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DWReviewHistories_DictionaryWords_WordID",
                table: "DWReviewHistories",
                column: "WordID",
                principalTable: "DictionaryWords",
                principalColumn: "WordID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_UserTypeID",
                table: "Users",
                column: "UserTypeID",
                principalTable: "UserTypes",
                principalColumn: "UserTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserID",
                table: "UserSessions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCodes_Users_UserID",
                table: "VerificationCodes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
