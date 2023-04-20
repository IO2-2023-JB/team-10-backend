using Microsoft.Extensions.FileProviders;

namespace MojeWidelo_WebApi.Extensions
{
	public static class ConfigurationExtensions
	{
		public static (StaticFileOptions, DefaultFilesOptions) FrontendPathConfiguration(
			this ConfigurationManager configurationManager
		)
		{
			var frontendPath = configurationManager.GetValue<string>("Variables:FrontendPath");
			var frontendAbsolutePath = Path.GetFullPath(frontendPath);
			var fileProvider = new PhysicalFileProvider(frontendAbsolutePath);
			var staticFileOptions = new StaticFileOptions { FileProvider = fileProvider };
			var defaultFilesOptions = new DefaultFilesOptions { FileProvider = fileProvider };
			return (staticFileOptions, defaultFilesOptions);
		}
	}
}
