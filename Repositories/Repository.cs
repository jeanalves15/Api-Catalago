using Catalogo_Api.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Catalogo_Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly appDbContext _context;

    public Repository(appDbContext context)
    {
        _context = context;
    }
    public async Task< IEnumerable<T>> GetAllAsync()
    {
     return await _context.Set<T>().AsNoTracking().ToListAsync();
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
       return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);
        //_context.SaveChanges();
        return entity;
    }
    public T Update(T entity)
    {
       _context.Set<T>().Update(entity);
       //_context.SaveChanges();
        return entity;
    }

    public T Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        //_context.SaveChanges();
        return entity;
        
    }

    

    

    
}
