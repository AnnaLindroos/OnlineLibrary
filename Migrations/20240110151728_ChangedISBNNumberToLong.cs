using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ChangedISBNNumberToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ISBNNumber",
                table: "ISBNs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISBNNumber",
                table: "ISBNs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
