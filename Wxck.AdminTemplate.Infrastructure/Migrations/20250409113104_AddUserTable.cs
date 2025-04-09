using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wxck.AdminTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "User_UserInfo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InviteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User_UserVipInfo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VipName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VipLevel = table.Column<int>(type: "int", nullable: false),
                    VipExperience = table.Column<int>(type: "int", nullable: false),
                    RechargePoints = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VipDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_UserVipInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserVipInfo_User_UserInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User_UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserInfo_CreatedTime",
                schema: "dbo",
                table: "User_UserInfo",
                column: "CreatedTime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserInfo_UserCode",
                schema: "dbo",
                table: "User_UserInfo",
                column: "UserCode",
                unique: true,
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserVipInfo_UserId",
                schema: "dbo",
                table: "User_UserVipInfo",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_UserVipInfo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "User_UserInfo",
                schema: "dbo");
        }
    }
}
