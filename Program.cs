using Microsoft.EntityFrameworkCore;
using ForcaVendas.Api.Background;

using ForcaVendas.Api.Infra.Data;
using ForcaVendas.Api.Infra.Integration.Erp.Clientes;
using ForcaVendas.Api.Infra.Config;
using ForcaVendas.Api.Infra.Integration.Erp.EmpresasFiliais;
using ForcaVendas.Api.Domain.Services;
using ForcaVendas.Api.Infra.Integration.Erp.Filiais;


var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:5185") // ajuste p/ porta do React
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Connection String
var connectionString = builder.Configuration.GetConnectionString("ForcaVendas");
builder.Services.AddDbContext<ForcaVendasContext>(options =>
    options.UseSqlServer(connectionString));

// bind das configs do ERP
builder.Services.Configure<ErpSeniorConfig>(
    builder.Configuration.GetSection("ErpSenior"));

// HttpClient para o serviço SOAP de clientes
builder.Services.AddHttpClient<IClienteErpService, ClienteErpService>();
builder.Services.AddHttpClient<IFiliaisErpService, FiliaisErpService>();
builder.Services.AddHttpClient<EmpresasFiliaisErpService>();
builder.Services.AddHttpClient<FiliaisErpService>();

//Serviços de integração e sync
builder.Services.AddScoped<IClienteErpService, ClienteErpService>();
builder.Services.AddScoped<ClienteSyncService>();
builder.Services.AddScoped<FilialSyncService>();
builder.Services.AddScoped<IFiliaisErpService, FiliaisErpService>();

builder.Services.AddScoped<IEmpresasFiliaisErpService, EmpresasFiliaisErpService>();
builder.Services.AddScoped<EmpresasFiliaisIntegradasSyncService>();


// BackgroundService de sincronização
builder.Services.AddHostedService<IntegracoesBackgroundService>();


//builder.Services.AddHostedService<ClienteSyncBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);
app.UseAuthorization();
app.MapControllers();
app.Run();
