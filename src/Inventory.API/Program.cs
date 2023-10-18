using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Inventory.Core.Helper;
using Microsoft.AspNetCore.Mvc;
using Inventory.API.RateLimits;
using Microsoft.OpenApi.Models;
using Inventory.API.Filters;
using Serilog;
using Inventory.API.Middleware;
using System.Threading.RateLimiting;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Inventory.API.Extensions;
using Inventory.Database.DbContext;
using Inventory.Core.Configurations;
using Inventory.Core.Common;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddOptions<JwtConfig>()
                .Bind(config.GetSection(JwtConfig.Name));


builder.Services.AddDatabase(config)
                .AddRedisCache(config)
                .AddRepository()
                .AddServices();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
                .AddJwtBearer(jwtOptions =>
                    {
                        jwtOptions.SaveToken = true;
                        jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidAudience = config["Bearer:Audience"],
                            ValidIssuer = config["Bearer:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(config["Bearer:SecretKey"]!))
                        };
                    });
//.AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = config["OAuth:Google:ClientID"]!;
//        googleOptions.ClientSecret = config["OAuth:Google:Secret"]!;
//        googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
//    });

builder.Services.AddControllers(options =>
    {
        options.Conventions
            .Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //})
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = (errorContext) =>
                    {
                        var errors = errorContext.ModelState
                            .Where(m => m.Value!.Errors.Any())
                            .Select(m => new ResultMessage(
                                m.Key,
                                m.Value!.Errors.FirstOrDefault()!.ErrorMessage))
                            .ToList();
                        return new BadRequestObjectResult(errors);
                    };
                });

builder.Services.AddRateLimiter(option =>
    {
        option.AddPolicy<string, RefreshTokenLimitPolicy>("RefresshTokenLimit");
        option.OnRejected = (context, _) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response
                .WriteAsync(new ResultMessage("Too many request", "Please try again later!!").ToString(), cancellationToken: _);

            return new ValueTask();
        };
        option.GlobalLimiter = PartitionedRateLimiter.CreateChained(
            PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
            {
                var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 500,
                    Window = TimeSpan.FromMinutes(1)
                });
            }),
            PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
            {
                var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
                return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 25000,
                    Window = TimeSpan.FromMinutes(60)
                });
            })
        );
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.OperationFilter<AuthorizationOperationFilter>();
        options.SwaggerDoc("v1", new OpenApiInfo { Title = config["PAppName"], Version = "v1" });

    });

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var allowedCors = config.GetSection("AllowedCORS").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedCors)
                .WithExposedHeaders(new[] { "Location" })
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(
//        options =>
//        {
//            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger");
//            options.RoutePrefix = string.Empty;
//        });
//}
app.UseSwagger();

app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory.API");
        options.RoutePrefix = string.Empty;
    });

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        Log.Information("Trying to connect Db & Get migration");
        if (!context.Database.GetPendingMigrations().Any())
        {
            Log.Information("No migration needed!");
        }
        else
        {
            Log.Information("Started migration!");
            context.Database.Migrate();
        }
    }
    catch (Exception)
    {
        Log.Error("Cannot connect to Database Server!");
    }
}

app.Run();
