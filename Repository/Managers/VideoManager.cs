using System.Runtime.InteropServices;

namespace Repository.Managers
{
	public class VideoManager
	{
		public string? CreateNewPath(string id, string fileName)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			string extension = Path.GetExtension(fileName);
			return Path.Combine(location, id + "_original" + extension);
		}

		public string? GetStorageDirectory()
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
				//WILL BE IMPLEMENTED PROPERLY IN SPRINT 3
				location = "/home/ubuntu/video-storage";
			}

			return location;
		}

		public string? GetReadyFilePath(string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			return Path.Combine(location, id + ".mp4");
		}
	}
}
