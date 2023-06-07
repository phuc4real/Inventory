using Inventory.Core.Helper;
using Inventory.Core.Options;
using Inventory.Repository.DbContext;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Inventory.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Inventory")));


builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider("Inventory",typeof(DataProtectorTokenProvider<AppUser>))
    .AddSignInManager();

builder.Services.AddOptions<JWTOption>()
    .Bind(builder.Configuration.GetSection(JWTOption.JWTBearer));

builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(
        jwtOptions =>
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
    .AddGoogle(
        googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["OAuth:Google:ClientID"]!;
            googleOptions.ClientSecret = builder.Configuration["OAuth:Google:Secret"]!;
            googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
        }
    );

builder.Services.AddControllers(
    options => options.Conventions.Add(
        new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

builder.Services.AddScoped<IAuthService, AuthService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
