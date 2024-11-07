using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Catalogo_Api.Properties.Models;

public class Categoria
{
    

    public Categoria()
    {
            produtos=new Collection<Produto>();
    }
    public int CategoriaId { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    [JsonIgnore]
   public ICollection<Produto> produtos { get; set; }


}
