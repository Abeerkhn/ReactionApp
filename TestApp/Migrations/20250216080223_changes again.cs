using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations
{
    /// <inheritdoc />
    public partial class changesagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReactionId",
                table: "UserSurveyResponses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserSurveyResponses_ReactionId",
                table: "UserSurveyResponses",
                column: "ReactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSurveyResponses_UserReactions_ReactionId",
                table: "UserSurveyResponses",
                column: "ReactionId",
                principalTable: "UserReactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSurveyResponses_UserReactions_ReactionId",
                table: "UserSurveyResponses");

            migrationBuilder.DropIndex(
                name: "IX_UserSurveyResponses_ReactionId",
                table: "UserSurveyResponses");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "UserSurveyResponses");
        }
    }
}
