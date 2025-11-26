using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserPhoneNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    UserDirection = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.CheckConstraint("Check_UserType", "\"UserType\" IN ('Administrador', 'Usuario', 'Organizador', 'Soporte')");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
