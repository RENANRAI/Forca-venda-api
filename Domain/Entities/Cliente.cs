namespace ForcaVendas.Api.Domain.Entities;

public class Cliente
{
      
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Documento { get; set; } = default!;
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    //ligação com o ERP
    public string? CodigoErp { get; set; }
    public DateTime? DataUltimaSincronizacao { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
