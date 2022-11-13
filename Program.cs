using Microsoft.EntityFrameworkCore;
using DocumentsApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS

IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        List<CorsOrigin> origins = new List<CorsOrigin>();
        configuration.GetSection("cors:origins").Bind(origins);
        foreach (var o in origins)
        {
            policy.WithOrigins(o.uri);
        }
        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

// .WithOrigins("https://localhost:7127", "http://localhost:3000")

// Add services to the container.

builder.Services.AddDbContext<DocumentContext>(opt => opt.UseInMemoryDatabase("DocumentList"));

// Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        options =>
        {
            configuration.Bind("AzureAdB2C", options);
            options.TokenValidationParameters.NameClaimType = "name";
        },
        options =>
        {
            configuration.Bind("AzureAdB2C", options);
        }
    );

// End of the Microsoft Identity platform block

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpLogging();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
