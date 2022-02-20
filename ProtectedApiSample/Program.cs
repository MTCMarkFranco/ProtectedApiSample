using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Pull in the Configuration for AAD Authentication
var aadConfig = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();

// 2. apply it to the MiddleWare (Microsoft.Identity.Web)
builder.Services.AddMicrosoftIdentityWebApiAuthentication(aadConfig, "AzureAd", "Bearer", true)
    .EnableTokenAcquisitionToCallDownstreamApi()
     .AddDownstreamWebApi("WeatherApi", aadConfig.GetSection("DownstreamAPI"))
    .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddControllers();

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

app.UseCertificateForwarding();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
