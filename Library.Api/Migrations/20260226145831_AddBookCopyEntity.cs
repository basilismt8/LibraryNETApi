using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCopyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_bookId",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "bookId",
                table: "Loans",
                newName: "bookCopyId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_bookId",
                table: "Loans",
                newName: "IX_Loans_bookCopyId");

            migrationBuilder.AlterColumn<int>(
                name: "totalCopies",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "copiesAvailable",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.CreateTable(
                name: "BookCopies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    copyCode = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Available")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopies", x => x.id);
                    table.ForeignKey(
                        name: "FK_BookCopies_Books_bookId",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_bookId",
                table: "BookCopies",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_copyCode",
                table: "BookCopies",
                column: "copyCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_bookCopyId",
                table: "Loans",
                column: "bookCopyId",
                principalTable: "BookCopies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_bookCopyId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "BookCopies");

            migrationBuilder.RenameColumn(
                name: "bookCopyId",
                table: "Loans",
                newName: "bookId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_bookCopyId",
                table: "Loans",
                newName: "IX_Loans_bookId");

            migrationBuilder.AlterColumn<int>(
                name: "totalCopies",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "copiesAvailable",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_bookId",
                table: "Loans",
                column: "bookId",
                principalTable: "Books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
