
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{

    private readonly AppDbContext _context;
    private readonly Marketplace_Joias.Models.PasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;


    public ClientesController(
        AppDbContext context,
        Marketplace_Joias.Models.PasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetClientes()
    {
        var clientes = await _context.Clientes
            .Select(c => new
            {
                c.IdCliente,
                c.Nome,
                c.Email,
                c.Telefone
            })
            .ToListAsync();

        return Ok(clientes);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCliente(int id)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.IsInRole("Cliente") && idUsuario != id.ToString())
        {
            return Forbid();
        }

        var cliente = await _context.Clientes
            .Where(c => c.IdCliente == id)
            .Select(c => new
            {
                c.IdCliente,
                c.Nome,
                c.Email,
                c.Telefone
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }


    [HttpPost]
    public async Task<IActionResult> CriarCliente(CadastroClienteDto cadastro)
    {
        var emailAdmin = _configuration["Admin:Email"];

        if (cadastro.Email.Equals(emailAdmin, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensagem = "Este e-mail é reservado ao administrador."
            });
        }

        var emailExiste = await _context.Clientes
    .AnyAsync(c => c.Email == cadastro.Email);

        if (emailExiste)
        {
            return Conflict(new
            {
                mensagem = "Este e-mail já está cadastrado."
            });
        }

        var cpfExiste = await _context.Clientes
    .AnyAsync(c => c.Cpf == cadastro.Cpf);

        if (cpfExiste)
        {
            return Conflict(new
            {
                mensagem = "Este CPF já está cadastrado."
            });
        }

        var cliente = new Cliente
        {
            Nome = cadastro.Nome,
            Email = cadastro.Email,
            Cpf = cadastro.Cpf,
            Senha = _passwordHasher.GerarHash(cadastro.Senha),
            Telefone = cadastro.Telefone
        };

        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCliente),
            new { id = cliente.IdCliente },
            new
            {
                cliente.IdCliente,
                cliente.Nome,
                cliente.Email,
                cliente.Telefone
            }
        );
    }

    [Authorize] 
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarCliente(int id, AtualizarClienteDto cliente)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole("Cliente") || idUsuario != id.ToString())
        {
            return Forbid();
        }

        var clienteExistente = await _context.Clientes.FindAsync(id);

        if (clienteExistente == null)
        {
            return NotFound();
        }

        var emailExiste = await _context.Clientes
            .AnyAsync(c => c.Email == cliente.Email && c.IdCliente != id);

        if (emailExiste)
        {
            return Conflict(new
            {
                mensagem = "Este e-mail já está cadastrado."
            });
        }

        clienteExistente.Nome = cliente.Nome;
        clienteExistente.Email = cliente.Email;
        clienteExistente.Telefone = cliente.Telefone;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirCliente(int id)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole("Admin") &&
            (!User.IsInRole("Cliente") || idUsuario != id.ToString()))
        {
            return Forbid();
        }

        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        _context.Clientes.Remove(cliente);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}