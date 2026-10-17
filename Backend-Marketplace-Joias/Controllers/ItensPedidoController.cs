
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItensPedidoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItensPedidoController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemPedido>>> GetItensPedido()
    {
        return await _context.ItensPedido.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ItemPedido>> GetItemPedido(int id)
    {
        var item = await _context.ItensPedido.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }


    [HttpPost]
    public async Task<ActionResult<ItemPedido>> CriarItemPedido(ItemPedido item)
    {
        var pedidoExiste = await _context.Pedidos
            .AnyAsync(p => p.IdPedido == item.IdPedido);

        if (!pedidoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O pedido informado não existe."
            });
        }

        var produtoExiste = await _context.Produtos
            .AnyAsync(p => p.IdProduto == item.IdProduto);

        if (!produtoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O produto informado não existe."
            });
        }

        item.IdItem = 0;

        _context.ItensPedido.Add(item);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetItemPedido),
            new { id = item.IdItem },
            item
        );
    
   }


    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarItemPedido(int id, ItemPedido item)
    {
        if (id != item.IdItem)
        {
            return BadRequest();
        }

        var itemExistente = await _context.ItensPedido.FindAsync(id);

        if (itemExistente == null)
        {
            return NotFound();
        }

        var pedidoExiste = await _context.Pedidos
            .AnyAsync(p => p.IdPedido == item.IdPedido);

        if (!pedidoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O pedido informado não existe."
            });
        }

        var produtoExiste = await _context.Produtos
            .AnyAsync(p => p.IdProduto == item.IdProduto);

        if (!produtoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O produto informado não existe."
            });
        }

        itemExistente.IdPedido = item.IdPedido;
        itemExistente.IdProduto = item.IdProduto;
        itemExistente.Quantidade = item.Quantidade;
        itemExistente.PrecoUnitario = item.PrecoUnitario;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirItemPedido(int id)
    {
        var item = await _context.ItensPedido.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        _context.ItensPedido.Remove(item);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}