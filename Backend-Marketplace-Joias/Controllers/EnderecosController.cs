
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnderecosController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnderecosController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Cliente")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
    {
        if (User.IsInRole("Admin"))
        {
            return await _context.Enderecos.ToListAsync();
        }

        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Unauthorized();
        }

        return await _context.Enderecos
            .Where(e => e.IdCliente == idCliente)
            .ToListAsync();
    }


    [Authorize(Roles = "Admin,Cliente")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Endereco>> GetEndereco(int id)
    {
        var endereco = await _context.Enderecos.FindAsync(id);

        if (endereco == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Admin"))
        {
            return endereco;
        }

        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Unauthorized();
        }

        if (endereco.IdCliente != idCliente)
        {
            return Forbid();
        }

        return endereco;
    }


    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<ActionResult<Endereco>> CriarEndereco(Endereco endereco)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Unauthorized();
        }

        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == idCliente);

        if (!clienteExiste)
        {
            return Unauthorized();
        }

        endereco.IdEndereco = 0;
        endereco.IdCliente = idCliente;

        _context.Enderecos.Add(endereco);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEndereco),
            new { id = endereco.IdEndereco },
            endereco
        );
    }


    [Authorize(Roles = "Cliente")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarEndereco(int id, Endereco endereco)
    {
        if (id != endereco.IdEndereco)
        {
            return BadRequest();
        }

        var enderecoExistente = await _context.Enderecos.FindAsync(id);

        if (enderecoExistente == null)
        {
            return NotFound();
        }

        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Unauthorized();
        }

        if (enderecoExistente.IdCliente != idCliente)
        {
            return Forbid();
        }

        enderecoExistente.Rua = endereco.Rua;
        enderecoExistente.Numero = endereco.Numero;
        enderecoExistente.Complemento = endereco.Complemento;
        enderecoExistente.Bairro = endereco.Bairro;
        enderecoExistente.Cidade = endereco.Cidade;
        enderecoExistente.Estado = endereco.Estado;
        enderecoExistente.Cep = endereco.Cep;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Cliente")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirEndereco(int id)
    {
        var endereco = await _context.Enderecos.FindAsync(id);

        if (endereco == null)
        {
            return NotFound();
        }

        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out var idCliente))
        {
            return Unauthorized();
        }

        if (endereco.IdCliente != idCliente)
        {
            return Forbid();
        }

        _context.Enderecos.Remove(endereco);

        await _context.SaveChangesAsync();

        return NoContent();
    }

}

