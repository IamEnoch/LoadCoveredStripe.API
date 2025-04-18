using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadCoveredStripe.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialStripePaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_billing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    stripe_customer_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    stripe_subscription_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    price_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    current_period_start = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    current_period_end = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    cancel_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    paused_from = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    paused_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_billing", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_billing_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "price_catalog",
                columns: table => new
                {
                    price_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    unit_amount = table.Column<long>(type: "bigint", nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    interval = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    trial_days = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_synced_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_catalog", x => x.price_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_customer_billing_customer_id",
                table: "customer_billing",
                column: "customer_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_billing");

            migrationBuilder.DropTable(
                name: "price_catalog");
        }
    }
}
