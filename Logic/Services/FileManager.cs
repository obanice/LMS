using Logic.Services.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
	public interface IFileManager
	{
		void CreateFolder(string name, string baseFolderPath);
		Task DeleteFileAsync(string fileName);
		Task DeleteFileAsync(string fileName, string folderPath);
		void DeleteFolder(string path);
		Stream GetFile(string fileName, string folderPath);
		FileDirectory GetInfo(string folderPath = null);
		void RenameFolder(string name, string baseFolderPath);
		void RenameFolder(string name, string newName, string baseFolderPath);
		string ResolveFileName(string fileName, string folderPath, int? number);
		Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName);
		Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, string folderPath);
	}

	public class FileManager : IFileManager
	{
		private const string MediaRootFolder = "fileSystem";
		private readonly IServiceProvider _serviceProvider;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly char[] dirSeperators = new char[] { '/', '\\' };

		public FileManager(IServiceProvider serviceProvider,
			IWebHostEnvironment webHostEnvironment)
		{
			_serviceProvider = serviceProvider;
			_webHostEnvironment = webHostEnvironment;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="baseFolderPath"></param>
		public void CreateFolder(string name, string baseFolderPath)
		{
			DirectoryInfo folderInfo = new($"{_webHostEnvironment.WebRootPath}{baseFolderPath}");
			if (folderInfo.Exists)
			{
				folderInfo.CreateSubdirectory(name);
				return;
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="baseFolderPath"></param>
		public void RenameFolder(string name, string baseFolderPath)
		{
			var folderInfo = new DirectoryInfo($"{_webHostEnvironment.WebRootPath}/{baseFolderPath}/{name}");
			if (folderInfo.Exists)
			{
				//folderInfo.Rename(name);
				return;
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="path"></param>
		public void DeleteFolder(string path)
		{
			var folderInfo = new DirectoryInfo($"{_webHostEnvironment.WebRootPath}{path}");
			if (folderInfo.Exists)
			{
				try
				{
					folderInfo.Delete(false);
				}
				catch (IOException)
				{

				}
				return;
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public Task DeleteFileAsync(string fileName, string folderPath)
		{
			var filePath = _webHostEnvironment.WebRootPath + folderPath + "/" + fileName;
			if (File.Exists(filePath))
			{
				Task.Run(() => File.Delete(filePath));
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task DeleteFileAsync(string fileName)
		{
			var filePath = $"{_webHostEnvironment.WebRootPath}/{MediaRootFolder}/{fileName}";
			if (File.Exists(filePath))
			{
				await Task.Run(() => File.Delete(filePath));
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public FileDirectory GetInfo(string folderPath = null)
		{
			// we get HttpContext inside the method because, at the time this service is being
			// created, there's no request which means HttpContext will be null

			var accessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
			string _hostingRootUrl = $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host.Value}";
			FileDirectory directory = new();

			DirectoryInfo d = new(_webHostEnvironment.WebRootPath + folderPath);
			if (d.Exists)
			{
				directory.Directories = d.EnumerateDirectories().Select(x => new FileDirectory
				{
					FullPath = $"{folderPath}/{x.Name}",
					Name = x.Name
				}).ToList();

				directory.FullPath = folderPath;
				directory.Files = d.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => new MediaListItem
				{
					Name = x.Name,
					DateCreated = x.CreationTime,
					Url = $"{_hostingRootUrl}{folderPath}/{x.Name}"
				}).ToList();
			}
			else
			{
				DirectoryInfo dr = new(_webHostEnvironment.WebRootPath);
				directory.Directories = dr.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(x => new FileDirectory
				{
					FullPath = $"/{x.Name}",
					Name = x.Name
				}).ToList();

				directory.FullPath = "/";
				directory.Files = dr.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => new MediaListItem
				{
					Name = x.Name,
					DateCreated = x.CreationTime,
					Url = $"{_hostingRootUrl}/{x.Name}"
				}).OrderBy(x => x.DateCreated).ToList();
			}
			return directory;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public Stream GetFile(string fileName, string folderPath)
		{
			var filePath = $"{_webHostEnvironment.WebRootPath}{folderPath}/{fileName}";
			if (File.Exists(filePath))
			{
				return new StreamReader(filePath).BaseStream;
			}

			return null;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="mediaBinaryStream"></param>
		/// <param name="fileName"></param>
		/// <param name="mimeType"></param>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public async Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, string folderPath)
		{
			// prefer to save file with user's preferred filename
			// if filename isn't available, find a filename and save with then send
			fileName = ResolveFileName(fileName, folderPath, null);
			string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
			string pathString = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);
			if (!Directory.Exists(pathString))
			{
				Directory.CreateDirectory(pathString);
			}
			string filePath = Path.Combine(uploadsFolder, fileName);
			using (var output = new FileStream(filePath, FileMode.Create))
			{
				await mediaBinaryStream.CopyToAsync(output);
			}
			var accessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
			string _hostingRootUrl = $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host.Value}/";
			string f = $"{folderPath}/{fileName}";
			return $"{_hostingRootUrl}{f}";
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="mediaBinaryStream"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName)
		{
			return await SaveFileAsync(mediaBinaryStream, fileName, null);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="folderPath"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public string ResolveFileName(string fileName, string folderPath, int? number)
		{
			string[] dirs = folderPath?.Split(dirSeperators, StringSplitOptions.RemoveEmptyEntries);
			dirs ??= Array.Empty<string>();
			folderPath = $"/{string.Join('/', dirs)}/";

			var filePath = _webHostEnvironment.WebRootPath + $"{folderPath}{fileName}";
			if (File.Exists(filePath))
			{
				var fnt = Path.GetFileNameWithoutExtension(fileName);
				var ext = Path.GetExtension(fileName);
				if (number.HasValue)
					fileName = ResolveFileName($"{fnt}{number + 1}{ext}", folderPath, number + 1);
				else
					fileName = ResolveFileName($"{fnt}0{ext}", folderPath, 0);
				return fileName;
			}
			return fileName.Replace("//", "/");
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="newName"></param>
		/// <param name="baseFolderPath"></param>
		public void RenameFolder(string name, string newName, string baseFolderPath)
		{
			throw new NotImplementedException();
		}
	}
}
