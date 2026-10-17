
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        return await _context.Categorias.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound();
        }

        return categoria;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Categoria>> CriarCategoria(Categoria categoria)
    {
        _context.Categorias.Add(categoria);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategoria),
            new { id = categoria.IdCategoria },
            categoria
        );
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarCategoria(int id, Categoria categoria)
    {
        if (id != categoria.IdCategoria)
        {
            return BadRequest();
        }

        var categoriaExistente = await _context.Categorias.FindAsync(id);

        if (categoriaExistente == null)
        {
            return NotFound();
        }

        categoriaExistente.Nome = categoria.Nome;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound();
        }

        _context.Categorias.Remove(categoria);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
