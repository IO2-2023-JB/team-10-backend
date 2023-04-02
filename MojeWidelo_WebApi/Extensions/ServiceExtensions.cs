using Contracts;
using Entities.DatabaseUtils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using Repository;
using System.Reflection;
using System.Text;

namespace MojeWidelo_WebApi.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager configuration)
		{
			services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
			services.AddSingleton<IDatabaseSettings>(
				provider => provider.GetRequiredService<IOptions<DatabaseSettings>>().Value
			);
		}

		public static void ConfigureRepository(this IServiceCollection services)
		{
			services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
			services.AddScoped<IUsersRepository, UsersRepository>();
			services.AddScoped<IVideoRepository, VideoRepository>();
		}

		public static void ConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc(
					"v1",
					new Microsoft.OpenApi.Models.OpenApiInfo
					{
						Title = "MojeWideło API",
						Version = "v1",
						TermsOfService = new Uri("https://ww2.mini.pw.edu.pl/"),
						Contact = new Microsoft.OpenApi.Models.OpenApiContact
						{
							Name = "Niziołek Norbert, Nowak Mikołaj, Saj Patryk, Sosnowski Jakub, Zagórski Mateusz",
							Email = "jakub.sosnowski2001@gmail.com"
						},
					}
				);

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);

				var currentAssembly = Assembly.GetExecutingAssembly();
				var xmlDocs = currentAssembly
					.GetReferencedAssemblies()
					.Select(x => Path.Combine(Path.GetDirectoryName(currentAssembly.Location)!, $"{x.Name}.xml"))
					.Where(f => File.Exists(f))
					.ToArray();

				Array.ForEach(xmlDocs, d => c.IncludeXmlComments(d));
			});
		}

		public static void ConfigureServices(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			});

			services.AddCors(options =>
			{
				options.AddPolicy(
					"EnableCORS",
					builder =>
					{
						builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
					}
				);
			});
			services
				.AddAuthentication(opt =>
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

		public static void ConfigureFilters(this IServiceCollection services)
		{
			services.AddScoped<ModelValidationFilter>();
			services.AddScoped<ObjectIdValidationFilter>();
		}
	}
}
