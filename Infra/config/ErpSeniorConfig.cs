using System.ComponentModel.DataAnnotations;

namespace ForcaVendas.Api.Infra.Config
{
    public class ErpSeniorDefaults
    {
        public string Usuario { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string CodEmp { get; set; } = default!;
        public string CodFil { get; set; } = default!;
        public string IdentificadorSistema { get; set; } = default!;
        public string QuantidadeRegistros { get; set; } = "100";
        public string TipoIntegracao { get; set; } = "T";
        public string ConPen { get; set; } = "1";


        // Novo campo para sincronização

        [Range(1, int.MaxValue, ErrorMessage = "O intervalo de sincronização deve ser no mínimo 1 minuto.")]
        public int SyncIntervalMinutes { get; set; }

    }

    public class ErpSeniorServiceConfig
    {
        public string WsUrl { get; set; } = default!;
    }



    public class ErpSeniorConfig
    {

        public ErpSeniorDefaults Defaults { get; set; } = new();
        public ErpSeniorServiceConfig Clientes { get; set; } = new();
        public ErpSeniorServiceConfig Empresas { get; set; } = new();
        public ErpSeniorServiceConfig Filial { get; set; } = new();

        public ErpSeniorServiceConfig Representantes { get; set; } = new();
        public ErpSeniorServiceConfig Integracao { get; set; } = new();


    }
}
