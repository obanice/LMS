using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
	public class Quiz
	{
        public Quiz()
        {
			Active = true;
			DateCreated = DateTime.Now;
        }
        [Key]
		public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? QuestionId { get; set; }
		[ForeignKey(nameof(QuestionId))]
		public virtual Media? Question { get; set; }
		public int? CourseId { get; set; }
		[ForeignKey(nameof(CourseId))]
		public virtual Course? Course { get; set; }
		public string? LecturerId { get; set; }
		[ForeignKey(nameof(LecturerId))]
		public virtual ApplicationUser? Lecturer { get; set; }
		public ICollection<QuizAnswers> Answers { get; set; }
	}
}
