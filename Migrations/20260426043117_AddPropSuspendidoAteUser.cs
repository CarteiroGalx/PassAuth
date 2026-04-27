using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddPropSuspendidoAteUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedUntil",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuspendedUntil",
                table: "Users");
        }
    }
}
