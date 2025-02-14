using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations
{
    /// <inheritdoc />
    public partial class Behaviourchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_UploadedBy",
                table: "Videos");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_UploadedBy",
                table: "Videos",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_UploadedBy",
                table: "Videos");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_UploadedBy",
                table: "Videos",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
