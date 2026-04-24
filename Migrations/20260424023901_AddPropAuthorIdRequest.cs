using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddPropAuthorIdRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Requests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Requests");
        }
    }
}
