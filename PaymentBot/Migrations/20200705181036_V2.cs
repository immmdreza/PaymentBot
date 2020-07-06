using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentBot.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TsPayments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Authority = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    PaymentGoal = table.Column<int>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    IsSuccesseded = table.Column<bool>(nullable: false),
                    PayMessageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TsPayments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TsPayments");
        }
    }
}
