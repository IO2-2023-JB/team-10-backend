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

#endregion

builder.Services.AddEndpointsApiExplorer();

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
