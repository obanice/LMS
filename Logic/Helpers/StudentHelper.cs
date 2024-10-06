using Core;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Logic.Helpers
{
	public interface IStudentHelper
	{
		IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, int? levelId, int page);
		IPagedList<QuizViewModel> FetchAllQuiz(IPageListModel<QuizViewModel> model, int page, int? courseId);
		IPagedList<QuizAnswersViewModel> FetchQuizAnswersByStudentId(string loggedInUserId, IPageListModel<QuizAnswersViewModel> model, int page);
        StudentDashboardViewModel StudentDashboard(int departmentId);
		List<StudyMaterialViewModel> GetStudyMaterials(int? departmentId, int? courseId);

        bool UploadAnswer(QuizAnswersDTO quizAnswers);
        List<QuizViewModel> GetQuizByDepartmentId(int? departmentId);
    }

	public class StudentHelper(AppDbContext dbContext) : BaseHelper(dbContext), IStudentHelper
    {
		private readonly AppDbContext db = dbContext;

		public StudentDashboardViewModel StudentDashboard(int departmentId)
		{
			var studentDashboardViewModel = new StudentDashboardViewModel();

			studentDashboardViewModel.StudyMaterials = GetStudyMaterials(departmentId, 0);
			studentDashboardViewModel.QuizViewModel = GetQuizByDepartmentId(departmentId);

            return studentDashboardViewModel;
		}
        public List<QuizViewModel> GetQuizByDepartmentId(int? departmentId)
        {

            return [.. db.Quiz.Include(x => x.Course)
				.Include(x=>x.Question)
				.Where(x => x.Active && x.Course != null && x.Course.DepartmentId == departmentId)
				.Select(v => new QuizViewModel
                {
                    Id = v.Id,
					QuestionFile = v.Question.PhysicalPath,
                    CourseId = v.CourseId,
                    DateCreated = v.DateCreated.ToFormattedDate(),
					CourseCode = v.Course.Code,
					Lecturer = $"{v.Course.Lecturer.FirstName} {v.Course.Lecturer.LastName}",
				})];
        }

        public List<StudyMaterialViewModel> GetStudyMaterials(int? departmentId, int? courseId)
        {

			var studyMaterials = db.StudyMaterials
				.Where(x => x.Active && x.Course != null && x.Course.DepartmentId == departmentId)
				.Include(x => x.MediaType)
				.Include(x => x.Course)
				.ThenInclude(x => x.Lecturer).ToList();
			if (studyMaterials.Any())
			{
				if (courseId != null && courseId != 0)
				{
					studyMaterials = studyMaterials.Where(c => c.CourseId == courseId).ToList();

				}
			}

			return studyMaterials.Select(v => new StudyMaterialViewModel
			{
				Id = v.Id,
				CourseId = v.CourseId,
				Name = v.MediaType.Name,
				LecturerName = $"{v.Course.Lecturer.FirstName} {v.Course.Lecturer.LastName}",
				Code = v.Course.Code,
				File = v.MediaType.PhysicalPath,
				DateCreated = v.DateCreated.ToFormattedDate(),
				FileExtension = v.MediaType.MediaType.GetEnumDescription()
			}).ToList();
		}

        public IPagedList<QuizViewModel> FetchAllQuiz(IPageListModel<QuizViewModel> model, int page, int? courseId)
		{
			var user = Utility.GetCurrentUser();
			IQueryable<Quiz> query = GetByPredicate<Quiz>(p => p.Active);

			if (!query.Any())
			{
				return new List<QuizViewModel>().ToPagedList(page, 25);
			}
			query = db.Quiz.
				Include(x => x.Question)
				.Include(x => x.Course)
				.Include(x => x.Answers) 
				.Where(p => p.Active && p.Course.DepartmentId == user.DepartmentId && p.Course.LevelId == user.LevelId)
				.AsQueryable();

			if (courseId.HasValue && courseId != 0)
			{
				query = query.Where(p => p.CourseId == courseId);
			}
			if (!string.IsNullOrEmpty(model.Keyword))
			{
				var keyword = model.Keyword.ToLower();
				query = query.Where(v =>
						v.Course.Code.ToLower().Contains(keyword) ||
						v.Question.Name.ToLower().Contains(keyword)
				);
			}

			if (model.StartDate.HasValue)
			{
				query = query.Where(v => v.DateCreated >= model.StartDate);
			}

			if (model.EndDate.HasValue)
			{
				query = query.Where(v => v.DateCreated <= model.EndDate);
			}

			var quiz = query.OrderByDescending(v => v.DateCreated)
							   .Select(v => new QuizViewModel
							   {
								   Id = v.Id,
								   CourseCode = v.Course.Code,
								   QuestionFile = v.Question.PhysicalPath,
								   DateCreated = v.DateCreated.ToFormattedDate()
							   })
							   .ToPagedList(page, 25);

			model.Model = quiz;

			return quiz;
		}

		public IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, int? levelId, int page)
		{
			IQueryable<Course> query = GetByPredicate<Course>(p => p.Active);

			if (departmentId.HasValue && departmentId != 0)
			{
				query = query.Where(p => p.DepartmentId == departmentId && p.LevelId == levelId);
			}

			if (!string.IsNullOrEmpty(model.Keyword))
			{
				var keyword = model.Keyword.ToLower();
				query = query.Where(v =>
						v.Code.ToLower().Contains(keyword) ||
						v.Name.ToLower().Contains(keyword) ||
						v.Description.ToLower().Contains(keyword) ||
						v.Lecturer.FirstName.ToLower().Contains(keyword) ||
						v.Department.Name.ToLower().Contains(keyword));
			}

			if (model.StartDate.HasValue)
			{
				query = query.Where(v => v.DateCreated >= model.StartDate);
			}

			if (model.EndDate.HasValue)
			{
				query = query.Where(v => v.DateCreated <= model.EndDate);
			}

			query = query.Include(x => x.Lecturer)
						 .Include(x => x.Department)
						 .Include(x => x.Semester)
						 .Include(x => x.Level)
						 .Include(x => x.Quizzes);

			if (!query.Any())
			{
				return new List<CourseViewModel>().ToPagedList(page, 25);
			}

			var courses = query.OrderByDescending(v => v.DateCreated)
							   .Select(v => new CourseViewModel
							   {
								   Id = v.Id,
								   Name = v.Name,
								   Code = v.Code,
								   Description = v.Description,
								   LecturerName = v.Lecturer.FullName,
								   Department = v.Department.Name,
								   Semester = v.Semester.Name,
								   Level = v.Level.Name
							   })
							   .ToPagedList(page, 25);

			model.Model = courses;

			return courses;
		}
		public bool UploadAnswer(QuizAnswersDTO quizAnswers)
		{
			return Create<QuizAnswersDTO, QuizAnswers>(quizAnswers);
		}
		public IPagedList<QuizAnswersViewModel> FetchQuizAnswersByStudentId(string loggedInUserId, IPageListModel<QuizAnswersViewModel> model, int page)
		{

			var query = GetByPredicate<QuizAnswers>(p => p.Active && p.StudentId == loggedInUserId)
				.Include(x => x.Quiz)
				.ThenInclude(x => x.Course)
				.Include(x => x.Answer)
				.AsQueryable();

			if (!query.Any())
			{
				return new List<QuizAnswersViewModel>().ToPagedList(page, 25);
			}
			if (!string.IsNullOrEmpty(model.Keyword))
			{
				var keyword = model.Keyword.ToLower();
				query = query.Where(v =>
						v.Mark.ToString().ToLower().Contains(keyword) ||
						v.Quiz.Course.Code.ToLower().Contains(keyword)
				);
			}

			var quiz = query.OrderByDescending(v => v.DateSubmitted)
							   .Select(v => new QuizAnswersViewModel
							   {
								   Id = v.Id,
								   CourseCode = v.Quiz.Course.Code,
								   AnswerFile = v.Answer.PhysicalPath,
								   Mark = v.Mark
							   })
							   .ToPagedList(page, 25);

			model.Model = quiz;

			return quiz;
		}
	}
}
