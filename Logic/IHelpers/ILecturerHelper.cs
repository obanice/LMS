using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Logic.IHelpers
{
	public interface ILecturerHelper
	{
		bool AddQuiz(QuizDTO quizViewModel);
		bool CreateStudyMaterial(StudyMaterialViewModel studyMaterial);
		IPagedList<QuizAnswersViewModel> FetchQuizAnswersByQuizId(int? quizId, IPageListModel<QuizAnswersViewModel> model, int page);
		IPagedList<QuizViewModel> FetchQuizByLecturerId(int? courseId,IPageListModel<QuizViewModel> model, int page, string loggedInUserId);
	}
}
