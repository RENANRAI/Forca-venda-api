namespace ForcaVendas.Api.Domain.Entities;

public class ClienteParametrosFilial
{
    public Guid Id { get; set; }

    public int CodCli { get; set; }      // codcli int
    public short CodEmp { get; set; }    // codemp smallint
    public int CodFil { get; set; }      // codfil int

    // Campos principais (numeric -> decimal)
    public decimal? SalDup { get; set; } // saldup numeric(15,2)
    public decimal? SalOut { get; set; } // salout numeric(15,2)
    public decimal? SalCre { get; set; } // salcre numeric(15,2)
    public DateTime? DatLim { get; set; } // datlim datetime
    public decimal? VlrLim { get; set; } // vlrlim numeric(15,2)
    public string? LimApr { get; set; }  // limapr varchar(1)
    public decimal? VlrPfa { get; set; } // vlrpfa numeric(15,2)
    public DateTime? DatMac { get; set; } // datmac datetime
    public decimal? VlrMac { get; set; } // vlrmac numeric(15,2)

    public short? CatCli { get; set; }   // catcli smallint
    public string? CodCpg { get; set; }  // codcpg varchar(6)
    public short? CodFpg { get; set; }   // codfpg smallint
    public string? CodTpr { get; set; }  // codtpr varchar(4)

    public decimal? PerDsc { get; set; } // perdsc numeric(4,2)
    public decimal? PerFre { get; set; } // perfre numeric(5,2)
    public decimal? PerIss { get; set; } // periss numeric(6,4)
    public string? CifFob { get; set; }  // ciffob varchar(1)
    public string? CodTab { get; set; }  // codtab varchar(4)

    // Controle interno
    public bool SitReg { get; set; } = true;
    public DateTime DatCri { get; set; }
    public DateTime? DatAtu { get; set; }

    // Se quiser guardar o JSON dos campos usuário:
    // public string? CamposUsuarioJson { get; set; }
}
