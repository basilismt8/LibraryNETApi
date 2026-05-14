using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanHistories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    loanDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    returnDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanHistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_LoanHistories_BookCopies_bookCopyId",
                        column: x => x.bookCopyId,
                        principalTable: "BookCopies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistories_bookCopyId",
                table: "LoanHistories",
                column: "bookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistories_userId",
                table: "LoanHistories",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanHistories");
        }
    }
}
