using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xarajat.Data.Migrations
{
    public partial class changedRoomIDToId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "rooms",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "rooms",
                newName: "ID");
        }
    }
}
