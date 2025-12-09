using System.Text;
using System.Xml.Linq;
using ForcaVendas.Api.Infra.Integration.Erp.Clientes;
using ForcaVendas.Api.Infra.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace ForcaVendas.Api.Infra.Integration.Erp.Clientes;

public class ClienteErpService : IClienteErpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClienteErpService> _logger;

    private readonly string _url;
    private readonly ErpSeniorDefaults _defaults;

    public ClienteErpService(
        HttpClient httpClient,
        IOptions<ErpSeniorConfig> options,
        ILogger<ClienteErpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var config = options.Value;

        _defaults = config.Defaults;
        _url = config.Clientes.WsUrl
            ?? throw new InvalidOperationException("ErpSenior:Clientes:WsUrl não configurado");
    }

    public async Task<IReadOnlyList<ClienteErpDto>> BuscarClientesAsync(DateTime? alteradosDesde)
    {
        // Por enquanto ignorando alteradosDesde, como você já fazia
        var soapEnvelope = MontarEnvelopeSoap(
            _defaults.CodEmp,
            _defaults.CodFil
        );

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Resposta bruta do WS de clientes recebida: {Xml}", xml);

            return ParseResposta(xml);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar clientes no ERP Senior (Exportar_7).");
            return Array.Empty<ClienteErpDto>();
        }
    }

    private string MontarEnvelopeSoap(string codEmp, string codFil)
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
        sb.AppendLine(@"  <soapenv:Header/>");
        sb.AppendLine(@"  <soapenv:Body>");
        sb.AppendLine(@"    <ser:Exportar_7>");
        sb.AppendLine($"      <user>{System.Security.SecurityElement.Escape(_defaults.Usuario)}</user>");
        sb.AppendLine($"      <password>{System.Security.SecurityElement.Escape(_defaults.Senha)}</password>");
        sb.AppendLine(@"      <encryption>0</encryption>");
        sb.AppendLine(@"      <parameters>");

        sb.AppendLine(@"        <codCli></codCli>");

        sb.AppendLine($"        <codEmp>{System.Security.SecurityElement.Escape(codEmp)}</codEmp>");
        sb.AppendLine($"        <codFil>{System.Security.SecurityElement.Escape(codFil)}</codFil>");
        sb.AppendLine(@"        <codRep></codRep>");
        sb.AppendLine($"        <conPen>{System.Security.SecurityElement.Escape(_defaults.ConPen)}</conPen>");
        sb.AppendLine(@"        <flowInstanceID></flowInstanceID>");
        sb.AppendLine(@"        <flowName></flowName>");
        sb.AppendLine($"        <identificadorSistema>{System.Security.SecurityElement.Escape(_defaults.IdentificadorSistema)}</identificadorSistema>");
        sb.AppendLine($"        <quantidadeRegistros>{System.Security.SecurityElement.Escape(_defaults.QuantidadeRegistros)}</quantidadeRegistros>");
        sb.AppendLine(@"        <tipEnd></tipEnd>");
        sb.AppendLine($"        <tipoIntegracao>{System.Security.SecurityElement.Escape(_defaults.TipoIntegracao)}</tipoIntegracao>");

        sb.AppendLine(@"      </parameters>");
        sb.AppendLine(@"    </ser:Exportar_7>");
        sb.AppendLine(@"  </soapenv:Body>");
        sb.AppendLine(@"</soapenv:Envelope>");

        return sb.ToString();
    }

    private IReadOnlyList<ClienteErpDto> ParseResposta(string xml)
    {
        var lista = new List<ClienteErpDto>();

        try
        {
            var doc = XDocument.Parse(xml);

            XNamespace ser = "http://services.senior.com.br";

            var resultNode = doc
                .Descendants(ser + "Exportar_7Response")
                .Descendants("result")
                .FirstOrDefault();

            if (resultNode is null)
            {
                _logger.LogWarning("Nó <result> não encontrado na resposta do Exportar_7.");
                return lista;
            }

            var clientesNodes = resultNode.Elements("cliente");

            foreach (var c in clientesNodes)
            {
                var codigoErp = c.Element("codCli")?.Value?.Trim() ?? "";
            
                var nome = c.Element("nomCli")?.Value?.Trim() ?? "";
                var documento = c.Element("cgcCpf")?.Value?.Trim() ?? "";
                var cidade = c.Element("cidCli")?.Value?.Trim();
                var uf = c.Element("sigUfs")?.Value?.Trim();

                if (string.IsNullOrWhiteSpace(codigoErp) ||
                    string.IsNullOrWhiteSpace(nome))
                {
                    continue;
                }

                var dto = new ClienteErpDto
                {
                    CodCli = Convert.ToInt32(codigoErp),
                    NomCli = nome,
                    NumCgc = documento,
                    CidCli = cidade,
                    SigUfs = uf,
                };

                lista.Add(dto);
            }

            if (lista.Count == 0)
            {
                _logger.LogWarning("Nenhum <cliente> válido encontrado em <result> na resposta do Exportar_7.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao interpretar XML de clientes do ERP Senior (Exportar_7).");
        }

        return lista;
    }
}
