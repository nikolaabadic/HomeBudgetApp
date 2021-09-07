using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBudgetApp.Domain.Migrations.HomeBudget
{
    public partial class Alter_transactions_table_added_account_number : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Transactions");
        }
    }
}
