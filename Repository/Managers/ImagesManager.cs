namespace Repository.Managers
{
	public class ImagesManager
	{
		private const double _maxSize = 300.0;

		public byte[] GetBytesFromBase64(string base64)
		{
			int startIdx = base64.IndexOf(',');
			return Convert.FromBase64String(base64.Substring(startIdx + 1));
		}

		public byte[] CompressFile(byte[] bytes)
		{
			Image image;

			using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
			{
				image = Image.Load(ms);
			}

			int biggerDimension = image.Width > image.Height ? image.Width : image.Height;

			if (biggerDimension > _maxSize)
			{
				double scale = _maxSize / biggerDimension;
				int width = (int)(image.Width * scale);
				int height = (int)(image.Height * scale);
				image.Mutate(x => x.Resize(width, height));
			}

			using (MemoryStream ms = new MemoryStream())
			{
				image.SaveAsPng(ms);
				return ms.ToArray();
			}
		}
	}
}
