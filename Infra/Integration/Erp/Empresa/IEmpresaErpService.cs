namespace Forca_venda_api.Infra.Integration.Erp.Empresa;

public interface IEmpresaErpService
{
    Task<IReadOnlyList<EmpresaErpDto>> BuscarEmpresasAsync();
}
