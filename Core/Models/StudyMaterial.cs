using Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
	public class StudyMaterial:BaseModel
    {
		public int? MediaTypeId { get; set; }
		[ForeignKey(nameof(MediaTypeId))]
		public virtual Media? MediaType { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }
    }
}
