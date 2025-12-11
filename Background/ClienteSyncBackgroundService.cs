using ForcaVendas.Api.Domain.Services;
using Microsoft.SqlServer.Server;

namespace ForcaVendas.Api.Background;

public class ClienteSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClienteSyncBackgroundService> _logger;
    //teste
    public ClienteSyncBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ClienteSyncBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background de sincronização de clientes iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {

                using var scope = _serviceProvider.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<ClienteSyncService>();

                await syncService.SincronizarClientesAsync(stoppingToken);

                _logger.LogInformation("Sincronização de clientes concluída com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao sincronizar clientes.");
            }

            // espera 15 minutos antes da próxima sincronização
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }

        _logger.LogInformation("Background de sincronização de clientes finalizado.");
    }
}
