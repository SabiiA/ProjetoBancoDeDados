
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("avaliacoes")]
public class Avaliacao
{
    [Key]
    [Column("id_avaliacao")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdAvaliacao { get; set; }

    [Required]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Required]
    [Column("id_produto")]
    public int IdProduto { get; set; }

    [Column("nota")]
    public int? Nota { get; set; }

    [Column("comentario")]
    public string? Comentario { get; set; }

    [Required]
    [Column("data_avaliacao")]
    public DateTime DataAvaliacao { get; set; }
}