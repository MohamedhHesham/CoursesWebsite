using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesWebsite.Migrations
{
    /// <inheritdoc />
    public partial class FixingUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_userId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "learningProgressId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_userId",
                table: "LearningProgresses",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_userId",
                table: "LearningProgresses");

            migrationBuilder.AddColumn<Guid>(
                name: "learningProgressId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_userId",
                table: "LearningProgresses",
                column: "userId",
                unique: true);
        }
    }
}
