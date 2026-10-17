
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        return await _context.Pedidos.ToListAsync();
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetPedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole("Admin") &&
            idUsuario != pedido.IdCliente.ToString())
        {
            return Forbid();
        }

        return pedido;
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<ActionResult<Pedido>> CriarPedido(CriarPedidoRequest request)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Forbid();
        }

        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == idCliente);

        if (!clienteExiste)
        {
            return BadRequest(new
            {
                mensagem = "O cliente informado não existe."
            });
        }

        if (request.Itens == null || request.Itens.Count == 0)
        {
            return BadRequest(new
            {
                mensagem = "O pedido precisa ter pelo menos um produto."
            });
        }

        if (request.Itens.Any(i => i.IdProduto <= 0 || i.Quantidade <= 0))
        {
            return BadRequest(new
            {
                mensagem = "Os produtos e as quantidades devem ser válidos."
            });
        }

        var itensAgrupados = request.Itens
            .GroupBy(i => i.IdProduto)
            .Select(g => new
            {
                IdProduto = g.Key,
                Quantidade = g.Sum(i => i.Quantidade)
            })
            .ToList();

        var idsProdutos = itensAgrupados.Select(i => i.IdProduto).ToList();

        var produtos = await _context.Produtos
            .Where(p => idsProdutos.Contains(p.IdProduto))
            .ToListAsync();

        if (produtos.Count != idsProdutos.Count)
        {
            return BadRequest(new
            {
                mensagem = "Um ou mais produtos informados não existem."
            });
        }

        foreach (var item in itensAgrupados)
        {
            var produto = produtos.First(p => p.IdProduto == item.IdProduto);

            if (produto.Estoque < item.Quantidade)
            {
                return BadRequest(new
                {
                    mensagem = $"Estoque insuficiente para o produto {produto.Nome}."
                });
            }
        }

        var pedido = new Pedido
        {
            IdCliente = idCliente,
            DataPedido = DateTime.UtcNow,
            Status = "Pendente"
        };


        await using var transacao = await _context.Database.BeginTransactionAsync();

        _context.Pedidos.Add(pedido);

        await _context.SaveChangesAsync();

        foreach (var item in itensAgrupados)
        {
            var produto = produtos.First(p => p.IdProduto == item.IdProduto);

            _context.ItensPedido.Add(new ItemPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });

            produto.Estoque -= item.Quantidade;
        }

        await _context.SaveChangesAsync();

        await transacao.CommitAsync();


        return CreatedAtAction(
            nameof(GetPedido),
            new { id = pedido.IdPedido },
            pedido
        );
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPedido(int id, Pedido pedido)
    {
        if (id != pedido.IdPedido)
        {
            return BadRequest();
        }

        var pedidoExistente = await _context.Pedidos.FindAsync(id);

        if (pedidoExistente == null)
        {
            return NotFound();
        }

        pedidoExistente.Status = pedido.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [Authorize(Roles = "Cliente")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPedido(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        if (pedido == null)
        {
            return NotFound();
        }

        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (idUsuario != pedido.IdCliente.ToString())
        {
            return Forbid();
        }

        if (!string.Equals(pedido.Status, "Pendente", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensagem = "Somente pedidos pendentes podem ser cancelados."
            });
        }

        var itens = await _context.ItensPedido
            .Where(i => i.IdPedido == pedido.IdPedido)
            .ToListAsync();

        await using var transacao = await _context.Database.BeginTransactionAsync();

        foreach (var item in itens)
        {
            var produto = await _context.Produtos
                .FindAsync(item.IdProduto);

            if (produto != null)
            {
                produto.Estoque += item.Quantidade;
            }
        }

        pedido.Status = "Cancelado";

        await _context.SaveChangesAsync();

        await transacao.CommitAsync();

        return NoContent();
    }

}