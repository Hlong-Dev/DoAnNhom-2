using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnNhom_2.Data.Migrations
{
    public partial class V11MomoThongtin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "MoMoPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MoMoPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "MoMoPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "MoMoPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "MoMoPayments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "MoMoPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "MoMoPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "MoMoPayments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MoMoPayments");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "MoMoPayments");
        }
    }
}
