using Catalogo_Api.Context;
using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo_Api.Repositories;

public class CategoriaRepository :Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(appDbContext context):base(context)
    {
        
    }

    public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters parameters)
    {
        var categorias = await GetAllAsync();
        var CategoriasOrdenadas= categorias.OrderBy(p=>p.CategoriaId).AsQueryable();
        var resultado=PagedList<Categoria>.ToPagedList(CategoriasOrdenadas,parameters.PageNumber,parameters.PageSize);
        return resultado;
    }

    public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categorias=categorias.Where(p=>p.Nome.Contains(categoriasFiltroNome.Nome,StringComparison.OrdinalIgnoreCase));
        }
        var categoriasFiltradas=PagedList<Categoria>.ToPagedList(categorias.AsQueryable(),categoriasFiltroNome.PageNumber,categoriasFiltroNome.PageSize);
        return categoriasFiltradas;
    }
}

