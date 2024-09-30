using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class AdminDashboardViewModel
	{
		public List<CourseViewModel>? CourseViewModels { get; set; }
		public List<QuizAnswersViewModel>? QuizAnswers { get; set; }
		public List<QuizAnswersViewModel>? Students { get; set; }
        public int? StudentCount { get; set; }
        public int? LecturerCount { get; set; }
        public int? CoursesCount { get; set; }
    }
}
