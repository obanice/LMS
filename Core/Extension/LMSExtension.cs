using System.ComponentModel;
using System.Reflection;
using static Core.Enums.LMSEnum;

namespace Core
{
    public static class LMSExtension
    {
        public static string EnumToString(this Enum value)
        {
            return nameof(value);
        }
		public static string? ToFormattedDate(this DateTime? value)
		{
			return value?.ToString("dd/MM/yyyy");
		}
		public static string ToFormattedTime(this DateTime value)
		{
			return value.ToString("t");
		}
        public static string ToFormattedTimeSpan(this TimeSpan value)
        {
            DateTime dateTime = DateTime.Today.Add(value);

            return dateTime.ToString("h:mm tt");
        }

        public static readonly Dictionary<string, EventMediaType> MediaTypes = new()
        {
            // image files
            { ".art", EventMediaType.Image }, // image/x-jg
            { ".bm", EventMediaType.Image }, // image/bmp
            { ".bmp", EventMediaType.Image }, // image/bmp, image/x-windows-bmp
            { ".dwg", EventMediaType.Image }, // image/vnd.dwg, image/x-dwg
            { ".dxf", EventMediaType.Image }, // image/vnd.dwg, image/x-dwg
            { ".fif", EventMediaType.Image }, // image/fif
            { ".flo", EventMediaType.Image }, // image/florian
            { ".fpx", EventMediaType.Image }, // image/vnd.fpx
            { ".g3", EventMediaType.Image }, // image/g3fax
            { ".gif", EventMediaType.Image }, // image/gif
            { ".ico", EventMediaType.Image }, // image/x-icon
            { ".ief", EventMediaType.Image }, // image/ief
            { ".iefs", EventMediaType.Image }, // image/ief
            { ".jfif", EventMediaType.Image }, // image/jpeg, image/pjpeg
            { ".jfif-tbnl", EventMediaType.Image }, // image/jpeg
            { ".jpe", EventMediaType.Image }, // image/jpeg, image/pjpeg
            { ".jpeg", EventMediaType.Image }, // image/jpeg, image/pjpeg
            { ".jpg", EventMediaType.Image }, // image/jpeg, image/pjpeg
            { ".jps", EventMediaType.Image }, // image/x-jps
            { ".jut", EventMediaType.Image }, // image/jutvision
            { ".mcf", EventMediaType.Image }, // image/vasa
            { ".nap", EventMediaType.Image }, // image/naplps
            { ".naplps", EventMediaType.Image }, // image/naplps
            { ".nif", EventMediaType.Image }, // image/x-niff
            { ".niff", EventMediaType.Image }, // image/x-niff
            { ".pbm", EventMediaType.Image }, // image/x-portable-bitmap
            { ".pct", EventMediaType.Image }, // image/x-pict
            { ".pcx", EventMediaType.Image }, // image/x-pcx
            { ".pgm", EventMediaType.Image }, // image/x-portable-graymap, image/x-portable-greymap
            { ".pic", EventMediaType.Image }, // image/pict
            { ".pict", EventMediaType.Image }, // image/pict
            { ".pm", EventMediaType.Image }, // image/x-xpixmap
            { ".png", EventMediaType.Image }, // image/png
            { ".pnm", EventMediaType.Image }, // image/x-portable-anymap
            { ".ppm", EventMediaType.Image }, // image/x-portable-pixmap
            { ".qif", EventMediaType.Image }, // image/x-quicktime
            { ".qti", EventMediaType.Image }, // image/x-quicktime
            { ".qtif", EventMediaType.Image }, // image/x-quicktime
            { ".ras", EventMediaType.Image }, // image/cmu-raster, image/x-cmu-raster
            { ".rast", EventMediaType.Image }, // image/cmu-raster
            { ".rf", EventMediaType.Image }, // image/vnd.rn-realflash
            { ".rgb", EventMediaType.Image }, // image/x-rgb
            { ".rp", EventMediaType.Image }, // image/vnd.rn-realpix
            { ".svf", EventMediaType.Image }, // image/vnd.dwg, image/x-dwg
            { ".svg", EventMediaType.Image }, // image/vnd.svg, image/x-dwg
            { ".tif", EventMediaType.Image }, // image/tiff, image/x-tiff
            { ".tiff", EventMediaType.Image }, // image/tiff, image/x-tiff
            { ".turbot", EventMediaType.Image }, // image/florian
            { ".wbmp", EventMediaType.Image }, // image/vnd.wap.wbmp
            { ".webp", EventMediaType.Image }, // image/webp
            { ".xbm", EventMediaType.Image }, // image/x-xbitmap, image/x-xbm, image/xbm
            { ".xif", EventMediaType.Image }, // image/vnd.xiff
            { ".xpm", EventMediaType.Image }, // image/x-xpixmap, image/xpm
            { ".ax-pngrt", EventMediaType.Image }, // image/png
            { ".xwd", EventMediaType.Image }, // image/x-xwd, image/x-xwindowdump
            
            // music extensions
            { ".mp3", EventMediaType.Music },

            //video extensions
            { ".mp4", EventMediaType.Video },
            { ".avi", EventMediaType.Video },

            { ".pdf", EventMediaType.Pdf },
           
            // word document extensions
            { ".doc", EventMediaType.Document },
            { ".docx", EventMediaType.Document },

            { ".exe", EventMediaType.Application },
            
            // presentation files
            { ".ppt", EventMediaType.Presentation },
            
            //spreadsheet files
            { ".xls", EventMediaType.SpreadSheet },
            { ".xlsx", EventMediaType.SpreadSheet },
            { ".csv", EventMediaType.SpreadSheet },
            
            // plain text files
            { ".txt", EventMediaType.Text }
        };
		public static string GetEnumDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
			return attribute == null ? value.ToString() : attribute.Description;
		}
	}

}
