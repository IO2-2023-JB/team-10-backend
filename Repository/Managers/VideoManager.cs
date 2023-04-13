using CliWrap.Buffered;
using CliWrap;
using System.Runtime.InteropServices;

namespace Repository.Managers
{
	public class VideoManager
	{
		public string? CreateNewPath(string id, string fileName)
		{
			string? location = GetStorageDirectory().Result;
			if (location == null)
				return null;

			string extension = Path.GetExtension(fileName);
			return Path.Combine(location, id + "_original" + extension);
		}

		public async Task<string?> GetStorageDirectory()
		{
			string location;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				location = Environment.GetEnvironmentVariable("MojeWideloStorage", EnvironmentVariableTarget.Machine)!;

				if (string.IsNullOrEmpty(location))
					return null;
			}
			else
			{
				BufferedCommandResult result;

				try
				{
					result = await Cli.Wrap("printenv").WithArguments(@"MojeWideloStorage").ExecuteBufferedAsync();
				}
				catch
				{
					return null;
				}

				location = result.StandardOutput.Substring(0, result.StandardOutput.Length - 1);
			}

			return location;
		}

		public string? GetReadyFilePath(string id)
		{
			string? location = GetStorageDirectory().Result;
			if (location == null)
				return null;

			return Path.Combine(location, id + ".mp4");
		}
	}
}
