
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("endereco")]
public class Endereco
{
    [Key]
    [Column("id_endereco")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEndereco { get; set; }

    [Required]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Required]
    [Column("rua")]
    public string Rua { get; set; } = string.Empty;

    [Required]
    [Column("numero")]
    public string Numero { get; set; } = string.Empty;

    [Column("complemento")]
    public string? Complemento { get; set; }

    [Required]
    [Column("bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Required]
    [Column("cidade")]
    public string Cidade { get; set; } = string.Empty;


    [Required]
    [Column("estado")]
    public string Estado { get; set; } = string.Empty;

    [Required]
    [Column("cep")]
    public string Cep { get; set; } = string.Empty;
}
