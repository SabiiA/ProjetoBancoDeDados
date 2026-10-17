
using Marketplace_Joias.Data;
using Marketplace_Joias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marketplace_Joias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthController(
        AppDbContext context,
        PasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dados)
    {

        var adminEmail = _configuration["Admin:Email"];
        var adminPassword = _configuration["Admin:Password"];
        
        if (adminEmail != null &&
            adminPassword != null &&
            dados.Email != null &&
            dados.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) &&
            dados.Senha == adminPassword)
        {
            var adminClaims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "admin"),
        new Claim(ClaimTypes.Name, "Administrador"),
        new Claim(ClaimTypes.Email, adminEmail),
        new Claim(ClaimTypes.Role, "Admin")
    };

            var adminKey = _configuration["Jwt:Key"]!;

            var adminSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(adminKey));

            var adminCredentials = new SigningCredentials(
                adminSigningKey, SecurityAlgorithms.HmacSha256);

            var adminToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: adminClaims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: adminCredentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler()
                    .WriteToken(adminToken),
                nome = "Administrador",
                perfil = "Admin"
            });
        }


        var cliente = await _context.Clientes
      .FirstOrDefaultAsync(c => c.Email == dados.Email);

        if (cliente == null ||
            !_passwordHasher.VerificarSenha(
                cliente.Senha, dados.Senha))
        {
            return Unauthorized(new
            {
                mensagem = "E-mail ou senha inválidos."
            });
        }

        var jwtKey = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                cliente.IdCliente.ToString()),

            new Claim(ClaimTypes.Name, cliente.Nome),
            new Claim(ClaimTypes.Email, cliente.Email),
            new Claim(ClaimTypes.Role, "Cliente")
        };

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credenciais = new SigningCredentials(
            chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler()
                .WriteToken(token),
            nome = cliente.Nome,
            idCliente = cliente.IdCliente
        });
    }
}

public class LoginDto
{
    public string? Email { get; set; }
  //  public string? Cpf { get; set; }
    public string Senha { get; set; } = string.Empty;
}