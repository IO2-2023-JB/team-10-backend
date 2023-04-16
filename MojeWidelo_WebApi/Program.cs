using Microsoft.AspNetCore.Rewrite;
using MojeWidelo_WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Extensions

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepository();
builder.Services.ConfigureFilters();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureSwagger();
builder.Services.ConfigureServices();
builder.Services.ConfigureManagers();
builder.Services.ConfigureVariables(builder.Configuration);

#endregion

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseCors("EnableCORS");
}

// serve static files (frontend assets)
app.UseStaticFiles();

// serve React app
RewriteOptions rewrite = new RewriteOptions().AddRewrite("^(?!api).+", "/", true);
app.UseRewriter(rewrite);
app.UseDefaultFiles();
app.UseStaticFiles();

// handle API routes
app.UsePathBase(new PathString("/api"));
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
