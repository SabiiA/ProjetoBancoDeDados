
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("itens_pedido")]
public class ItemPedido
{
    [Key]
    [Column("id_item")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdItem { get; set; }

    [Required]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Required]
    [Column("id_produto")]
    public int IdProduto { get; set; }

    [Required]
    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Required]
    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }
}