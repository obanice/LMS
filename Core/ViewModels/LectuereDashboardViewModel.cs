using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class LectuereDashboardViewModel
	{

		public List<CourseViewModel>? CourseViewModels { get; set; }
		public List<QuizAnswersViewModel>? QuizAnswers { get; set; }
    }
}
