using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
    {
        return await _context.Produtos.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> GetProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
        {
            return NotFound();
        }

        return produto;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Produto>> CriarProduto(Produto produto)
    {
        _context.Produtos.Add(produto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduto),
            new { id = produto.IdProduto },
            produto
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, Produto produto)
    {
        if (id != produto.IdProduto)
        {
            return BadRequest();
        }

        var produtoExistente = await _context.Produtos.FindAsync(id);

        if (produtoExistente == null)
        {
            return NotFound();
        }

        produtoExistente.Nome = produto.Nome;
        produtoExistente.Descricao = produto.Descricao;
        produtoExistente.Preco = produto.Preco;
        produtoExistente.Estoque = produto.Estoque;
        produtoExistente.IdCategoria = produto.IdCategoria;
        produtoExistente.Material = produto.Material;
        produtoExistente.Tamanho = produto.Tamanho;
        produtoExistente.Imagem = produto.Imagem;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
        {
            return NotFound();
        }

        _context.Produtos.Remove(produto);

        await _context.SaveChangesAsync();

        return NoContent();
    }

}