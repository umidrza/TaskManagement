using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartTaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "77a2ed1a-b711-4bdd-b080-d54e3fa4f3d4", "Admin", "ADMIN" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "f3e0009f-ded3-401a-ab44-82126d8890fc", "User", "USER" }
                });
        }
    }
}
