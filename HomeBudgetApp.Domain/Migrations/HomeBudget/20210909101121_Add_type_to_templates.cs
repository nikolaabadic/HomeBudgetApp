using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBudgetApp.Domain.Migrations.HomeBudget
{
    public partial class Add_type_to_templates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Templates");
        }
    }
}
