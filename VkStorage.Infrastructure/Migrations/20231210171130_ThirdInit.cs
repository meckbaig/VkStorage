using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VkStorage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThirdInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileChunks_VkFiles_VkFileId",
                table: "FileChunks");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "FileChunks");

            migrationBuilder.AlterColumn<int>(
                name: "VkFileId",
                table: "FileChunks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VkFiles_Id",
                table: "VkFiles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileChunks_Id",
                table: "FileChunks",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileChunks_VkFiles_VkFileId",
                table: "FileChunks",
                column: "VkFileId",
                principalTable: "VkFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileChunks_VkFiles_VkFileId",
                table: "FileChunks");

            migrationBuilder.DropIndex(
                name: "IX_VkFiles_Id",
                table: "VkFiles");

            migrationBuilder.DropIndex(
                name: "IX_FileChunks_Id",
                table: "FileChunks");

            migrationBuilder.AlterColumn<int>(
                name: "VkFileId",
                table: "FileChunks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "FileChunks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_FileChunks_VkFiles_VkFileId",
                table: "FileChunks",
                column: "VkFileId",
                principalTable: "VkFiles",
                principalColumn: "Id");
        }
    }
}
