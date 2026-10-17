
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagamentosController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pagamento>>> GetPagamentos()
    {
        return await _context.Pagamentos.ToListAsync();
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Pagamento>> GetPagamento(int id)
    {
        var pagamento = await _context.Pagamentos.FindAsync(id);

        if (pagamento == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .FindAsync(pagamento.IdPedido);

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

        return pagamento;
    }




    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<ActionResult<Pagamento>> CriarPagamento(Pagamento pagamento)
    {
        var pedido = await _context.Pedidos
            .FindAsync(pagamento.IdPedido);

        if (pedido == null)
        {
            return BadRequest(new
            {
                mensagem = "O pedido informado não existe."
            });
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
                mensagem = "Somente pedidos pendentes podem receber pagamento."
            });
        }

        var pagamentoExiste = await _context.Pagamentos
            .AnyAsync(p => p.IdPedido == pedido.IdPedido);

        if (pagamentoExiste)
        {
            return BadRequest(new
            {
                mensagem = "Já existe um pagamento registrado para este pedido."
            });
        }
        var itens = await _context.ItensPedido
            .Where(i => i.IdPedido == pedido.IdPedido)
            .ToListAsync();

        if (itens.Count == 0)
        {
            return BadRequest(new
            {
                mensagem = "O pedido não possui itens."
            });
        }

        var valorTotal = itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        var formasPermitidas = new[] { "Pix", "Boleto", "Cartão de crédito", "Cartão de débito" };

        if (!formasPermitidas.Contains(pagamento.FormaPagamento, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensagem = "A forma de pagamento informada não é válida."
            });
        }


        pagamento.IdPagamento = 0;
        pagamento.Valor = valorTotal;
        pagamento.StatusPagamento = "Aprovado";

        await using var transacao = await _context.Database.BeginTransactionAsync();

        pedido.Status = "Pago";

        _context.Pagamentos.Add(pagamento);

        await _context.SaveChangesAsync();

        await transacao.CommitAsync();

        return CreatedAtAction(
            nameof(GetPagamento),
            new { id = pagamento.IdPagamento },
            pagamento
        );
    }
        [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPagamento(int id, Pagamento pagamento)
    {
        if (id != pagamento.IdPagamento)
        {
            return BadRequest();
        }

        var pagamentoExistente = await _context.Pagamentos.FindAsync(id);

        if (pagamentoExistente == null)
        {
            return NotFound();
        }

        var pedidoExiste = await _context.Pedidos
            .AnyAsync(p => p.IdPedido == pagamento.IdPedido);

        if (!pedidoExiste)
        {
            return BadRequest(new
            {
                mensagem = "O pedido informado não existe."
            });
        }

        pagamentoExistente.IdPedido = pagamento.IdPedido;
        pagamentoExistente.FormaPagamento = pagamento.FormaPagamento;
        pagamentoExistente.Valor = pagamento.Valor;
        pagamentoExistente.StatusPagamento = pagamento.StatusPagamento;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPagamento(int id)
    {
        var pagamento = await _context.Pagamentos.FindAsync(id);

        if (pagamento == null)
        {
            return NotFound();
        }

        _context.Pagamentos.Remove(pagamento);

        await _context.SaveChangesAsync();

        return NoContent();
    }

}
