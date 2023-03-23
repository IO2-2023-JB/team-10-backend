using Microsoft.AspNetCore.Authentication.JwtBearer;
using MojeWidelo_WebApi.Extensions;
using Microsoft.IdentityModel.Tokens;
using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Extensions

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepository();
builder.Services.ConfigureFilters();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#endregion

void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "https://localhost:5001",
            ValidAudience = "https://localhost:5001",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hasloooo1234$#@!"))
        };

    });

    services.AddControllers();
}


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
