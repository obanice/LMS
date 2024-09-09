using System.Collections.Generic;

namespace Logic.Services.Model
{
    public class FileDirectory
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public List<FileDirectory> Directories { get; set; } = new List<FileDirectory>();

        public List<MediaListItem> Files { get; set; } = new List<MediaListItem>();
    }
}
