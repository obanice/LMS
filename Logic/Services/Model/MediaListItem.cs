using System;

namespace Logic.Services.Model
{
    public class MediaListItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
