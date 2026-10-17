using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("produtos")]
public class Produto
{
    [Key]
    [Column("id_produto")]
    public int IdProduto { get; set; }

    [Required]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Column("preco", TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Required]
    [Column("estoque")]
    public int Estoque { get; set; }

    [Required]
    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Required]
    [Column("material")]
    public string Material { get; set; } = string.Empty;

    [Required]
    [Column("tamanho")]
    public string Tamanho { get; set; } = string.Empty;

    [Required]
    [Column("imagem")]
    public string Imagem { get; set; } = string.Empty;
}