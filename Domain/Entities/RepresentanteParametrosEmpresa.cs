// Domain/Entities/RepresentanteParametrosEmpresa.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace ForcaVendas.Api.Domain.Entities;

public class RepresentanteParametrosEmpresa
{
    public Guid Id { get; set; }

    public int CodEmp { get; set; }
    public int CodRep { get; set; }

    public string CodRve { get; set; } = null!;

    public decimal? PerCom { get; set; }
    public decimal? PerCos { get; set; }
    public decimal? ComFat { get; set; }
    public decimal? ComRec { get; set; }

    public decimal? PerIrf { get; set; }
    public decimal? PerIss { get; set; }
    public decimal? PerIns { get; set; }

    public string IpiCom { get; set; } = null!;
    public string IcmCom { get; set; } = null!;
    public string SubCom { get; set; } = null!;
    public string FreCom { get; set; } = null!;
    public string SegCom { get; set; } = null!;
    public string EmbCom { get; set; } = null!;
    public string EncCom { get; set; } = null!;
    public string OutCom { get; set; } = null!;
    public string DarCom { get; set; } = null!;

    public string? RecAdc { get; set; }
    public string? RecAoc { get; set; }
    public string? RecPcj { get; set; }
    public string? RecPcm { get; set; }
    public string? RecPcc { get; set; }
    public string? RecPce { get; set; }
    public string? RecPco { get; set; }

    public string ComPri { get; set; } = null!;

    public int? RepSup { get; set; }
    public string? CatRep { get; set; }
    public decimal? ComSup { get; set; }

    public string? CodBan { get; set; }
    public string? CodAge { get; set; }
    public string? CcbRep { get; set; }

    public int? TipCol { get; set; }
    public int? NumCad { get; set; }

    public decimal? VenVmp { get; set; }
    public decimal? RecVmt { get; set; }
    public decimal? PerCqt { get; set; }
    public decimal? PerCvl { get; set; }

    public int CriRat { get; set; }
    public int? CtaRed { get; set; }
    public int? CtaRcr { get; set; }
    public int? CtaFdv { get; set; }
    public int? CtaFcr { get; set; }

    public string ConEst { get; set; } = null!;

    public string? InsCom { get; set; }
    public string? IssCom { get; set; }
    public string? CofCom { get; set; }
    public string? PisCom { get; set; }
    public string? IrfCom { get; set; }
    public string? CslCom { get; set; }
    public string? OurCom { get; set; }
    public string? PifCom { get; set; }
    public string? CffCom { get; set; }

    public decimal? AvaVlr { get; set; }
    public decimal? AvaVlu { get; set; }
    public decimal? AvaVls { get; set; }
    public string? AvaAti { get; set; }
    public int? AvaMot { get; set; }
    public string? AvaObs { get; set; }

    public DateTime? AvdAlt { get; set; }
    public int? AvhAlt { get; set; }
    public decimal? AvuAlt { get; set; }
    public DateTime? AvdGer { get; set; }
    public int? AvhGer { get; set; }
    public decimal? AvuGer { get; set; }

    public string? RepAud { get; set; }

    public decimal? ComAss { get; set; }
    public decimal? PerFix { get; set; }

    public bool SitReg { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }

    [ForeignKey(nameof(CodRep))]
    public Representante Representante { get; set; } = null!;
}
