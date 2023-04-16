using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;
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

if (builder.Environment.IsProduction())
{
	builder.Services.AddLettuceEncrypt();
	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(80);
		options.ListenAnyIP(443, configure => configure.UseHttps());
	});
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("EnableCORS");

RewriteOptions rewriteHttps = new RewriteOptions().AddRedirectToHttpsPermanent();
app.UseRewriter(rewriteHttps);

var frontendPath = builder.Configuration.GetValue<string>("Variables:FrontendPath");
var frontendAbsolutePath = Path.GetFullPath(frontendPath);
var fileProvider = new PhysicalFileProvider(frontendAbsolutePath);
var staticFileOptions = new StaticFileOptions { FileProvider = fileProvider };
var defaultFilesOptions = new DefaultFilesOptions { FileProvider = fileProvider };

// serve static files (frontend assets)
app.UseStaticFiles(staticFileOptions);

// serve React app
RewriteOptions rewriteReact = new RewriteOptions().AddRewrite("^(?!api).+", "/", true);
app.UseRewriter(rewriteReact);
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles(staticFileOptions);

// handle API routes
app.UsePathBase(new PathString("/api"));
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
