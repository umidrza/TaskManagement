using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_AspNetUsers_UserId",
                table: "TaskComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_Tasks_TaskId",
                table: "TaskComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskComment",
                table: "TaskComment");

            migrationBuilder.RenameTable(
                name: "TaskComment",
                newName: "TaskComments");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComment_UserId",
                table: "TaskComments",
                newName: "IX_TaskComments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComment_TaskId",
                table: "TaskComments",
                newName: "IX_TaskComments_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskComments",
                table: "TaskComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_AspNetUsers_UserId",
                table: "TaskComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_Tasks_TaskId",
                table: "TaskComments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_AspNetUsers_UserId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_Tasks_TaskId",
                table: "TaskComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskComments",
                table: "TaskComments");

            migrationBuilder.RenameTable(
                name: "TaskComments",
                newName: "TaskComment");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_UserId",
                table: "TaskComment",
                newName: "IX_TaskComment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComment",
                newName: "IX_TaskComment_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskComment",
                table: "TaskComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_AspNetUsers_UserId",
                table: "TaskComment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_Tasks_TaskId",
                table: "TaskComment",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
