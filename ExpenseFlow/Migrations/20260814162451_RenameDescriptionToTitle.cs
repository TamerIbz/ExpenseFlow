using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseFlow.Migrations
{
    /// <inheritdoc />
    public partial class RenameDescriptionToTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Expenses",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Expenses",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Expenses",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Expenses",
                newName: "Description");
        }
    }
}
