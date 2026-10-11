using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursesWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddingLearningProgress_AndfurtherModelsTweaking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Lessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Lessons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
