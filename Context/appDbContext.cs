using Catalogo_Api.Models;
using Catalogo_Api.Properties.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalogo_Api.Context;

public class appDbContext : IdentityDbContext<ApplicationUser>
{
    public appDbContext(DbContextOptions<appDbContext> options ) : base( options ) 
    {
    }
    public DbSet<Categoria>? Categorias {  get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }



}
