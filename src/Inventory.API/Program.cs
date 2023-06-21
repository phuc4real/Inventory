using Inventory.Core.Options;
using Inventory.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Inventory.Core.Helper;
using System.Text.Json.Serialization;
using Inventory.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Inventory.API.RateLimits;
using Microsoft.OpenApi.Models;
using Inventory.API.Filters;
using Serilog;
using Inventory.API.Middleware;
using System.Threading.RateLimiting;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<JWTOption>()
    .Bind(builder.Configuration.GetSection(JWTOption.JWTBearer));

builder.Services
    .AddDatabase(builder.Configuration)
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
                ValidAudience = builder.Configuration["JWTBearer:ValidAudience"],
                ValidIssuer = builder.Configuration["JWTBearer:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWTBearer:SecretKey"]!))
            };
        })
    .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["OAuth:Google:ClientID"]!;
            googleOptions.ClientSecret = builder.Configuration["OAuth:Google:Secret"]!;
            googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
        }
    );

builder.Services.AddControllers(options =>
    {
        options.Conventions
            .Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options=>
    {
        options.InvalidModelStateResponseFactory = (errorContext) =>
        {
            var errors = errorContext.ModelState
                .Where(m => m.Value!.Errors.Any())
                .Select(m => new ResponseMessage(
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
                .WriteAsync(new ResponseMessage("Too many request", "Please try again later!!").ToString());

            return new ValueTask();
        };
        option.GlobalLimiter = PartitionedRateLimiter.CreateChained(
            PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
            {
                var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                });
            }),
            PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
            {
                var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
                return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 300,
                    Window = TimeSpan.FromMinutes(60)
                });
            })
        );
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
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
    }
);

builder.Host.UseSerilog((context,configuration)=>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
