using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class QuizAnswersViewModel
    {
        public double? Mark { get; set; }
        public int? Id { get; set; }
        public string? StudentFullName { get; set; }
        public string? CourseCode { get; set; }
        public string? AnswerFile { get; set; }
        public string? DateSubmitted { get; set; }
    }
}
