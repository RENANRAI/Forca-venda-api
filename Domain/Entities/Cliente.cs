namespace ForcaVendas.Api.Domain.Entities;

public class Cliente
{
      
    public Guid Id { get; set; }
    //ligação com o ERP
    public int CodCli { get; set; }
    public string NomCli { get; set; } = default!;

    public string CgcCpf { get; set; } = default!;
    public string? CidCli { get; set; }
    public string? SigUfs { get; set; }
    public bool SitCli { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }

   
    public DateTime? DatSyn { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
