using Catalogo_Api.Context;

namespace Catalogo_Api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepository _ProdutoRepo;
    private ICategoriaRepository _CategoriaRepo;

    public appDbContext _context;

    public UnitOfWork(appDbContext context)
    {
        _context = context;
    }
    public IProdutoRepository ProdutoRepository
    {
        get
        {
            return _ProdutoRepo=_ProdutoRepo ?? new ProdutoRepository(_context);
        }
    }
    public ICategoriaRepository CategoriaRepository
    {
        get
        {
            return _CategoriaRepo = _CategoriaRepo ?? new CategoriaRepository(_context);
        }
    }

    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context?.Dispose();
    }
}
