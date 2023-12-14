using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VkStorage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "FileChunks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "FileChunks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
