using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;

namespace Catalogo_Api.Repositories;

public interface ICategoriaRepository:IRepository<Categoria>
{
   Task< PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters parameters);
   Task< PagedList<Categoria>> GetCategoriasAsync(CategoriasFiltroNome categoriasFiltroNome);

}
