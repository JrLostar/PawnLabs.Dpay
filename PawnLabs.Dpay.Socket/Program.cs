using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PawnLabs.Dpay.Socket.Hub;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHealthChecks();

#region Cors

builder.Services.AddCors(options =>
{
    options.AddPolicy("policy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
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

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/dpayHub"))
                context.Token = accessToken;

            return Task.CompletedTask;
        }
    };
});

#endregion

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<DpayHub>("/dpayHub");

app.MapHealthChecks("/health");

app.Run();
