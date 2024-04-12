using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnNhom_2.Data.Migrations
{
    public partial class v22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDiscountCode",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiscountCodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscountCode", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDiscountCode_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscountCode_DiscountCodes_DiscountCodeId",
                        column: x => x.DiscountCodeId,
                        principalTable: "DiscountCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCode_DiscountCodeId",
                table: "UserDiscountCode",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCode_UserId1",
                table: "UserDiscountCode",
                column: "UserId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDiscountCode");
        }
    }
}
