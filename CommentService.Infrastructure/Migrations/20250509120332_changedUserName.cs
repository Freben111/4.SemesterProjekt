using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changedUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Comments",
                newName: "UserId");
        }
    }
}
