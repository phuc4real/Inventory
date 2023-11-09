using Serilog;
using Inventory.API.Middleware;
using Inventory.API.Extensions;
using Inventory.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var service = builder.Services;

service.AddOptions<JwtConfig>()
       .Bind(configuration.GetSection(JwtConfig.Name));

service.AddDatabaseService(configuration);

service.AddRedisCacheService(configuration);

service.AddRepositoryService();

service.AddAppService();

service.AddAuthenticateService(configuration);

service.AddControllerService();

service.AddRateLimiterService();

service.AddEndpointsApiExplorer();

service.AddSwaggerSerivce(configuration);

builder.Host.UseSerilog((context, configs) => configs.ReadFrom.Configuration(context.Configuration));

service.AddCorService(configuration);

var app = builder.Build();

app.ApplyMigrations();

app.SeedingData();

app.UseSwagger();

app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory.API");
        options.RoutePrefix = string.Empty;
    });

app.UseSerilogRequestLogging();

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseRateLimiter();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
