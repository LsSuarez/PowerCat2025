using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pg1.Migrations
{
    public partial class InsertarDatosInicialesCategorias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { "Monitores", "Pantallas y monitores para gaming y oficina" },
                    { "Sillas Gamer", "Sillas ergonómicas para gamers" },
                    { "Teclados", "Teclados mecánicos y membrana para gaming" },
                    { "Mouses Gamer", "Ratones con alta precisión para gaming" },
                    { "GPU", "Tarjetas gráficas de última generación" },
                    { "Laptops", "Computadoras portátiles para gaming y trabajo" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "Monitores");
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "Sillas Gamer");
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "Teclados");
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "Mouses Gamer");
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "GPU");
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Nombre", keyValue: "Laptops");
        }
    }
}
