using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SobMedidaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToPersonalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "PersonalInfos",
                newName: "Address_ZipCode");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "PersonalInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Neighborhood",
                table: "PersonalInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Number",
                table: "PersonalInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "PersonalInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "PersonalInfos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "Address_Neighborhood",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "Address_Number",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "PersonalInfos");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "PersonalInfos");

            migrationBuilder.RenameColumn(
                name: "Address_ZipCode",
                table: "PersonalInfos",
                newName: "Location");
        }
    }
}
