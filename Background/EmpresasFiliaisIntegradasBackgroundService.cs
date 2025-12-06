using ForcaVendas.Api.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ForcaVendas.Api.Background;

public class EmpresasFiliaisIntegradasBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmpresasFiliaisIntegradasBackgroundService> _logger;

    public EmpresasFiliaisIntegradasBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmpresasFiliaisIntegradasBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background de sincronização de Empresas/Filiais Integradas iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<EmpresasFiliaisIntegradasSyncService>();

                await syncService.SincronizarAsync(stoppingToken);

                _logger.LogInformation("Sincronização de Empresas/Filiais Integradas concluída com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao sincronizar Empresas/Filiais Integradas.");
            }

            // mesmo esquema do cliente – aqui você ajusta o intervalo
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }

        _logger.LogInformation("Background de sincronização de Empresas/Filiais Integradas finalizado.");
    }
}
