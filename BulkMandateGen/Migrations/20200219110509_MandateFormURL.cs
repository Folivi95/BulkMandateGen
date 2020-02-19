using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkMandateGen.Migrations
{
    public partial class MandateFormURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MandateFormUrl",
                table: "Mandates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MandateFormUrl",
                table: "Mandates");
        }
    }
}
