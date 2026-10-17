
using System.ComponentModel.DataAnnotations;

namespace Marketplace_Joias.Models;

public class AtualizarClienteDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Telefone { get; set; }
}