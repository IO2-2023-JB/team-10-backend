using Contracts;
using Entities.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MojeWidelo_WebApi.Filters;
using Repository;
using Repository.Managers;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

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
			services.AddScoped<IReactionRepository, ReactionRepository>();
			services.AddScoped<ICommentRepository, CommentRepository>();
			services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
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
						ValidateLifetime = false,
						ValidateIssuerSigningKey = true,
						ValidIssuer = "https://localhost:5001",
						ValidAudience = "https://localhost:5001",
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hasloooo1234$#@!"))
					};

					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = (context) =>
						{
							StringValues values;

							if (!context.Request.Query.TryGetValue("access_token", out values))
							{
								return Task.CompletedTask;
							}

							if (values.Count > 1)
							{
								context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								context.Fail(
									"Only one 'access_token' query string parameter can be defined. "
										+ $"However, {values.Count:N0} were included in the request."
								);

								return Task.CompletedTask;
							}

							var token = values.Single();

							if (String.IsNullOrWhiteSpace(token))
							{
								context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								context.Fail(
									"The 'access_token' query string parameter was defined, "
										+ "but a value to represent the token was not included."
								);

								return Task.CompletedTask;
							}

							context.Token = token;

							return Task.CompletedTask;
						}
					};
				});

			services
				.AddControllers()
				.AddJsonOptions(x =>
				{
					x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});
		}

		public static void ConfigureFilters(this IServiceCollection services)
		{
			services.AddScoped<ModelValidationFilter>();
			services.AddScoped<ObjectIdValidationFilter>();
			services.AddScoped<VideoExtensionValidationFilter>();
		}

		public static void ConfigureManagers(this IServiceCollection services)
		{
			services.AddScoped<UsersManager>();
			services.AddScoped<VideoManager>();
			services.AddScoped<CommentManager>();
			services.AddScoped<SubscriptionsManager>();
		}

		public static void ConfigureVariables(this IServiceCollection services, ConfigurationManager configuration)
		{
			services.Configure<Variables>(configuration.GetSection(nameof(Variables)));
			services.AddOptions<Variables>();
		}
	}
}
