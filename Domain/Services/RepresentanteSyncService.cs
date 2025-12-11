using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForcaVendas.Api.Domain.Entities;
using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Representantes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ForcaVendas.Api.Domain.Services
{
    public class RepresentanteSyncService
    {
        private readonly ForcaVendasContext _db;
        private readonly IRepresentanteErpService _erp;
        private readonly ILogger<RepresentanteSyncService> _logger;

        public RepresentanteSyncService(
            ForcaVendasContext db,
            IRepresentanteErpService erp,
            ILogger<RepresentanteSyncService> logger)
        {
            _db = db;
            _erp = erp;
            _logger = logger;
        }

        /// <summary>
        /// Busca todas as empresas ativas e sincroniza os representantes de cada uma.
        /// </summary>
        public async Task BuscarRepresentantesAsync(CancellationToken cancellationToken)
        {
            /*var empresas = await _db.Empresas
                .Where(e => e.SitEmp)              // ajusta se o campo de ativo for outro
                .Select(e => e.CodEmp)
                .Distinct()
                .ToListAsync(cancellationToken);*/

            var empresas = await _db.EmpresasFiliaisIntegradas
                .AsNoTracking()
                .Where(x => x.SitReg)
                .Select(x => new { x.CodEmp, x.CodFil })
                .ToListAsync(cancellationToken);


            foreach (var Par in empresas)
            {
                _logger.LogInformation("Sincronizando representantes da empresa {CodEmp}", Par.CodEmp);

                // AQUI estava o erro: chamar o método por empresa
                await BuscarRepresentantesEmpresaAsync(Par.CodEmp, Par.CodFil, cancellationToken);
            }
        }

        /// <summary>
        /// Sincroniza representantes de uma empresa específica.
        /// </summary>
        public async Task BuscarRepresentantesEmpresaAsync(int codEmp, int codfil, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando sincronização de representantes Emp={CodEmp}", codEmp);

            var repsErp = await _erp.BuscarRepresentantesEmpresaAsync(codEmp, codfil, cancellationToken);

            var agora = DateTime.UtcNow;

            foreach (var repErp in repsErp)
            {
                var rep = await _db.Representantes
                    .FirstOrDefaultAsync(x => x.CodRep == repErp.CodRep, cancellationToken);

                if (rep is null)
                {
                    rep = new Representante
                    {
                        CodRep = repErp.CodRep,
                        DatCri = agora,
                        SitReg = true
                    };
                    _db.Representantes.Add(rep);
                }

                // ===== Campos principais do Representante =====
                rep.NomRep = repErp.NomRep;
                rep.ApeRep = repErp.ApeRep;
                rep.BaiRep = repErp.BaiRep;
                rep.CidRep = repErp.CidRep;
                rep.SigUfs = repErp.SigUfs;
                rep.EndRep = repErp.EndRep;
                rep.CepRep = repErp.CepRep;
                rep.CgcCpf = repErp.CgcCpf;
                rep.InsEst = repErp.InsEst;
                rep.InsMun = repErp.InsMun;
                rep.FonRep = repErp.FonRep;
                rep.FonRe2 = repErp.FonRe2;
                rep.FonRe3 = repErp.FonRe3;
                rep.FaxRep = repErp.FaxRep;
                rep.IntNet = repErp.IntNet;
                rep.TipRep = repErp.TipRep;
                rep.SitRep = repErp.SitRep;
                rep.SitWmw = repErp.SitWmw;
                rep.CalIns = repErp.CalIns;
                rep.CalIrf = repErp.CalIrf;
                rep.CalIss = repErp.CalIss;
                rep.GerTit = repErp.GerTit;
                rep.IndExp = repErp.IndExp;
                rep.CxaPst = repErp.CxaPst;
                rep.CodCdi = repErp.CodCdi;
                rep.CodMot = repErp.CodMot;
                rep.DatCad = repErp.DatCad;
                rep.DatAtu = repErp.DatAtu;
                rep.DatMot = repErp.DatMot;
                rep.DatNas = repErp.DatNas;
                rep.DatPal = repErp.DatPal;
                rep.DatRge = repErp.DatRge;
                rep.HorCad = repErp.HorCad;
                rep.HorAtu = repErp.HorAtu;
                rep.HorMot = repErp.HorMot;
                rep.HorPal = repErp.HorPal;
                rep.NumRge = repErp.NumRge;
                rep.OrgRge = repErp.OrgRge;
                rep.EenRep = repErp.EenRep;
                rep.CplEnd = repErp.CplEnd;
                rep.NenRep = repErp.NenRep;
                rep.QtdDep = repErp.QtdDep;
                rep.RepCli = repErp.RepCli;
                rep.RepFor = repErp.RepFor;
                rep.SenRep = repErp.SenRep;
                rep.SeqInt = repErp.SeqInt;
                rep.ZipCod = repErp.ZipCod;

                rep.SitReg = true;
                rep.DatAtuApp = agora;

                // ===== Parâmetros por empresa =====
                var parametros = repErp.ParametrosPorEmpresa ?? new List<RepresentanteParametrosEmpresaErpDto>();

                foreach (var paramErp in parametros)
                {
                    var param = await _db.RepresentanteParametrosEmpresas
                        .FirstOrDefaultAsync(x =>
                            x.CodEmp == paramErp.CodEmp &&
                            x.CodRep == paramErp.CodRep,
                            cancellationToken);

                    if (param is null)
                    {
                        param = new RepresentanteParametrosEmpresa
                        {
                            Id = Guid.NewGuid(),
                            CodEmp = paramErp.CodEmp,
                            CodRep = paramErp.CodRep,
                            DatCri = agora,
                            SitReg = true
                        };
                        _db.RepresentanteParametrosEmpresas.Add(param);
                    }

                    param.CodRve = paramErp.CodRve;
                    param.PerCom = paramErp.PerCom;
                    param.PerCos = paramErp.PerCos;
                    param.ComFat = paramErp.ComFat;
                    param.ComRec = paramErp.ComRec;
                    param.PerIrf = paramErp.PerIrf;
                    param.PerIss = paramErp.PerIss;
                    param.PerIns = paramErp.PerIns;
                    param.IpiCom = paramErp.IpiCom;
                    param.IcmCom = paramErp.IcmCom;
                    param.SubCom = paramErp.SubCom;
                    param.FreCom = paramErp.FreCom;
                    param.SegCom = paramErp.SegCom;
                    param.EmbCom = paramErp.EmbCom;
                    param.EncCom = paramErp.EncCom;
                    param.OutCom = paramErp.OutCom;
                    param.DarCom = paramErp.DarCom;
                    param.RecAdc = paramErp.RecAdc;
                    param.RecAoc = paramErp.RecAoc;
                    param.RecPcj = paramErp.RecPcj;
                    param.RecPcm = paramErp.RecPcm;
                    param.RecPcc = paramErp.RecPcc;
                    param.RecPce = paramErp.RecPce;
                    param.RecPco = paramErp.RecPco;
                    param.ComPri = paramErp.ComPri;
                    param.RepSup = paramErp.RepSup;
                    param.CatRep = paramErp.CatRep;
                    param.ComSup = paramErp.ComSup;
                    param.CodBan = paramErp.CodBan;
                    param.CodAge = paramErp.CodAge;
                    param.CcbRep = paramErp.CcbRep;
                    param.TipCol = paramErp.TipCol;
                    param.NumCad = paramErp.NumCad;
                    param.VenVmp = paramErp.VenVmp;
                    param.RecVmt = paramErp.RecVmt;
                    param.PerCqt = paramErp.PerCqt;
                    param.PerCvl = paramErp.PerCvl;
                    param.CriRat = paramErp.CriRat;
                    param.CtaRed = paramErp.CtaRed;
                    param.CtaRcr = paramErp.CtaRcr;
                    param.CtaFdv = paramErp.CtaFdv;
                    param.CtaFcr = paramErp.CtaFcr;
                    param.ConEst = paramErp.ConEst;
                    param.InsCom = paramErp.InsCom;
                    param.IssCom = paramErp.IssCom;
                    param.CofCom = paramErp.CofCom;
                    param.PisCom = paramErp.PisCom;
                    param.IrfCom = paramErp.IrfCom;
                    param.CslCom = paramErp.CslCom;
                    param.OurCom = paramErp.OurCom;
                    param.PifCom = paramErp.PifCom;
                    param.CffCom = paramErp.CffCom;
                    param.AvaVlr = paramErp.AvaVlr;
                    param.AvaVlu = paramErp.AvaVlu;
                    param.AvaVls = paramErp.AvaVls;
                    param.AvaAti = paramErp.AvaAti;
                    param.AvaMot = paramErp.AvaMot;
                    param.AvaObs = paramErp.AvaObs;
                    param.AvdAlt = paramErp.AvdAlt;
                    param.AvhAlt = paramErp.AvhAlt;
                    param.AvuAlt = paramErp.AvuAlt;
                    param.AvdGer = paramErp.AvdGer;
                    param.AvhGer = paramErp.AvhGer;
                    param.AvuGer = paramErp.AvuGer;
                    param.RepAud = paramErp.RepAud;
                    param.ComAss = paramErp.ComAss;

                    param.SitReg = true;
                    param.DatAtu = agora;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sincronização de representantes Emp={CodEmp} finalizada.", codEmp);
        }
    }
}
