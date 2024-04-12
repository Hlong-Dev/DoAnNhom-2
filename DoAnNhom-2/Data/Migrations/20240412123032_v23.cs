using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnNhom_2.Data.Migrations
{
    public partial class v23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCode_AspNetUsers_UserId1",
                table: "UserDiscountCode");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCode_DiscountCodes_DiscountCodeId",
                table: "UserDiscountCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDiscountCode",
                table: "UserDiscountCode");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountCode_UserId1",
                table: "UserDiscountCode");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserDiscountCode");

            migrationBuilder.RenameTable(
                name: "UserDiscountCode",
                newName: "UserDiscountCodes");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCode_DiscountCodeId",
                table: "UserDiscountCodes",
                newName: "IX_UserDiscountCodes_DiscountCodeId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDiscountCodes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserDiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDiscountCodes",
                table: "UserDiscountCodes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId",
                table: "UserDiscountCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_DiscountCodes_DiscountCodeId",
                table: "UserDiscountCodes",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId",
                table: "UserDiscountCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_DiscountCodes_DiscountCodeId",
                table: "UserDiscountCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDiscountCodes",
                table: "UserDiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCodes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserDiscountCodes");

            migrationBuilder.RenameTable(
                name: "UserDiscountCodes",
                newName: "UserDiscountCode");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCodes_DiscountCodeId",
                table: "UserDiscountCode",
                newName: "IX_UserDiscountCode_DiscountCodeId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDiscountCode",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserDiscountCode",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDiscountCode",
                table: "UserDiscountCode",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCode_UserId1",
                table: "UserDiscountCode",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCode_AspNetUsers_UserId1",
                table: "UserDiscountCode",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCode_DiscountCodes_DiscountCodeId",
                table: "UserDiscountCode",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
