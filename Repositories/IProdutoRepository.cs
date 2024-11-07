using Catalogo_Api.Migrations;
using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;
using System.Collections;

namespace Catalogo_Api.Repositories;

public interface IProdutoRepository :IRepository<Produto>
{
   // IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters);
    Task <PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);
    Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);
    Task<IEnumerable<Produto>> GetProdutosPorCategoriasAsync(int id);
}
