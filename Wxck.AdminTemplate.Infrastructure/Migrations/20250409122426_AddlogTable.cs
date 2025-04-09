using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wxck.AdminTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddlogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log_OperationLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    RequestIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<long>(type: "bigint", nullable: false),
                    OperationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestMethod = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeSpent = table.Column<long>(type: "bigint", nullable: false),
                    IsQueryDb = table.Column<bool>(type: "bit", nullable: false),
                    QueryDbElapsed = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_OperationLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Log_OperationLog_CreatedTime",
                schema: "dbo",
                table: "Log_OperationLog",
                column: "CreatedTime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Log_OperationLog_OperationTime",
                schema: "dbo",
                table: "Log_OperationLog",
                column: "OperationTime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Log_OperationLog_OperatorId",
                schema: "dbo",
                table: "Log_OperationLog",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_OperationLog_RequestMethod",
                schema: "dbo",
                table: "Log_OperationLog",
                column: "RequestMethod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log_OperationLog",
                schema: "dbo");
        }
    }
}
