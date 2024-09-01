using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class modifiedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_AspNetUsers_LecturerId",
                table: "StudyMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_Departments_DepartmentId",
                table: "StudyMaterials");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_DepartmentId",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "StudyMaterials");

            migrationBuilder.RenameColumn(
                name: "LecturerId",
                table: "StudyMaterials",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyMaterials_LecturerId",
                table: "StudyMaterials",
                newName: "IX_StudyMaterials_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "Semester",
                table: "Courses",
                newName: "SemesterId");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "AspNetUsers",
                newName: "SemesterId");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "StudyMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LevelId",
                table: "Courses",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SemesterId",
                table: "Courses",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LevelId",
                table: "AspNetUsers",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SemesterId",
                table: "AspNetUsers",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommonDropDowns_LevelId",
                table: "AspNetUsers",
                column: "LevelId",
                principalTable: "CommonDropDowns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommonDropDowns_SemesterId",
                table: "AspNetUsers",
                column: "SemesterId",
                principalTable: "CommonDropDowns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CommonDropDowns_LevelId",
                table: "Courses",
                column: "LevelId",
                principalTable: "CommonDropDowns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CommonDropDowns_SemesterId",
                table: "Courses",
                column: "SemesterId",
                principalTable: "CommonDropDowns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_AspNetUsers_ApplicationUserId",
                table: "StudyMaterials",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommonDropDowns_LevelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommonDropDowns_SemesterId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CommonDropDowns_LevelId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CommonDropDowns_SemesterId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_AspNetUsers_ApplicationUserId",
                table: "StudyMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Courses_LevelId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SemesterId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LevelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SemesterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "StudyMaterials",
                newName: "LecturerId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyMaterials_ApplicationUserId",
                table: "StudyMaterials",
                newName: "IX_StudyMaterials_LecturerId");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Courses",
                newName: "Semester");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "AspNetUsers",
                newName: "Level");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "StudyMaterials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudyMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "StudyMaterials",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_DepartmentId",
                table: "StudyMaterials",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_AspNetUsers_LecturerId",
                table: "StudyMaterials",
                column: "LecturerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_Departments_DepartmentId",
                table: "StudyMaterials",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
