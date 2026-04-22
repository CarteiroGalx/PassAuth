using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddPropAuthorParaRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Requests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Requests");
        }
    }
}
