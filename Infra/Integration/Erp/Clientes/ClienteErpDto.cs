namespace ForcaVendas.Api.Infra.Integration.Erp.Clientes;

public class ClienteErpDto
{
    public int CodCli { get; set; } = default!;
    public string NomCli { get; set; } = default!;
    public string CgcCpf { get; set; } = default!;
    public string? CidCli { get; set; }
    public string? SigUfs { get; set; }

    public List<ClienteParametrosFilialErpDto> ParametrosPorFilial { get; set; } = new();
}
