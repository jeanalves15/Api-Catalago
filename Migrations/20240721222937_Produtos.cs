using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogo_Api.Migrations
{
    /// <inheritdoc />
    public partial class Produtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MB)
        {

            MB.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                " Values('Coca Cola Diet','Refrigerante de Cola 350ml',5.45,'cocacola.jpg',50,now(),1)");
            MB.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                " Values('Lanche de atum','Lanche de atum com maionese',8.50,'atum.jpg',10,now(),2)");
            MB.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                " Values('Pudim 100g','Pudim de leite condesado l100g',6.75,'pudim.jpg',20,now(),3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MB)
        {
            MB.Sql("Delete from Produtos");
        }
    }
}
