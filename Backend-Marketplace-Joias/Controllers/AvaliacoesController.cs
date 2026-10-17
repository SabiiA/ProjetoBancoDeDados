
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvaliacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AvaliacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Avaliacao>>> GetAvaliacoes()
    {
        return await _context.Avaliacoes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Avaliacao>> GetAvaliacao(int id)
    {
        var avaliacao = await _context.Avaliacoes.FindAsync(id);

        if (avaliacao == null)
        {
            return NotFound();
        }

        return avaliacao;
    }

    [HttpPost]
    public async Task<ActionResult<Avaliacao>> CriarAvaliacao(Avaliacao avaliacao)
    {
        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == avaliacao.IdCliente);

        if (!clienteExiste)
        {
            return BadRequest(new
            {
                mensagem = "O cliente informado não existe."
            });
        }

        var produtoExiste = await _context.Produtos
            .AnyAsync(p => p.IdProduto == avaliacao.IdProduto);

        if (!produtoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O produto informado não existe."
            });
        }

        avaliacao.IdAvaliacao = 0;

        if (avaliacao.DataAvaliacao == default)
        {
            avaliacao.DataAvaliacao = DateTime.UtcNow;
        }

        _context.Avaliacoes.Add(avaliacao);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAvaliacao),
            new { id = avaliacao.IdAvaliacao },
            avaliacao
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarAvaliacao(int id, Avaliacao avaliacao)
    {
        if (id != avaliacao.IdAvaliacao)
        {
            return BadRequest();
        }

        var avaliacaoExistente = await _context.Avaliacoes.FindAsync(id);

        if (avaliacaoExistente == null)
        {
            return NotFound();
        }

        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == avaliacao.IdCliente);

        if (!clienteExiste)
        {
            return BadRequest(new
            {
                mensagem = "O cliente informado não existe."
            });
        }

        var produtoExiste = await _context.Produtos
            .AnyAsync(p => p.IdProduto == avaliacao.IdProduto);

        if (!produtoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O produto informado não existe."
            });
        }

        avaliacaoExistente.IdCliente = avaliacao.IdCliente;
        avaliacaoExistente.IdProduto = avaliacao.IdProduto;
        avaliacaoExistente.Nota = avaliacao.Nota;
        avaliacaoExistente.Comentario = avaliacao.Comentario;
        avaliacaoExistente.DataAvaliacao = avaliacao.DataAvaliacao;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirAvaliacao(int id)
    {
        var avaliacao = await _context.Avaliacoes.FindAsync(id);

        if (avaliacao == null)
        {
            return NotFound();
        }

        _context.Avaliacoes.Remove(avaliacao);

        await _context.SaveChangesAsync();

        return NoContent();
    }

}
