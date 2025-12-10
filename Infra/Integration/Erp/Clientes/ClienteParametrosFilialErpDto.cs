// Domain/Entities/ClienteParametrosFilialErpDto.cs (ou onde você estiver guardando os DTOs do ERP)
namespace ForcaVendas.Api.Infra.Integration.Erp.Clientes;


public class ClienteParametrosFilialErpDto
{
    // Chave
    public int CodCli { get; set; }      // do XML <codCli> (cliente)
    public short CodEmp { get; set; }    // vem do contexto (empresa da chamada)
    public int CodFil { get; set; }      // vem do contexto (filial da chamada)

    // Saldos / limites
    public decimal? SalDup { get; set; }
    public decimal? SalOut { get; set; }
    public decimal? SalCre { get; set; }
    public DateTime? DatLim { get; set; }
    public decimal? VlrLim { get; set; }
    public string? LimApr { get; set; }
    public decimal? VlrPfa { get; set; }
    public DateTime? DatMac { get; set; }
    public decimal? VlrMac { get; set; }

    // Classificação / condições
    public short? CatCli { get; set; }
    public string? CodCpg { get; set; }
    public short? CodFpg { get; set; }
    public string? CodTpr { get; set; }

    // Percentuais
    public decimal? PerDsc { get; set; }
    public decimal? PerFre { get; set; }
    public decimal? PerIss { get; set; }

    // CIF/FOB / Tabela
    public string? CifFob { get; set; }
    public string? CodTab { get; set; }

    // Se você quiser, pode trazer também o JSON dos campos usuário: //public Dictionary<string, string>? CamposUsuario { get; set; }
}
