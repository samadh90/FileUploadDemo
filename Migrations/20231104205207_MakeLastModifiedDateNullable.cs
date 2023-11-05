using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileUploadDemo.Migrations
{
    /// <inheritdoc />
    public partial class MakeLastModifiedDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Files",
                newName: "ID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Files",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Files",
                newName: "Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
