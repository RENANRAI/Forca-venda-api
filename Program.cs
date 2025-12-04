using ForcaVendas.Api.Data;
using Microsoft.EntityFrameworkCore;
using ForcaVendas.Api.Background;
using ForcaVendas.Api.Data;
using ForcaVendas.Api.Integration.Erp;
using ForcaVendas.Api.Services;
using Microsoft.EntityFrameworkCore;


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

// HttpClient para o serviço SOAP de clientes
builder.Services.AddHttpClient<IClienteErpService, ClienteErpService>();


//Serviços de integração e sync
builder.Services.AddScoped<IClienteErpService, ClienteErpService>();
builder.Services.AddScoped<ClienteSyncService>();

// BackgroundService de sincronização
builder.Services.AddHostedService<ClienteSyncBackgroundService>();

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
