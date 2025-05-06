using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedModeratorIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "ModeratorIds",
                table: "Forums",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Forums",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeratorIds",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Forums");
        }
    }
}
