using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsManageApp.Migrations
{
    public partial class UpdateProductUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6bfb5770-8752-472f-a0e4-dd4469dc7854");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eefb028f-0aa6-4cab-94c4-b39f15b70185");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "cc059d84-7c2c-4eb9-828e-397679bebc45", "4f63932d-3584-4abf-bf36-d239238ed394", "Administrator", "ADMINISTRATOR" },
                    { "f174deb6-e87e-433c-a0b0-332bfbe4f56e", "4686838f-1c86-4b98-855b-a177f3e8446e", "Visitor", "VISITOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc059d84-7c2c-4eb9-828e-397679bebc45");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f174deb6-e87e-433c-a0b0-332bfbe4f56e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6bfb5770-8752-472f-a0e4-dd4469dc7854", "e6085efe-9655-41f0-ac38-281b1cd5274e", "Visitor", "VISITOR" },
                    { "eefb028f-0aa6-4cab-94c4-b39f15b70185", "dfc619d8-e962-45f0-9cdd-b071ac18b866", "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
