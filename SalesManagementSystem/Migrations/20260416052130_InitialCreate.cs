using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SalesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleChargeType",
                columns: table => new
                {
                    ChargeTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargeTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleChargeType", x => x.ChargeTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SaleDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateLabel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalePlatform",
                columns: table => new
                {
                    PlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalePlatform", x => x.PlatformId);
                });

            migrationBuilder.CreateTable(
                name: "SaleStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleStatus", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "SaleTransactionType",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleTransactionType", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "SaleAccount",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    PlatformId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAccount", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_SaleAccount_SalePlatform_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "SalePlatform",
                        principalColumn: "PlatformId");
                });

            migrationBuilder.CreateTable(
                name: "SaleProduct",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleProduct", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_SaleProduct_SalePlatform_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "SalePlatform",
                        principalColumn: "PlatformId");
                });

            migrationBuilder.CreateTable(
                name: "SaleAcct",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    PlatformId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QtyHeld = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QtySold = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    FromAccountId = table.Column<int>(type: "int", nullable: true),
                    ToAccountId = table.Column<int>(type: "int", nullable: true),
                    TotalProCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmazonFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPromotion = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SoldAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalRroRebate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProfitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true, computedColumnSql: "ISNULL(SoldAmount,0) - ISNULL(CostAmount,0) - ISNULL(TotalProCharges,0) - ISNULL(AmazonFee,0) - ISNULL(OtherCharges,0) + ISNULL(TotalRroRebate,0)", stored: true),
                    AmzProRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Discription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAcct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleAcct_SaleAccount_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "SaleAccount",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_SaleAcct_SaleAccount_ToAccountId",
                        column: x => x.ToAccountId,
                        principalTable: "SaleAccount",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_SaleAcct_SalePlatform_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "SalePlatform",
                        principalColumn: "PlatformId");
                    table.ForeignKey(
                        name: "FK_SaleAcct_SaleProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "SaleProduct",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_SaleAcct_SaleStatus_StatusID",
                        column: x => x.StatusID,
                        principalTable: "SaleStatus",
                        principalColumn: "StatusID");
                    table.ForeignKey(
                        name: "FK_SaleAcct_SaleTransactionType_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "SaleTransactionType",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateTable(
                name: "SaleCharge",
                columns: table => new
                {
                    SaleChargeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleId = table.Column<long>(type: "bigint", nullable: false),
                    ChargeTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCharge", x => x.SaleChargeId);
                    table.ForeignKey(
                        name: "FK_SaleCharge_SaleAcct_SaleId",
                        column: x => x.SaleId,
                        principalTable: "SaleAcct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCharge_SaleChargeType_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalTable: "SaleChargeType",
                        principalColumn: "ChargeTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleTransactionDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleAcctId = table.Column<long>(type: "bigint", nullable: false),
                    DateLabelId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleTransactionDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleTransactionDates_SaleAcct_SaleAcctId",
                        column: x => x.SaleAcctId,
                        principalTable: "SaleAcct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleTransactionDates_SaleDates_DateLabelId",
                        column: x => x.DateLabelId,
                        principalTable: "SaleDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SaleDates",
                columns: new[] { "Id", "DateLabel" },
                values: new object[,]
                {
                    { 1, "TransactionDate" },
                    { 2, "PaymentDate" },
                    { 3, "OrderDate" },
                    { 4, "ProcessDate" },
                    { 5, "SoldDate" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleAccount_PlatformId",
                table: "SaleAccount",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_FromAccountId",
                table: "SaleAcct",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_PlatformId",
                table: "SaleAcct",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_ProductId",
                table: "SaleAcct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_StatusID",
                table: "SaleAcct",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_ToAccountId",
                table: "SaleAcct",
                column: "ToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleAcct_TransactionId",
                table: "SaleAcct",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCharge_ChargeTypeId",
                table: "SaleCharge",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCharge_SaleId",
                table: "SaleCharge",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleProduct_PlatformId",
                table: "SaleProduct",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransactionDates_DateLabelId",
                table: "SaleTransactionDates",
                column: "DateLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransactionDates_SaleAcctId",
                table: "SaleTransactionDates",
                column: "SaleAcctId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleCharge");

            migrationBuilder.DropTable(
                name: "SaleTransactionDates");

            migrationBuilder.DropTable(
                name: "SaleChargeType");

            migrationBuilder.DropTable(
                name: "SaleAcct");

            migrationBuilder.DropTable(
                name: "SaleDates");

            migrationBuilder.DropTable(
                name: "SaleAccount");

            migrationBuilder.DropTable(
                name: "SaleProduct");

            migrationBuilder.DropTable(
                name: "SaleStatus");

            migrationBuilder.DropTable(
                name: "SaleTransactionType");

            migrationBuilder.DropTable(
                name: "SalePlatform");
        }
    }
}
