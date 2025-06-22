using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PawnLabs.Dpay.Api.Extension;
using PawnLabs.Dpay.Business;
using PawnLabs.Dpay.Business.BackgroundService;
using PawnLabs.Dpay.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddMemoryCache();

builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddHelpers();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<StellarBackgroundService>();
builder.Services.AddHostedService<StellarBackgroundService>();

#region Cors

var corsDomains = builder.Configuration.GetSection("CorsDomains").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "policy",
                      builder =>
                      {
                          builder.WithOrigins(corsDomains)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                      });
});

#endregion

#region Authentication

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("TokenConfiguration:Issuer"),

        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("TokenConfiguration:Audience"),

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("TokenConfiguration:SecretKey").Get<string>())),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

#endregion

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("policy");

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
