namespace ForcaVendas.Api.Infra.Integration.Erp.EmpresasFiliais;

public interface IEmpresasFiliaisErpService
{
    /// <summary>
    /// Consulta no ERP Senior quais empresas/filiais estão configuradas para integração.
    /// </summary>
    Task<IReadOnlyList<EmpresasFiliaisErpDto>> BuscarEmpresasFiliaisIntegradasAsync();
}
