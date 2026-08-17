using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringType",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurringType",
                table: "Expenses");
        }
    }
}
