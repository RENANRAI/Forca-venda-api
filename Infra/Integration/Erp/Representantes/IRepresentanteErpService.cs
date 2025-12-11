using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ForcaVendas.Api.Infra.Integration.Erp.Representantes
{
    public interface IRepresentanteErpService
    {
        /// <summary>
        /// Busca representantes de uma empresa específica no ERP.
        /// </summary>
        Task<IReadOnlyList<RepresentanteErpDto>> BuscarRepresentantesEmpresaAsync(
            int codEmp,
            int CodFil,
            CancellationToken cancellationToken,
            int? codRep = null);

        /// <summary>
        /// (Opcional) Se quiser ter uma chamada para buscar todos por todas empresas, 
        /// pode usar esse método depois, ou remover se não for usar.
        /// </summary>
        Task<IReadOnlyList<RepresentanteErpDto>> BuscarRepresentantesAsync(
            CancellationToken cancellationToken);
    }
}
