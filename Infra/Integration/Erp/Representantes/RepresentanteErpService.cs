using System.Text;
using System.Xml.Linq;
using ForcaVendas.Api.Infra.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ForcaVendas.Api.Infra.Integration.Erp.Representantes
{
    public class RepresentanteErpService : IRepresentanteErpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RepresentanteErpService> _logger;
        private readonly string _url;
        private readonly ErpSeniorDefaults _defaults;

        public RepresentanteErpService(
            HttpClient httpClient,
            IOptions<ErpSeniorConfig> options,
            ILogger<RepresentanteErpService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var config = options.Value;
            _defaults = config.Defaults;
            _url = config.Representantes.WsUrl
                ?? throw new InvalidOperationException("ErpSenior:Representantes:WsUrl não configurado");
        }

        public async Task<IReadOnlyList<RepresentanteErpDto>> BuscarRepresentantesEmpresaAsync(
            int codEmp,
            int CodFil,
            CancellationToken cancellationToken,
            int? codRep = null)
        {
            var soapEnvelope = MontarEnvelopeSoap(codEmp, CodFil, codRep);

            var request = new HttpRequestMessage(HttpMethod.Post, _url)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var xml = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Resposta WS Representantes Emp={CodEmp}: {Xml}", codEmp, xml);

                return ParseResposta(xml, codEmp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar representantes Emp={CodEmp}", codEmp);
                return Array.Empty<RepresentanteErpDto>();
            }
        }

        // Se quiser implementar BuscarRepresentantesAsync(global), pode ser algo simples
        public async Task<IReadOnlyList<RepresentanteErpDto>> BuscarRepresentantesAsync(
            CancellationToken cancellationToken)
        {
            // Se não tiver lógica de "todas as empresas" aqui, pode até lançar NotImplemented ou retornar vazio
            _logger.LogWarning("BuscarRepresentantesAsync(cancellationToken) não implementado. Use BuscarRepresentantesEmpresaAsync.");
            return Array.Empty<RepresentanteErpDto>();
        }

        private string MontarEnvelopeSoap(int codEmp, int codFil, int? codRep)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
            sb.AppendLine(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.senior.com.br"">");
            sb.AppendLine(@"  <soapenv:Body>");
            sb.AppendLine(@"    <ser:Exportar_2>");
            sb.AppendLine($"      <user>{_defaults.Usuario}</user>");
            sb.AppendLine($"      <password>{_defaults.Senha}</password>");
            sb.AppendLine(@"      <encryption>0</encryption>");
            sb.AppendLine(@"      <parameters>");
            sb.AppendLine($"        <codEmp>{codEmp}</codEmp>");
            sb.AppendLine($"        <codRep>{(codRep?.ToString() ?? "")}</codRep>");
            sb.AppendLine($"        <codfil>{codFil}</codfil>");
            sb.AppendLine($"        <identificadorSistema>{_defaults.IdentificadorSistema}</identificadorSistema>");
            sb.AppendLine($"        <quantidadeRegistros>{_defaults.QuantidadeRegistros}</quantidadeRegistros>");
            sb.AppendLine($"        <tipoIntegracao>{_defaults.TipoIntegracao}</tipoIntegracao>");
            sb.AppendLine(@"      </parameters>");
            sb.AppendLine(@"    </ser:Exportar_2>");
            sb.AppendLine(@"  </soapenv:Body>");
            sb.AppendLine(@"</soapenv:Envelope>");
            return sb.ToString();
        }

        private IReadOnlyList<RepresentanteErpDto> ParseResposta(string xml, int codEmp)
        {
            var lista = new List<RepresentanteErpDto>();

            try
            {
                var doc = XDocument.Parse(xml);
                XNamespace ser = "http://services.senior.com.br";

                var resultNode = doc
                    .Descendants(ser + "Exportar_2Response")
                    .Descendants("result")
                    .FirstOrDefault();

                if (resultNode is null)
                {
                    _logger.LogWarning("Nó <result> não encontrado na resposta Exportar_2.");
                    return lista;
                }

                var repsNodes = resultNode.Elements("representante");

                foreach (var r in repsNodes)
                {
                    var codRepStr = r.Element("codRep")?.Value;

                    if (!int.TryParse(codRepStr, out var codRep) || codRep <= 0)
                    {
                        // Loga e ignora esse registro inválido
                        _logger.LogWarning("Representante com codRep inválido: '{CodRepStr}' - nó ignorado.", codRepStr);
                        continue;
                    }



                    var dto = new RepresentanteErpDto
                    {
                        CodRep = codRep,
                        ApeRep = r.Element("apeRep")?.Value,
                        NomRep = r.Element("nomRep")?.Value ?? "",
                        BaiRep = r.Element("baiRep")?.Value,
                        CidRep = r.Element("cidRep")?.Value,
                        SigUfs = r.Element("sigUfs")?.Value,
                        EndRep = r.Element("endRep")?.Value,
                        CepRep = r.Element("cepRep")?.Value,
                        CgcCpf = r.Element("cgcCpf")?.Value,
                        InsEst = r.Element("insEst")?.Value,
                        InsMun = r.Element("insMun")?.Value,
                        FonRep = r.Element("fonRep")?.Value,
                        FonRe2 = r.Element("fonRe2")?.Value,
                        FonRe3 = r.Element("fonRe3")?.Value,
                        FaxRep = r.Element("faxRep")?.Value,
                        IntNet = r.Element("intNet")?.Value,
                        TipRep = r.Element("tipRep")?.Value,
                        SitRep = r.Element("sitRep")?.Value,
                        SitWmw = r.Element("sitWmw")?.Value,
                        CalIns = r.Element("calIns")?.Value,
                        CalIrf = r.Element("calIrf")?.Value,
                        CalIss = r.Element("calIss")?.Value,
                        GerTit = r.Element("gerTit")?.Value,
                        IndExp = TryInt(r.Element("indExp")?.Value),
                        CxaPst = TryInt(r.Element("cxaPst")?.Value),
                        CodCdi = TryInt(r.Element("codCdi")?.Value),
                        CodMot = TryInt(r.Element("codMot")?.Value),
                        DatCad = TryDate(r.Element("datCad")?.Value),
                        DatAtu = TryDate(r.Element("datAtu")?.Value),
                        DatMot = TryDate(r.Element("datMot")?.Value),
                        DatNas = TryDate(r.Element("datNas")?.Value),
                        DatPal = TryDate(r.Element("datPal")?.Value),
                        DatRge = TryDate(r.Element("datRge")?.Value),
                        HorCad = TryInt(r.Element("horCad")?.Value),
                        HorAtu = TryInt(r.Element("horAtu")?.Value),
                        HorMot = TryInt(r.Element("horMot")?.Value),
                        HorPal = TryInt(r.Element("horPal")?.Value),
                        NumRge = r.Element("numRge")?.Value,
                        OrgRge = r.Element("orgRge")?.Value,
                        EenRep = r.Element("eenRep")?.Value,
                        CplEnd = r.Element("cplEnd")?.Value,
                        NenRep = r.Element("nenRep")?.Value,
                        QtdDep = TryInt(r.Element("qtdDep")?.Value),
                        RepCli = TryInt(r.Element("repCli")?.Value),
                        RepFor = TryInt(r.Element("repFor")?.Value),
                        SenRep = r.Element("senRep")?.Value,
                        SeqInt = TryInt(r.Element("seqInt")?.Value),
                        ZipCod = r.Element("zipCod")?.Value
                    };

                    var h = r.Element("historico");
                    if (h != null)
                    {
                        var param = new RepresentanteParametrosEmpresaErpDto
                        {
                            CodEmp = codEmp,
                            CodRep = codRep,
                            CodRve = h.Element("codRve")?.Value ?? "",
                            PerCom = TryDec(h.Element("perCom")?.Value),
                            PerCos = TryDec(h.Element("perCos")?.Value),
                            ComFat = TryDec(h.Element("comFat")?.Value),
                            ComRec = TryDec(h.Element("comRec")?.Value),
                            PerIrf = TryDec(h.Element("perIrf")?.Value),
                            PerIss = TryDec(h.Element("perIss")?.Value),
                            PerIns = TryDec(h.Element("perIns")?.Value),
                            IpiCom = h.Element("ipiCom")?.Value ?? "N",
                            IcmCom = h.Element("icmCom")?.Value ?? "N",
                            SubCom = h.Element("subCom")?.Value ?? "N",
                            FreCom = h.Element("freCom")?.Value ?? "N",
                            SegCom = h.Element("segCom")?.Value ?? "N",
                            EmbCom = h.Element("embCom")?.Value ?? "N",
                            EncCom = h.Element("encCom")?.Value ?? "N",
                            OutCom = h.Element("outCom")?.Value ?? "N",
                            DarCom = h.Element("darCom")?.Value ?? "N",
                            RecAdc = h.Element("recAdc")?.Value,
                            RecAoc = h.Element("recAoc")?.Value,
                            RecPcj = h.Element("recPcj")?.Value,
                            RecPcm = h.Element("recPcm")?.Value,
                            RecPcc = h.Element("recPcc")?.Value,
                            RecPce = h.Element("recPce")?.Value,
                            RecPco = h.Element("recPco")?.Value,
                            ComPri = h.Element("comPri")?.Value ?? "N",
                            RepSup = TryInt(h.Element("repSup")?.Value),
                            CatRep = h.Element("catRep")?.Value,
                            ComSup = TryDec(h.Element("comSup")?.Value),
                            CodBan = h.Element("codBan")?.Value,
                            CodAge = h.Element("codAge")?.Value,
                            CcbRep = h.Element("ccbRep")?.Value,
                            TipCol = TryInt(h.Element("tipCol")?.Value),
                            NumCad = TryInt(h.Element("numCad")?.Value),
                            VenVmp = TryDec(h.Element("venVmp")?.Value),
                            RecVmt = TryDec(h.Element("recVmt")?.Value),
                            PerCqt = TryDec(h.Element("perCqt")?.Value),
                            PerCvl = TryDec(h.Element("perCvl")?.Value),
                            CriRat = TryInt(h.Element("criRat")?.Value) ?? 5,
                            CtaRed = TryInt(h.Element("ctaRed")?.Value),
                            CtaRcr = TryInt(h.Element("ctaRcr")?.Value),
                            CtaFdv = TryInt(h.Element("ctaFdv")?.Value),
                            CtaFcr = TryInt(h.Element("ctaFcr")?.Value),
                            ConEst = h.Element("conEst")?.Value ?? "N",
                            InsCom = h.Element("insCom")?.Value,
                            IssCom = h.Element("issCom")?.Value,
                            CofCom = h.Element("cofCom")?.Value,
                            PisCom = h.Element("pisCom")?.Value,
                            IrfCom = h.Element("irfCom")?.Value,
                            CslCom = h.Element("cslCom")?.Value,
                            OurCom = h.Element("ourCom")?.Value,
                            PifCom = h.Element("pifCom")?.Value,
                            CffCom = h.Element("cffCom")?.Value,
                            AvaVlr = TryDec(h.Element("avaVlr")?.Value),
                            AvaVlu = TryDec(h.Element("avaVlu")?.Value),
                            AvaVls = TryDec(h.Element("avaVls")?.Value),
                            AvaAti = h.Element("avaAti")?.Value,
                            AvaMot = TryInt(h.Element("avaMot")?.Value),
                            AvaObs = h.Element("avaObs")?.Value,
                            AvdAlt = TryDate(h.Element("avdAlt")?.Value),
                            AvhAlt = TryInt(h.Element("avhAlt")?.Value),
                            AvuAlt = TryDec(h.Element("avuAlt")?.Value),
                            AvdGer = TryDate(h.Element("avdGer")?.Value),
                            AvhGer = TryInt(h.Element("avhGer")?.Value),
                            AvuGer = TryDec(h.Element("avuGer")?.Value),
                            RepAud = h.Element("repAud")?.Value,
                            ComAss = TryDec(h.Element("comAss")?.Value)
                        };

                        dto.ParametrosPorEmpresa.Add(param);
                    }

                    lista.Add(dto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao interpretar XML de representantes.");
            }

            return lista;

            static int? TryInt(string? s)
                => int.TryParse(s, out var v) ? v : null;

            static decimal? TryDec(string? s)
                => decimal.TryParse(s, out var v) ? v : null;

            static DateTime? TryDate(string? s)
                => DateTime.TryParse(s, out var d) ? d : null;
        }
    }
}
