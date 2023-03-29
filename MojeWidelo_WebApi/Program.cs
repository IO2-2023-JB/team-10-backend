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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureServices();

var app = builder.Build();

app.UsePathBase(new PathString("/api"));
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("EnableCORS");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
