using Core;
using Core.Db;
using Core.Models;
using Logic.Helpers;
using static Core.Enums.LMSEnum;

namespace Logic.Services
{
	public interface IMediaService
	{
		Task DeleteMediaAsync(Media media);
		Task DeleteMediaAsync(string fileName);
		EventMediaType GetMediaType(string fileName);
		Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string directoryPath, EventMediaType mediaType);
	}

	public class MediaService : BaseHelper, IMediaService
	{
		private readonly IFileManager _fileManager;

		public MediaService(IFileManager fileManager, AppDbContext context) : base(context)
		{
			_fileManager = fileManager;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public EventMediaType GetMediaType(string fileName)
		{
			return LMSExtension.MediaTypes.GetValueOrDefault(Path.GetExtension(fileName));
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="mediaBinaryStream"></param>
		/// <param name="fileName"></param>
		/// <param name="directoryPath"></param>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string directoryPath, EventMediaType mediaType)
		{
			string fname = await _fileManager.SaveFileAsync(mediaBinaryStream, fileName, directoryPath);
			Media media = new()
			{
				PhysicalPath = fname,
				MediaType = mediaType,
				Name = fileName,
			};
			return CreateAndReturnEntity<Media, Media>(media);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="media"></param>
		/// <returns></returns>
		public Task DeleteMediaAsync(Media media)
		{
			return DeleteMediaAsync(media.PhysicalPath);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public Task DeleteMediaAsync(string fileName)
		{
			return _fileManager.DeleteFileAsync(fileName);
		}
	}
}
