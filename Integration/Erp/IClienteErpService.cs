using ForcaVendas.Api.Integration.Erp;

namespace ForcaVendas.Api.Integration.Erp;

public interface IClienteErpService
{
    /// <summary>
    /// Busca clientes no ERP.
    /// <paramref name="alteradosDesde"/> pode ser usado para otimizar (só alterados).
    /// </summary>
    Task<IReadOnlyList<ClienteErpDto>> BuscarClientesAsync(DateTime? alteradosDesde);
}
