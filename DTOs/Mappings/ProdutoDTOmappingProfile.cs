using AutoMapper;
using Catalogo_Api.Properties.Models;

namespace Catalogo_Api.DTOs.Mappings;

public class ProdutoDTOmappingProfile:Profile
{
    public ProdutoDTOmappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Produto,ProdutoDTOupdateRequest>().ReverseMap();
        CreateMap<Produto,ProdutoDTOupdateResponse>().ReverseMap();
    }

}
