using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addedQuizAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_Medias_AnswerId",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_AnswerId",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Quiz");

            migrationBuilder.AddColumn<string>(
                name: "LecturerId",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Mark = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnswerId = table.Column<int>(type: "int", nullable: true),
                    QuizId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizAnswers_Medias_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Medias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizAnswers_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_LecturerId",
                table: "Quiz",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_AnswerId",
                table: "QuizAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizId",
                table: "QuizAnswers",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_StudentId",
                table: "QuizAnswers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_AspNetUsers_LecturerId",
                table: "Quiz",
                column: "LecturerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_AspNetUsers_LecturerId",
                table: "Quiz");

            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_LecturerId",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "LecturerId",
                table: "Quiz");

            migrationBuilder.AddColumn<int>(
                name: "AnswerId",
                table: "Quiz",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_AnswerId",
                table: "Quiz",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Medias_AnswerId",
                table: "Quiz",
                column: "AnswerId",
                principalTable: "Medias",
                principalColumn: "Id");
        }
    }
}
