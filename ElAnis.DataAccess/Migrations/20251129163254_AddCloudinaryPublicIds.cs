using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElAnis.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudinaryPublicIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePublicId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVPublicId",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificatePublicId",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdDocumentPublicId",
                table: "ServiceProviderApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicturePublicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CVPublicId",
                table: "ServiceProviderApplications");

            migrationBuilder.DropColumn(
                name: "CertificatePublicId",
                table: "ServiceProviderApplications");

            migrationBuilder.DropColumn(
                name: "IdDocumentPublicId",
                table: "ServiceProviderApplications");
        }
    }
}
