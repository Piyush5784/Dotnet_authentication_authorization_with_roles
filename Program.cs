using System.Text;
using Auth_web_2.Context;
using Auth_web_2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("db");
var config = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("JwtSettings:SecretKey")!));

builder.Services.AddOpenApi();
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = config["JwtSettings:Issuer"],
    ValidAudience = config["JwtSettings:Audience"],
    IssuerSigningKey = key
});


builder.Services.AddDbContext<MyDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

