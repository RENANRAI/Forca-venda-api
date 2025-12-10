using System.Text;
using System.Xml.Linq;
using ForcaVendas.Api.Infra.Integration.Erp.Clientes;
using ForcaVendas.Api.Infra.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;
using ForcaVendas.Api.Domain.Entities;
using System.Globalization;

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

    public async Task<IReadOnlyList<ClienteErpDto>> BuscarClientesAsync(int codEmp, int codFil)
    {
        var soapEnvelope = MontarEnvelopeSoap(codEmp, codFil);

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Resposta WS Clientes para Emp={CodEmp} Fil={CodFil}: {Xml}", codEmp, codFil, xml);

            return ParseResposta(xml,codEmp,codFil);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar clientes para Emp={CodEmp} Fil={CodFil}", codEmp, codFil);
            return Array.Empty<ClienteErpDto>();
        }
    }


    private string MontarEnvelopeSoap(int codEmp, int codFil)
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
        sb.AppendLine(@"  <soapenv:Header/>");
        sb.AppendLine(@"  <soapenv:Body>");
        sb.AppendLine(@"    <ser:Exportar_7>");
        sb.AppendLine($"      <user>{_defaults.Usuario}</user>");
        sb.AppendLine($"      <password>{_defaults.Senha}</password>");
        sb.AppendLine(@"      <encryption>0</encryption>");
        sb.AppendLine(@"      <parameters>");
        sb.AppendLine(@"        <codCli></codCli>");
        sb.AppendLine($"        <codEmp>{codEmp}</codEmp>");
        sb.AppendLine($"        <codFil>{codFil}</codFil>");
        sb.AppendLine(@"        <codRep></codRep>");
        sb.AppendLine($"        <conPen>{_defaults.ConPen}</conPen>");
        sb.AppendLine(@"        <flowInstanceID></flowInstanceID>");
        sb.AppendLine(@"        <flowName></flowName>");
        sb.AppendLine($"        <identificadorSistema>{_defaults.IdentificadorSistema}</identificadorSistema>");
        sb.AppendLine($"        <quantidadeRegistros>{_defaults.QuantidadeRegistros}</quantidadeRegistros>");
        sb.AppendLine(@"        <tipEnd></tipEnd>");
        sb.AppendLine($"        <tipoIntegracao>{_defaults.TipoIntegracao}</tipoIntegracao>");
        sb.AppendLine(@"      </parameters>");
        sb.AppendLine(@"    </ser:Exportar_7>");
        sb.AppendLine(@"  </soapenv:Body>");
        sb.AppendLine(@"</soapenv:Envelope>");

        return sb.ToString();
    }


    private IReadOnlyList<ClienteErpDto> ParseResposta(string xml, int codEmp, int codFil)
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

            // Nó(s) <cliente> dentro de <result>
            var clientesNodes = resultNode.Elements("cliente");

            // Helpers de conversão
            double? ToDouble(string? s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return null;

                // XML vem com "0.0"
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                    return v;

                if (double.TryParse(s, out v))
                    return v;

                return null;
            }

            DateTime? ToDate(string? s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return null;

                // Padrão Senior: dd/MM/yyyy
                if (DateTime.TryParseExact(
                        s,
                        "dd/MM/yyyy",
                        CultureInfo.GetCultureInfo("pt-BR"),
                        DateTimeStyles.None,
                        out var d))
                    return d;

                if (DateTime.TryParse(s, out d))
                    return d;

                return null;
            }

            foreach (var c in clientesNodes)
            {
                // codCli vem do cliente
                var codCliStr = c.Element("codCli")?.Value?.Trim();
                if (!int.TryParse(codCliStr, out var codCli))
                    continue;

                var nome = c.Element("nomCli")?.Value?.Trim() ?? "";
                var documento = c.Element("cgcCpf")?.Value?.Trim() ?? "";
                var cidade = c.Element("cidCli")?.Value?.Trim();
                var uf = c.Element("sigUfs")?.Value?.Trim();

                var dto = new ClienteErpDto
                {
                    CodCli = codCli,
                    NomCli = nome,
                    CgcCpf = documento,
                    CidCli = cidade,
                    SigUfs = uf
                };

                lista.Add(dto);

                // <historico> é filho de <cliente>
                var historicosNodes = c.Elements("historico");

                foreach (var h in historicosNodes)
                {
                    var param = new ClienteParametrosFilialErpDto
                    {
                        // 👉 sempre vindo do contexto da chamada
                        CodCli = codCli,
                        CodEmp = codEmp,
                        CodFil = codFil,

                        SalDup = ToDouble(h.Element("salDup")?.Value),
                        SalOut = ToDouble(h.Element("salOut")?.Value),
                        SalCre = ToDouble(h.Element("salCre")?.Value),
                        DatLim = ToDate(h.Element("datLim")?.Value),
                        VlrLim = ToDouble(h.Element("vlrLim")?.Value),
                        LimApr = h.Element("limApr")?.Value?.Trim(),
                        VlrPfa = ToDouble(h.Element("vlrPfa")?.Value),
                        DatMac = ToDate(h.Element("datMac")?.Value),
                        VlrMac = ToDouble(h.Element("vlrMac")?.Value),

                        CatCli = int.TryParse(h.Element("catCli")?.Value, out var catCli) ? catCli : null,
                        CodCpg = h.Element("codCpg")?.Value?.Trim(),
                        CodFpg = int.TryParse(h.Element("codFpg")?.Value, out var codFpg) ? codFpg : null,
                        CodTpr = h.Element("codTpr")?.Value?.Trim(),
                        PerDsc = ToDouble(h.Element("perDsc")?.Value),

                        PerFre = ToDouble(h.Element("perFre")?.Value),
                        PerIss = ToDouble(h.Element("perIss")?.Value),
                        CifFob = h.Element("cifFob")?.Value?.Trim(),
                        CodTab = h.Element("codTab")?.Value?.Trim()
                    };

                    dto.ParametrosPorFilial.Add(param);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao interpretar XML de clientes (Exportar_7) com histórico.");
        }

        return lista;
    }


}
