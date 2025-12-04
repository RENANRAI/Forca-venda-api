using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ForcaVendas.Api.Integration.Erp;

public class ClienteErpService : IClienteErpService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ClienteErpService> _logger;

    private readonly string _url;
    private readonly string _usuario;
    private readonly string _senha;
    private readonly string _codEmp;
    private readonly string _codFil;
    private readonly string _identificadorSistema;
    private readonly string _quantidadeRegistros;
    private readonly string _tipoIntegracao;
    private readonly string _conPen;

    public ClienteErpService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ClienteErpService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _url = _configuration["ErpSenior:ClienteWsUrl"]
            ?? throw new InvalidOperationException("ErpSenior:ClienteWsUrl não configurado");
        _usuario = _configuration["ErpSenior:Usuario"] ?? "";
        _senha = _configuration["ErpSenior:Senha"] ?? "";
        _codEmp = _configuration["ErpSenior:CodEmp"] ?? "1";
        _codFil = _configuration["ErpSenior:CodFil"] ?? "1";
        _identificadorSistema = _configuration["ErpSenior:IdentificadorSistema"] ?? "FORVEN";
        _quantidadeRegistros = _configuration["ErpSenior:QuantidadeRegistros"] ?? "100";
        _tipoIntegracao = _configuration["ErpSenior:TipoIntegracao"] ?? "A";
        _conPen = _configuration["ErpSenior:ConPen"] ?? "1";
    }

    public async Task<IReadOnlyList<ClienteErpDto>> BuscarClientesAsync(DateTime? alteradosDesde)
    {
        // Nesse primeiro momento vamos ignorar alteradosDesde,
        // pois o serviço Exportar_7 usa "tipoIntegracao" e outros filtros próprios.
        var soapEnvelope = MontarEnvelopeSoap();

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        // Se precisar de SOAPAction, descomenta e ajuste:
        // request.Headers.Add("SOAPAction", "Exportar_7");

        // Normalmente nos serviços antigos da Senior o user/senha vão no body mesmo,
        // então nem sempre precisa de Basic Auth. Vou deixar sem.
        // Se o seu ambiente exigir Basic Auth, descomenta:
        /*
        if (!string.IsNullOrWhiteSpace(_usuario))
        {
            var byteArray = Encoding.ASCII.GetBytes($"{_usuario}:{_senha}");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(byteArray));
        }
        */

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
            // Em caso de erro, devolve lista vazia para não quebrar o sync
            return Array.Empty<ClienteErpDto>();
        }
    }

    private string MontarEnvelopeSoap()
    {
        // Envelope baseado no exemplo que você enviou:
        //
        // <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.senior.com.br">
        //   <soapenv:Header/>
        //   <soapenv:Body>
        //     <ser:Exportar_7>
        //       <user>suporte</user>
        //       <password>suporte</password>
        //       <encryption></encryption>
        //       <parameters>
        //         <codCli>1</codCli>
        //         <codEmp>9999</codEmp>
        //         ...
        //       </parameters>
        //     </ser:Exportar_7>
        //   </soapenv:Body>
        // </soapenv:Envelope>

        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
        sb.AppendLine(@"  <soapenv:Header/>");
        sb.AppendLine(@"  <soapenv:Body>");
        sb.AppendLine(@"    <ser:Exportar_7>");
        sb.AppendLine($"      <user>{System.Security.SecurityElement.Escape(_usuario)}</user>");
        sb.AppendLine($"      <password>{System.Security.SecurityElement.Escape(_senha)}</password>");
        sb.AppendLine(@"      <encryption>0</encryption>");
        sb.AppendLine(@"      <parameters>");

        // Aqui você decide se quer filtrar por um cliente específico ou não.
        // No seu exemplo tinha <codCli>1</codCli>. Para "todos", normalmente deixa vazio.
        sb.AppendLine(@"        <codCli></codCli>");

        sb.AppendLine($"        <codEmp>{System.Security.SecurityElement.Escape(_codEmp)}</codEmp>");
        sb.AppendLine($"        <codFil>{System.Security.SecurityElement.Escape(_codFil)}</codFil>");
        sb.AppendLine(@"        <codRep></codRep>");
        sb.AppendLine($"        <conPen>{System.Security.SecurityElement.Escape(_conPen)}</conPen>");
        sb.AppendLine(@"        <flowInstanceID></flowInstanceID>");
        sb.AppendLine(@"        <flowName></flowName>");
        sb.AppendLine($"        <identificadorSistema>{System.Security.SecurityElement.Escape(_identificadorSistema)}</identificadorSistema>");
        sb.AppendLine($"        <quantidadeRegistros>{System.Security.SecurityElement.Escape(_quantidadeRegistros)}</quantidadeRegistros>");
        sb.AppendLine(@"        <tipEnd></tipEnd>");
        sb.AppendLine($"        <tipoIntegracao>{System.Security.SecurityElement.Escape(_tipoIntegracao)}</tipoIntegracao>");

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

            // Envelope:
            // <S:Envelope>
            //   <S:Body>
            //     <ns2:Exportar_7Response xmlns:ns2="http://services.senior.com.br">
            //       <result>
            //         <cliente> ... </cliente>
            //         <cliente> ... </cliente>
            //         ...
            //       </result>
            //     </ns2:Exportar_7Response>
            //   </S:Body>
            // </S:Envelope>

            XNamespace ser = "http://services.senior.com.br";

            // Pega o nó <result> dentro do Exportar_7Response
            var resultNode = doc
                .Descendants(ser + "Exportar_7Response")
                .Descendants("result")
                .FirstOrDefault();

            if (resultNode is null)
            {
                _logger.LogWarning("Nó <result> não encontrado na resposta do Exportar_7.");
                return lista;
            }

            // Cada <cliente> é um registro
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
                    continue; // pula registros zuados
                }

                var dto = new ClienteErpDto
                {
                    CodigoErp = codigoErp,
                    Nome = nome,
                    Documento = documento,
                    Cidade = cidade,
                    Uf = uf
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
