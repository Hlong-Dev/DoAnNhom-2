using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnNhom_2.Data.Migrations
{
    public partial class v24NameProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDiscountCodes");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderDetails");

            migrationBuilder.CreateTable(
                name: "UserDiscountCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountCodeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscountCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiscountCodes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserDiscountCodes_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCodes_DiscountCodeId",
                table: "UserDiscountCodes",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCodes",
                column: "UserId");
        }
    }
}
