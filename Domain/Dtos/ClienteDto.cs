namespace Forca_venda_api.Domain.Dtos;

public class ClienteDto
{
    public string Id { get; set; } = default!;
    public int CodCli { get; set; } = default!;
    public string NomCli { get; set; } = default!;
    public string CgcCpf { get; set; } = default!;
    public string? CidCli { get; set; }
    public string? SigUfs { get; set; }
}
