using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnNhom_2.Data.Migrations
{
    public partial class V13MoMOPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    partnerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accessKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orderInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    localMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    responseTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    errorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    extraData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
