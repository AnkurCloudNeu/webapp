using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.CloudApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    AccountCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccountUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    DocumentID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BucketPath = table.Column<string>(type: "text", nullable: false),
                    DocumentCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_document_account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_document_AccountID",
                table: "document",
                column: "AccountID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
