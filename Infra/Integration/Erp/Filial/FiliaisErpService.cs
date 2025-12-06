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
        var soap = MontarEnvelope();

        var request = new HttpRequestMessage(HttpMethod.Post, _url)
        {
            Content = new StringContent(soap, Encoding.UTF8, "text/xml")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("XML de filiais recebido: {xml}", xml);

        return Parse(xml);
    }

    private string MontarEnvelope()
    {
        return
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">
  <soapenv:Header/>
  <soapenv:Body>
    <ser:ExportarFiliais>
       <user>{_defaults.Usuario}</user>
       <password>{_defaults.Senha}</password>
       <encryption>0</encryption>
       <parameters>
         <flowInstanceID></flowInstanceID>
         <flowName></flowName>
       </parameters>
    </ser:ExportarFiliais>
  </soapenv:Body>
</soapenv:Envelope>";
    }

    private IReadOnlyList<FilialErpDto> Parse(string xml)
    {
        var lista = new List<FilialErpDto>();

        var doc = XDocument.Parse(xml);
        XNamespace ser = "http://services.senior.com.br";

        var root = doc.Descendants(ser + "ExportarFiliaisResponse")
                      .Descendants("result")
                      .FirstOrDefault();

        if (root == null)
            return lista;

        foreach (var f in root.Elements("filial"))
        {
            lista.Add(new FilialErpDto
            {
                CodEmp = int.Parse(f.Element("codEmp")!.Value),
                CodFil = int.Parse(f.Element("codFil")!.Value),
                Nome = f.Element("nomFil")?.Value ?? "",
                Cnpj = f.Element("cgcFil")?.Value,
                Endereco = f.Element("endFil")?.Value,
                Numero = f.Element("numEnd")?.Value,
                Bairro = f.Element("baiFil")?.Value,
                Cidade = f.Element("cidFil")?.Value,
                Uf = f.Element("sigUfs")?.Value,
                Cep = f.Element("cepFil")?.Value
            });
        }

        return lista;
    }
}
