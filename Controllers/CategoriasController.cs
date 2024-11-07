using Catalogo_Api.Context;
using Catalogo_Api.DTOs;
using Catalogo_Api.DTOs.Mappings;
using Catalogo_Api.Migrations;
using Catalogo_Api.Pagination;
using Catalogo_Api.Properties.Models;
using Catalogo_Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Catalogo_Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public CategoriasController(IUnitOfWork uof)
        {
            _uof = uof;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task <ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
        {
            var categorias = await _uof.CategoriaRepository.GetAllAsync();
            if(categorias is null) { return NotFound(); }

     
            var CategoriaListDTO = categorias.ToCategoriasDtoList();
            return Ok(CategoriaListDTO);



        }
        [HttpGet("{id:int}",Name ="ObterCategoria")]

        public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
        {

            var categoria = await _uof.CategoriaRepository.GetAsync(c=> c.CategoriaId==id);
            if(categoria is null)
            {
                return NotFound();
            }
           
            var categoriaDTO=categoria.ToCategoriaDTO();
            return categoriaDTO;
        }
        [HttpGet("Paginação")]
        public async Task< ActionResult<IEnumerable<CategoriaDTO>>> GetAsync([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
            return ObterCategorias(categorias);

        }

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria> categorias)
        {
            var metadados = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPage,
                categorias.HasNext,
                categorias.HasPrevious
            };
            Response.Headers.Append("X-pagination", JsonConvert.SerializeObject(metadados));
            var categoriasDTO = categorias.ToCategoriasDtoList();
            return Ok(categoriasDTO);
        }

        [HttpGet("Filter/nome/pagination")]
        public async Task <ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradasAsync([FromQuery] CategoriasFiltroNome categoriasParameters)
        {
           var categoriasFiltradas=await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
           return ObterCategorias(categoriasFiltradas);
        

        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> PostAsync(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO is null)
            {
                return BadRequest();
            }
            
            var categoria = categoriaDTO.ToCategoria();

            var CategociaCriada =  _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();
           
            var NovacategoriaDTO = CategociaCriada.ToCategoriaDTO();



            return new CreatedAtRouteResult("ObterCategoria",new {id=CategociaCriada.CategoriaId});

            return new CreatedAtRouteResult("ObterCategoria",
              new { id = NovacategoriaDTO.CategoriaId }, NovacategoriaDTO);
        }
        [HttpPut("{id:int}")]

        public async Task <ActionResult<CategoriaDTO>> PutAsync(int id,CategoriaDTO categoriaDTO ) 
        {
           if(id !=categoriaDTO.CategoriaId)
           {
                return BadRequest();
           }
           
            var categoria = categoriaDTO.ToCategoria();

           var CategoriasAtualizada= _uof.CategoriaRepository.Update(categoria);
           await _uof.CommitAsync();
            
            var CategoriaAtualizadaDTO=categoria.ToCategoriaDTO();


            return Ok(CategoriaAtualizadaDTO);
        }
        [HttpDelete("{id=int}")]
        [Authorize(Policy ="AdminOnly")]

        public async Task< ActionResult<CategoriaDTO>> DeleteAsync(int id)
        {
            var categoria=await _uof.CategoriaRepository.GetAsync(c=> c.CategoriaId==id);
            if (categoria is null)
            {
                return BadRequest();
            }
            var categoriaExcluida=_uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();
          
            var CategoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();
            return Ok(CategoriaExcluidaDTO);
        }
            


























        

    }
}
