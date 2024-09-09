using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addedMediaTypeToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUploaded",
                table: "StudyMaterials");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "StudyMaterials",
                newName: "Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "StudyMaterials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaTypeId",
                table: "StudyMaterials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicalPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_MediaTypeId",
                table: "StudyMaterials",
                column: "MediaTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_Medias_MediaTypeId",
                table: "StudyMaterials",
                column: "MediaTypeId",
                principalTable: "Medias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_Medias_MediaTypeId",
                table: "StudyMaterials");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_MediaTypeId",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "MediaTypeId",
                table: "StudyMaterials");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StudyMaterials",
                newName: "FileUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUploaded",
                table: "StudyMaterials",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
