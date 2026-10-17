
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("pedidos")]
public class Pedido
{
    [Key]
    [Column("id_pedido")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPedido { get; set; }

    [Required]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Required]
    [Column("data_pedido")]
    public DateTime DataPedido { get; set; }

    [Required]
    [Column("status")]
    public string Status { get; set; } = string.Empty;
}