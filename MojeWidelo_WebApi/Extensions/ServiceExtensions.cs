using Contracts;
using Entities.DatabaseUtils;
using Microsoft.Extensions.Options;
using MojeWidelo_WebApi.Filters;
using Repository;
using System.Reflection;

namespace MojeWidelo_WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        }

        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IUsersRepository, UsersRepository>();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MojeWideło API",
                    Version = "v1",
                    TermsOfService = new Uri("https://ww2.mini.pw.edu.pl/"),
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Niziołek Norbert, Nowak Mikołaj, Saj Patryk, Sosnowski Jakub, Zagórski Mateusz",
                        Email = "jakub.sosnowski2001@gmail.com"
                    },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Select(x => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{x.Name}.xml"))
                .Where(f => File.Exists(f)).ToArray();

                Array.ForEach(xmlDocs, d => c.IncludeXmlComments(d));
            });
        }

        public static void ConfigureFilters(this IServiceCollection services)
        {
            services.AddScoped<ModelValidationFilter>();
        }
    }
}
