using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcPactice01.Migrations
{
    /// <inheritdoc />
    public partial class removeDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}
