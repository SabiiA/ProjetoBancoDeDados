
using System.ComponentModel.DataAnnotations;

namespace Marketplace_Joias.Models;

public class CadastroClienteDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(11, MinimumLength = 11,
        ErrorMessage = "O CPF deve ter exatamente 11 dígitos.")]
    [RegularExpression(@"^\d{11}$",
        ErrorMessage = "O CPF deve conter somente números.")]
    public string Cpf { get; set; } = string.Empty;
    [Required]
    [MinLength(8)]
    public string Senha { get; set; } = string.Empty;

    public string? Telefone { get; set; }
}