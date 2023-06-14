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
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
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
        option.AddPolicy<string, LimitRequestPerMinutesPolicy>("LimitRequestPer5Minutes");
    } 
);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
