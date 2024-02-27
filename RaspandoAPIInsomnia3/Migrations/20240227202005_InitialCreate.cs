using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaspandoAPIInsomnia3.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodRob = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuRob = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateLog = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Processo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InfLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProd = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.IdLog);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
