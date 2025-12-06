using System.Text;
using System.Xml.Linq;
using ForcaVendas.Api.Infra.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForcaVendas.Api.Infra.Integration.Erp.EmpresasFiliais;

public class EmpresasFiliaisErpService : IEmpresasFiliaisErpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmpresasFiliaisErpService> _logger;

    private readonly string _url;
    private readonly ErpSeniorDefaults _defaults;

    public EmpresasFiliaisErpService(
        HttpClient httpClient,
        IOptions<ErpSeniorConfig> options,
        ILogger<EmpresasFiliaisErpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var config = options.Value;
        _defaults = config.Defaults;

        _url = config.Integracao.WsUrl
            ?? throw new InvalidOperationException("ErpSenior:Empresas:WsUrl não configurado");
    }

    public async Task<IReadOnlyList<EmpresasFiliaisErpDto>> BuscarEmpresasFiliaisIntegradasAsync()
    {
        var soap = MontarEnvelopeSoap();

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soap, Encoding.UTF8, "text/xml")
        };

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("XML recebido do WS EmpresaFiliaisIntegradas: {Xml}", xml);

            return ParseResposta(xml);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar EmpresaFiliaisIntegradas.");
            return Array.Empty<EmpresasFiliaisErpDto>();
        }
    }

    private string MontarEnvelopeSoap()
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
        sb.AppendLine(@"  <soapenv:Header/>");
        sb.AppendLine(@"  <soapenv:Body>");
        sb.AppendLine(@"    <ser:EmpresaFiliaisIntegradas>");
        sb.AppendLine($"      <user>{_defaults.Usuario}</user>");
        sb.AppendLine($"      <password>{_defaults.Senha}</password>");
        sb.AppendLine(@"      <encryption>0</encryption>");
        sb.AppendLine(@"      <parameters>");
        sb.AppendLine(@"        <flowInstanceID></flowInstanceID>");
        sb.AppendLine(@"        <flowName></flowName>");
        sb.AppendLine(@"      </parameters>");
        sb.AppendLine(@"    </ser:EmpresaFiliaisIntegradas>");
        sb.AppendLine(@"  </soapenv:Body>");
        sb.AppendLine(@"</soapenv:Envelope>");

        return sb.ToString();
    }

    private IReadOnlyList<EmpresasFiliaisErpDto> ParseResposta(string xml)
    {
        var lista = new List<EmpresasFiliaisErpDto>();

        try
        {
            var doc = XDocument.Parse(xml);

            XNamespace ser = "http://services.senior.com.br";

            var resultNode = doc
                .Descendants(ser + "EmpresaFiliaisIntegradasResponse")
                .Descendants("result")
                .FirstOrDefault();

            if (resultNode == null)
            {
                _logger.LogWarning("Nó <result> não encontrado.");
                return lista;
            }

            foreach (var e in resultNode.Elements("filiais"))
            {
                var codEmpStr = e.Element("codEmp")?.Value?.Trim();
                var codFilStr = e.Element("codFil")?.Value?.Trim();

                // Conversão segura: se não conseguir converter, ignora o registro
                if (!int.TryParse(codEmpStr, out var codEmp))
                    continue;

                if (!int.TryParse(codFilStr, out var codFil))
                    continue;

                lista.Add(new EmpresasFiliaisErpDto
                {
                    CodEmp = codEmp,
                    CodFil = codFil,
                    NomEmp = e.Element("nomemp")?.Value,
                    NomFil = e.Element("nomfil")?.Value
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao interpretar XML de empresas/filiais integradas.");
        }

        return lista;
    }
}
