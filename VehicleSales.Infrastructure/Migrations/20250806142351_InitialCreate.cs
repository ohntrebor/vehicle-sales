using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleSales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    is_sold = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    buyer_cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    sale_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payment_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_buyer_cpf",
                table: "vehicles",
                column: "buyer_cpf",
                filter: "buyer_cpf IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_is_sold",
                table: "vehicles",
                column: "is_sold");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_payment_code",
                table: "vehicles",
                column: "payment_code",
                unique: true,
                filter: "payment_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_sold_date",
                table: "vehicles",
                columns: new[] { "is_sold", "sale_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicles");
        }
    }
}
