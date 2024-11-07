using Catalogo_Api.Context;
using Catalogo_Api.Migrations;
using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;

namespace Catalogo_Api.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        

        public ProdutoRepository(appDbContext contextAccessor):base(contextAccessor) 
        {
           
        }


        public async Task<IEnumerable<Produto>> GetProdutosPorCategoriasAsync(int id)
        {
            var produtos= await GetAllAsync();
            var produtosCategorias=produtos.Where(p=>p.CategoriaId==id).ToList();
            return produtosCategorias;

        }
        
        public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters)
        {
            var produto = await GetAllAsync();
            var produtosOrdenados=produto.OrderBy(p => p.ProdutoId).AsQueryable();
            var resultado=PagedList<Produto>.ToPagedList(produtosOrdenados, produtosParameters.PageNumber,produtosParameters.PageSize);
            return resultado;
        }

        public async Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco ProdutosFiltros)
        {
            var produtos =await GetAllAsync();
            if (ProdutosFiltros.preco.HasValue && !string.IsNullOrEmpty(ProdutosFiltros.precoSugerido))
            {
                if (ProdutosFiltros.precoSugerido.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco > ProdutosFiltros.preco.Value).OrderBy(p=>p.Preco);
                }
                else if (ProdutosFiltros.precoSugerido.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco < ProdutosFiltros.preco.Value).OrderBy(p => p.Preco);
                }
                else if (ProdutosFiltros.precoSugerido.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco == ProdutosFiltros.preco.Value).OrderBy(p => p.Preco);
                }
            }
            var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos.AsQueryable(), ProdutosFiltros.PageNumber, ProdutosFiltros.PageSize);
            return produtosFiltrados;
        }

        
    }
}
