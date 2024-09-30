using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class StudentDashboardViewModel
	{
		public List<StudyMaterialViewModel>? StudyMaterials { get; set; }
		public List<QuizViewModel>? QuizViewModel { get; set; }
	}
}
