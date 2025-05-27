using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowWise.Services.Consolidacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaldosDiarios",
                columns: table => new
                {
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SaldoTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCreditos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalDebitos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosDiarios", x => x.Data);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaldosDiarios");
        }
    }
}
