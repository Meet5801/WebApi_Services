using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crud_Operation.Migrations
{
    /// <inheritdoc />
    public partial class Uploadexceldatatodb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Row = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Face = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EDelivery = table.Column<bool>(type: "bit", nullable: false),
                    InternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcurementNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InHand = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    AvailableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrders");
        }
    }
}
