using System.Text;
using System.Xml.Linq;
using ForcaVendas.Api.Infra.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForcaVendas.Api.Infra.Integration.Erp.Filiais;

public class FiliaisErpService : IFiliaisErpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FiliaisErpService> _logger;
    private readonly string _url;
    private readonly ErpSeniorDefaults _defaults;

    public FiliaisErpService(
        HttpClient httpClient,
        IOptions<ErpSeniorConfig> config,
        ILogger<FiliaisErpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _defaults = config.Value.Defaults;
        _url = config.Value.Filial.WsUrl; // << ADICIONAR NO appsettings.json
    }

    public async Task<IReadOnlyList<FilialErpDto>> BuscarFiliaisAsync()
    {
        var soap = MontarEnvelopeSoap();

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soap, Encoding.UTF8, "text/xml")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("XML de filiais recebido: {xml}", xml);

       // return Parse(xml);
        return ParseResposta(xml);
    }

    private string MontarEnvelopeSoap()
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
        sb.AppendLine(@"  <soapenv:Header/>");
        sb.AppendLine(@"  <soapenv:Body>");
        sb.AppendLine(@"    <ser:Exportar_2>");
        sb.AppendLine($"      <user>{_defaults.Usuario}</user>");
        sb.AppendLine($"      <password>{_defaults.Senha}</password>");
        sb.AppendLine(@"      <encryption>0</encryption>");
        sb.AppendLine(@"      <parameters>");
        sb.AppendLine($"        <CodEmp>{_defaults.CodEmp}</CodEmp>");
        sb.AppendLine(@"        <CodFilEsp></CodFilEsp>"); // todas
        sb.AppendLine(@"        <CodFil>1</CodFil>");       // 0 = geral (padrão Senior)
        sb.AppendLine($"        <IdentificadorSistema>{_defaults.IdentificadorSistema}</IdentificadorSistema>");
        sb.AppendLine($"        <QuantidadeRegistros>{_defaults.QuantidadeRegistros}</QuantidadeRegistros>");
        sb.AppendLine($"        <TipoIntegracao>{_defaults.TipoIntegracao}</TipoIntegracao>");
        sb.AppendLine(@"      </parameters>");
        sb.AppendLine(@"    </ser:Exportar_2>");
        sb.AppendLine(@"  </soapenv:Body>");
        sb.AppendLine(@"</soapenv:Envelope>");

        return sb.ToString();
    }


    private IReadOnlyList<FilialErpDto> ParseResposta(string xml)
    {
        var lista = new List<FilialErpDto>();

        try
        {
            var doc = XDocument.Parse(xml);

            // namespace do serviço
            XNamespace ser = "http://services.senior.com.br";

            // <ns2:Exportar_2Response xmlns:ns2="http://services.senior.com.br">
            //   <result>...</result>
            // </ns2:Exportar_2Response>
            var resultNode = doc
                .Descendants(ser + "Exportar_2Response")
                .Descendants("result")
                .FirstOrDefault();

            if (resultNode is null)
            {
                _logger.LogWarning("Nó <result> não encontrado na resposta do Exportar_2 (filiais).");
                return lista;
            }

            // Cada <filial> é um registro
            foreach (var f in resultNode.Elements("filial"))
            {
                var codEmpStr = f.Element("codEmp")?.Value?.Trim();
                var codFilStr = f.Element("codFil")?.Value?.Trim();

                if (!int.TryParse(codEmpStr, out var codEmp))
                    continue;

                if (!int.TryParse(codFilStr, out var codFil))
                    continue;

                // numCgc no XML vem tipo "80680093000181.0" -> vamos normalizar
                var rawCnpj = f.Element("numCgc")?.Value?.Trim();
                var cnpj = rawCnpj?.Replace(".0", "");

                var dto = new FilialErpDto
                {
                    CodEmp = codEmp,
                    CodFil = codFil,
                    NomFil = f.Element("nomFil")?.Value?.Trim() ?? string.Empty,
                    NumCgc = cnpj,
                    EndFil = f.Element("endFil")?.Value?.Trim(),
                    NenFil = f.Element("nenFil")?.Value?.Trim(),
                    BaiFil = f.Element("baiFil")?.Value?.Trim(),
                    CidFil = f.Element("cidFil")?.Value?.Trim(),
                    SigUfs = f.Element("sigUfs")?.Value?.Trim(),
                    CepFil = f.Element("cepFil")?.Value?.Trim()
                };

                lista.Add(dto);
            }

            if (lista.Count == 0)
            {
                var msg = resultNode.Element("mensagemRetorno")?.Value;
                var tipoRetorno = resultNode.Element("tipoRetorno")?.Value;
                _logger.LogWarning("Nenhuma filial retornada. tipoRetorno={Tipo}, mensagemRetorno={Msg}", tipoRetorno, msg);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao interpretar XML de filiais (Exportar_2).");
        }

        return lista;
    }

}
