namespace ForcaVendas.Api.Infra.Integration.Erp.Clientes;


public interface IClienteErpService
{
    /// <summary>
    /// Busca clientes no ERP.
    /// <paramref name="alteradosDesde"/> pode ser usado para otimizar (só alterados).
    /// </summary>
    Task<IReadOnlyList<ClienteErpDto>> BuscarClientesAsync(DateTime? alteradosDesde);
}


