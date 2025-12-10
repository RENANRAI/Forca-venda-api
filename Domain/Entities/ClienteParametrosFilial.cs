// Domain/Entities/ClienteParametrosFilial.cs
namespace ForcaVendas.Api.Domain.Entities;

public class ClienteParametrosFilial
{
    public Guid Id { get; set; }

    public int CodCli { get; set; }
    public int CodEmp { get; set; }
    public int CodFil { get; set; }

    // Alguns campos principais (você pode ir completando depois com todos)
    public double? SalDup { get; set; }
    public double? SalOut { get; set; }
    public double? SalCre { get; set; }
    public DateTime? DatLim { get; set; }
    public double? VlrLim { get; set; }
    public string? LimApr { get; set; }
    public double? VlrPfa { get; set; }
    public DateTime? DatMac { get; set; }
    public double? VlrMac { get; set; }

    public int? CatCli { get; set; }
    public string? CodCpg { get; set; }
    public int? CodFpg { get; set; }
    public string? CodTpr { get; set; }
    public double? PerDsc { get; set; }

    public double? PerFre { get; set; }
    public double? PerIss { get; set; }
    public string? CifFob { get; set; }
    public string? CodTab { get; set; }

    // Se quiser guardar o JSON dos campos usuário:
   // public string? CamposUsuarioJson { get; set; }

    // Controle interno
    public bool SitReg { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }
}
