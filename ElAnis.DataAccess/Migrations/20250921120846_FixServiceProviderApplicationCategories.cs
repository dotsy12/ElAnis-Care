using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElAnis.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixServiceProviderApplicationCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderProfiles_AverageRating",
                table: "ServiceProviderProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderProfiles_IsAvailable",
                table: "ServiceProviderProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderProfiles_Status",
                table: "ServiceProviderProfiles");

            migrationBuilder.RenameColumn(
                name: "SelectedCategories",
                table: "ServiceProviderApplications",
                newName: "SelectedCategoriesJson");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEarnings",
                table: "ServiceProviderProfiles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServiceProviderProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "ServiceProviderProfiles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Experience",
                table: "ServiceProviderProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "ServiceProviderProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServiceProviderApplications",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "ServiceProviderApplications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ServiceProviderApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "ServiceProviderApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceProviderApplications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ServiceProviderApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderApplications_CreatedAt",
                table: "ServiceProviderApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderApplications_NationalId",
                table: "ServiceProviderApplications",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderApplications_Status",
                table: "ServiceProviderApplications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderApplications_CreatedAt",
                table: "ServiceProviderApplications");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderApplications_NationalId",
                table: "ServiceProviderApplications");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProviderApplications_Status",
                table: "ServiceProviderApplications");

            migrationBuilder.RenameColumn(
                name: "SelectedCategoriesJson",
                table: "ServiceProviderApplications",
                newName: "SelectedCategories");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalEarnings",
                table: "ServiceProviderProfiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServiceProviderProfiles",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "ServiceProviderProfiles",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Experience",
                table: "ServiceProviderProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "ServiceProviderProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServiceProviderApplications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceProviderApplications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderProfiles_AverageRating",
                table: "ServiceProviderProfiles",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderProfiles_IsAvailable",
                table: "ServiceProviderProfiles",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderProfiles_Status",
                table: "ServiceProviderProfiles",
                column: "Status");
        }
    }
}
