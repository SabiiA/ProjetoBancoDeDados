
namespace Marketplace_Joias.Models;

public class CriarPedidoRequest
{
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}

public class ItemPedidoRequest
{
    public int IdProduto { get; set; }

    public int Quantidade { get; set; }
}
