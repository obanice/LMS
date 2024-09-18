using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class QuizAnswersViewModel
    {
        public decimal? Mark { get; set; }
        public int? Id { get; set; }
        public string? StudentFullName { get; set; }
        public string? CourseCode { get; set; }
        public string? AnswerFile { get; set; }
        public string? DateSubmitted { get; set; }
    }
	public class QuizAnswersDTO
	{
		public int? AnswerId { get; set; }
		public int? QuizId { get; set; }
		public string? StudentId { get; set; }
	}
}
