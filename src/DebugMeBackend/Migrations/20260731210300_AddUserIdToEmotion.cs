using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebugMeBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEmotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Emotions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emotions_UserId",
                table: "Emotions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emotions_Users_UserId",
                table: "Emotions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emotions_Users_UserId",
                table: "Emotions");

            migrationBuilder.DropIndex(
                name: "IX_Emotions_UserId",
                table: "Emotions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Emotions");
        }
    }
}
