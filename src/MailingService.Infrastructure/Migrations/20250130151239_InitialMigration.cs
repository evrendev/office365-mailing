using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailTemplateId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTrackers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTrackers_RecipientEmail_EmailTemplateId",
                table: "EmailTrackers",
                columns: new[] { "RecipientEmail", "EmailTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTrackers_SentDate",
                table: "EmailTrackers",
                column: "SentDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTrackers");
        }
    }
}
