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
		[Key]
		public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? QuestionId { get; set; }
		[ForeignKey(nameof(QuestionId))]
		public virtual Media? Question { get; set; }
		public int? AnswerId { get; set; }
		[ForeignKey(nameof(AnswerId))]
		public virtual Media? Answer { get; set; }
		public int? CourseId { get; set; }
		[ForeignKey(nameof(CourseId))]
		public virtual Course? Course { get; set; }
	}
}
