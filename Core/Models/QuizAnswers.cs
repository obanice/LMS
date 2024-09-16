using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
	public class QuizAnswers
	{
		[Key]
		public int Id { get; set; }
		public bool Active { get; set; }
		public decimal? Mark { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public int? AnswerId { get; set; }
		[ForeignKey(nameof(AnswerId))]
		public virtual Media? Answer { get; set; }
		public int? QuizId { get; set; }
		[ForeignKey(nameof(QuizId))]
		public virtual Quiz? Quiz { get; set; }
		public string? StudentId { get; set; }
		[ForeignKey(nameof(StudentId))]
		public virtual ApplicationUser? Student { get; set; }
	}
}
