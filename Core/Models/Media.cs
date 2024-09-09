using Core.Enums;
using Core.Models;
using static Core.Enums.LMSEnum;
namespace Core.Models
{
    public class Media : BaseModel
    {
        public string? PhysicalPath { get; set; }

        public EventMediaType MediaType { get; set; }
    }
}
