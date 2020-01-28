using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkMandateGen.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mandates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayerName = table.Column<string>(nullable: true),
                    PayerEmail = table.Column<string>(nullable: true),
                    PayerPhone = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    EndDate = table.Column<string>(nullable: true),
                    StartDate = table.Column<string>(nullable: true),
                    MandateType = table.Column<string>(nullable: true),
                    MaxNoOfDebits = table.Column<string>(nullable: true),
                    PayerAccount = table.Column<string>(nullable: true),
                    PayerBankCode = table.Column<string>(nullable: true),
                    MandateId = table.Column<string>(nullable: true),
                    RequestId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mandates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mandates");
        }
    }
}
