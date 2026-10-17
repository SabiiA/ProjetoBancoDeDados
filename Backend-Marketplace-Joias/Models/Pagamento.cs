
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace_Joias.Models;

[Table("pagamentos")]
public class Pagamento
{
    [Key]
    [Column("id_pagamento")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPagamento { get; set; }

    [Required]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Required]
    [Column("forma_pagamento")]
    public string FormaPagamento { get; set; } = string.Empty;

    [Required]
    [Column("valor")]
    public decimal Valor { get; set; }

    [Required]
    [Column("status_pagamento")]
    public string StatusPagamento { get; set; } = string.Empty;
}