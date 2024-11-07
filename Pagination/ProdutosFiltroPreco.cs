namespace Catalogo_Api.Pagination;

public class ProdutosFiltroPreco:QueryStringParameters
{
    public decimal? preco {  get; set; }
    public string? precoSugerido { get; set; }
}
