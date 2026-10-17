using Microsoft.EntityFrameworkCore;
using Marketplace_Joias.Models;

namespace Marketplace_Joias.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Pedido> Pedidos { get; set; }

    public DbSet<ItemPedido> ItensPedido { get; set; }

    public DbSet<Pagamento> Pagamentos { get; set; }

    public DbSet<Avaliacao> Avaliacoes { get; set; }

    public DbSet<Endereco> Enderecos { get; set; }


}