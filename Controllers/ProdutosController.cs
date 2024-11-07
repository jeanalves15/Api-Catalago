using AutoMapper;
using Catalogo_Api.Context;
using Catalogo_Api.DTOs;
using Catalogo_Api.Migrations;
using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;
using Catalogo_Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace Catalogo_Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("produto/{id}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutoCategoriaAsync(int id)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriasAsync(id);
            if (produtos == null)
                return NotFound();
            var ProdutoDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(ProdutoDTO);

        }

        [HttpGet("pagination")]

        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAsync([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);
            return ObterProdutos(produtos);


        }
        [HttpGet("filter/preco/paginacao")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPrecoAsync([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos =await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPreco);
            return ObterProdutos(produtos);

        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Produto> produtos)
        {
            var metadados = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPage,
                produtos.HasNext,
                produtos.HasPrevious

            };
            Response.Headers.Append("X-pagination", JsonConvert.SerializeObject(metadados));

            var produtoDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtoDTO);
        }




        [HttpGet]
        [Authorize(Policy ="UserOnly")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAsync()
        {
            var produtos = await _uof.ProdutoRepository.GetAllAsync();
            if (produtos is null)
            {
                return NotFound("Produto não encontrado");
            }
            var ProdutoDTO=_mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(ProdutoDTO);
        }

        [HttpGet("{id:int}",Name ="ObterProduto")]

        public async Task<ActionResult<ProdutoDTO>> GetAsync(int id)
        {
            var produtos = await _uof.ProdutoRepository.GetAsync(c=> c.ProdutoId==id);
            if (produtos is null)
            {
                return NotFound("Produto não encontrado");
            }
            var ProdutoDTO=_mapper.Map<ProdutoDTO>(produtos);
            return Ok(ProdutoDTO);
        }

       

        [HttpPost]

        public async Task< ActionResult<ProdutoDTO>> PostAsync(ProdutoDTO produtoDTO)
        {
            if(produtoDTO is null)
            {
                return BadRequest();
            }

            var Produto=_mapper.Map<Produto>(produtoDTO);
          var NovoProduto=  _uof.ProdutoRepository.Create(Produto);
            await _uof.CommitAsync();
            var NovoProdutoDTO = _mapper.Map<ProdutoDTO>(NovoProduto);
            

            return new CreatedAtRouteResult("ObterProduto",
                new {id=NovoProduto.ProdutoId}, NovoProduto);
            

        }
        [HttpPatch("{id:int}/UpdatePartial")]

        public async Task<ActionResult<ProdutoDTOupdateResponse>> PatchAsync(int id,JsonPatchDocument<ProdutoDTOupdateRequest> patchProdutoDTO) 
        {
            if(patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }
            var produto=await _uof.ProdutoRepository.GetAsync(c=>c.ProdutoId==id);
            if (produto is null) return NotFound();
            var ProdutoUpdateRequest=_mapper.Map<ProdutoDTOupdateRequest>(produto);
            patchProdutoDTO.ApplyTo(ProdutoUpdateRequest, ModelState);
            if(!ModelState.IsValid || TryValidateModel(ProdutoUpdateRequest)) return BadRequest();
            _mapper.Map(ProdutoUpdateRequest, produto);
            _uof.ProdutoRepository.Update(produto);
           await _uof.CommitAsync();
            return Ok(_mapper.Map<ProdutoDTOupdateResponse>(produto));
        }


        [HttpPut("{id:int}")]

        public async Task<ActionResult<ProdutoDTO>> PutAsync(int id,ProdutoDTO produtoDTO) 
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest();
            }

            var Produto = _mapper.Map<Produto>(produtoDTO);
            var produtoAtualizado=_uof.ProdutoRepository.Update(Produto);
           await _uof.CommitAsync();
            var NovoProdutoDTO = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(NovoProdutoDTO);
        
        
        }


        [HttpDelete("{id:int}")]
        
        public async Task <ActionResult<ProdutoDTO>> DeleteAsync(int id)
        {
            var produtos= await _uof.ProdutoRepository.GetAsync(c=>c.ProdutoId==id);
            if (produtos is null)
            {
                return NotFound("Produto não encontrado");
            }
            var produtoDeletado=_uof.ProdutoRepository.Delete(produtos);
            await _uof.CommitAsync();

            var ProdutoDeletadoDTO = _mapper.Map<ProdutoDTO>(produtoDeletado);
            return Ok(ProdutoDeletadoDTO);

        }
















        



















        
    }
}
