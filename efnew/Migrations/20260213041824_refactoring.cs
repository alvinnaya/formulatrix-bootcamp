using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace efnew.Migrations
{
    /// <inheritdoc />
    public partial class refactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId1",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Postingan_PostinganId1",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Postingan_AspNetUsers_UserId1",
                table: "Postingan");

            migrationBuilder.DropIndex(
                name: "IX_Postingan_UserId1",
                table: "Postingan");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostinganId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Postingan");

            migrationBuilder.DropColumn(
                name: "PostinganId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostinganId",
                table: "Comment",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Postingan_UserId",
                table: "Postingan",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostinganId",
                table: "Comment",
                column: "PostinganId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Postingan_PostinganId",
                table: "Comment",
                column: "PostinganId",
                principalTable: "Postingan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Postingan_AspNetUsers_UserId",
                table: "Postingan",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Postingan_PostinganId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Postingan_AspNetUsers_UserId",
                table: "Postingan");

            migrationBuilder.DropIndex(
                name: "IX_Postingan_UserId",
                table: "Postingan");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostinganId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Postingan",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Comment",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "PostinganId",
                table: "Comment",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "PostinganId1",
                table: "Comment",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Comment",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Postingan_UserId1",
                table: "Postingan",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostinganId1",
                table: "Comment",
                column: "PostinganId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId1",
                table: "Comment",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId1",
                table: "Comment",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Postingan_PostinganId1",
                table: "Comment",
                column: "PostinganId1",
                principalTable: "Postingan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Postingan_AspNetUsers_UserId1",
                table: "Postingan",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
