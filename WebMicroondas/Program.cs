using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Domain.Services;
using WebMicroondas.Infra.Persistence;
using WebMicroondas.Infra.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DI
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<IMicrowaveService, MicrowaveService>();
builder.Services.AddSingleton<IProgramRepository, ProgramJsonRepository>();
builder.Services.AddSingleton<IProgramService, ProgramService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }