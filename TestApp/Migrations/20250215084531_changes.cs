using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Surveys_VideoId",
                table: "Surveys");

            migrationBuilder.AlterColumn<long>(
                name: "VideoId",
                table: "Surveys",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_VideoId",
                table: "Surveys",
                column: "VideoId",
                unique: true,
                filter: "[VideoId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Surveys_VideoId",
                table: "Surveys");

            migrationBuilder.AlterColumn<long>(
                name: "VideoId",
                table: "Surveys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_VideoId",
                table: "Surveys",
                column: "VideoId",
                unique: true);
        }
    }
}
