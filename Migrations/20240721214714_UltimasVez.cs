using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogo_Api.Migrations
{
    /// <inheritdoc />
    public partial class UltimasVez : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MB)
        {
            MB.Sql("Insert into Categorias(Nome, ImagemUrl) Values ('Bebidas','bebidas.jpg')");
            MB.Sql("Insert into Categorias(Nome, ImagemUrl) Values ('Lanches','lanches.jpg')");
            MB.Sql("Insert into Categorias(Nome, ImagemUrl) Values ('Sobremesas','sobremesas.jpg')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MB)
        {
            MB.Sql("Delete from Categorias");
        }
    }
}
