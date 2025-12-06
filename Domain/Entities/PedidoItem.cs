namespace ForcaVendas.Api.Domain.Entities;

public class PedidoItem
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal PercDesconto { get; set; }
    public decimal ValorTotal { get; set; }
    public int Ordem { get; set; }

    public Pedido Pedido { get; set; } = default!;
    public Produto Produto { get; set; } = default!;
}
