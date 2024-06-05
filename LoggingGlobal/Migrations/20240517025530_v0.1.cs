using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoggingGlobal.Migrations
{
    /// <inheritdoc />
    public partial class v01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Response",
                table: "HttpLogs",
                newName: "ResponseBody");

            migrationBuilder.RenameColumn(
                name: "Request",
                table: "HttpLogs",
                newName: "RequestBody");

            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "HttpLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "HttpLogs");

            migrationBuilder.RenameColumn(
                name: "ResponseBody",
                table: "HttpLogs",
                newName: "Response");

            migrationBuilder.RenameColumn(
                name: "RequestBody",
                table: "HttpLogs",
                newName: "Request");
        }
    }
}
