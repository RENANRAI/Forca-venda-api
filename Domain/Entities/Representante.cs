namespace ForcaVendas.Api.Domain.Entities;

public class Representante
{
    public int CodRep { get; set; }   // PK

    public string? ApeRep { get; set; }
    public string NomRep { get; set; } = null!;
    public string? BaiRep { get; set; }
    public string? CidRep { get; set; }
    public string? SigUfs { get; set; }
    public string? EndRep { get; set; }
    public string? CepRep { get; set; }
    public string? CgcCpf { get; set; }
    public string? InsEst { get; set; }
    public string? InsMun { get; set; }
    public string? FonRep { get; set; }
    public string? FonRe2 { get; set; }
    public string? FonRe3 { get; set; }
    public string? FaxRep { get; set; }
    public string? IntNet { get; set; }
    public string? TipRep { get; set; }
    public string? SitRep { get; set; }
    public string? SitWmw { get; set; }

    public string? CalIns { get; set; }
    public string? CalIrf { get; set; }
    public string? CalIss { get; set; }
    public string? GerTit { get; set; }
    public int? IndExp { get; set; }
    public int? CxaPst { get; set; }
    public int? CodCdi { get; set; }
    public int? CodMot { get; set; }

    public DateTime? DatCad { get; set; }
    public DateTime? DatAtu { get; set; }
    public DateTime? DatMot { get; set; }
    public DateTime? DatNas { get; set; }
    public DateTime? DatPal { get; set; }
    public DateTime? DatRge { get; set; }

    public int? HorCad { get; set; }
    public int? HorAtu { get; set; }
    public int? HorMot { get; set; }
    public int? HorPal { get; set; }

    public string? NumRge { get; set; }
    public string? OrgRge { get; set; }
    public string? EenRep { get; set; }
    public string? CplEnd { get; set; }
    public string? NenRep { get; set; }
    public int? QtdDep { get; set; }
    public int? RepCli { get; set; }
    public int? RepFor { get; set; }
    public string? SenRep { get; set; }
    public int? SeqInt { get; set; }
    public string? ZipCod { get; set; }

    // Controle interno
    public bool SitReg { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtuApp { get; set; }

    // Navegação
    public ICollection<RepresentanteParametrosEmpresa> ParametrosPorEmpresa { get; set; }
        = new List<RepresentanteParametrosEmpresa>();
}
