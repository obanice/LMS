using Core;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using X.PagedList;

namespace Logic.Helpers
{

    public class LecturerHelper(AppDbContext db) : BaseHelper(db), ILecturerHelper
    {
        private  readonly AppDbContext db = db;

        public bool CreateStudyMaterial(StudyMaterialViewModel studyMaterial)
        {
            return Create<StudyMaterialViewModel, StudyMaterial>(studyMaterial);
        }
        public IPagedList<QuizViewModel> FetchQuizByLecturerId(int? courseId, IPageListModel<QuizViewModel> model, int page, string loggedInUserId)
        {
            var query = GetByPredicate<Quiz>(p => p.Active && p.LecturerId == loggedInUserId)
                .Include(x => x.Question)
                .Include(x => x.Course)
                .Include(x => x.Answers)
                .AsQueryable();

            if (courseId.HasValue && courseId != 0)
            {
                query = query.Where(p => p.CourseId == courseId);
            }
            if (!query.Any())
            {
                return new List<QuizViewModel>().ToPagedList(page, 25);
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
        public bool AddQuiz(QuizDTO quizViewModel)
        {
            return Create<QuizDTO, Quiz>(quizViewModel);
        }
        public IPagedList<QuizAnswersViewModel> FetchQuizAnswersByQuizId(int? quizId, IPageListModel<QuizAnswersViewModel> model, int page)
        {
            var query = GetByPredicate<QuizAnswers>(p => p.Active && p.QuizId == quizId)
                .Include(x => x.Quiz)
                .ThenInclude(x => x.Course)
                .Include(x => x.Student)
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
                        v.Student.FirstName.ToLower().Contains(keyword) ||
                        v.Student.LastName.ToLower().Contains(keyword) ||
                        v.Mark.ToString().ToLower().Contains(keyword) ||
                        v.Quiz.Course.Code.ToLower().Contains(keyword)
                );
            }

            if (model.StartDate.HasValue)
            {
                query = query.Where(v => v.DateSubmitted >= model.StartDate);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(v => v.DateSubmitted <= model.EndDate);
            }



            var quiz = query.OrderByDescending(v => v.DateSubmitted)
                               .Select(v => new QuizAnswersViewModel
                               {
                                   Id = v.Id,
                                   CourseCode = v.Quiz.Course.Code,
                                   AnswerFile = v.Answer.PhysicalPath,
                                   StudentFullName = v.Student.FullName,
                                   DateSubmitted = v.DateSubmitted.ToFormattedDate(),
                                   Mark = v.Mark
                               })
                               .ToPagedList(page, 25);

            model.Model = quiz;

            return quiz;
        }
        public QuizAnswers? AddScoreToQuiz(int? quizId, decimal? mark)
        {
            var quizAnswer = GetByPredicate<QuizAnswers>(x => x.QuizId == quizId && x.Active).FirstOrDefault();
            if (quizAnswer == null)
            {
                return null;
            }
            return Update<QuizAnswers, int>("Mark", mark, quizAnswer.Id);
        }
        public LectuereDashboardViewModel GetLecturersDashboardInfo(string userId)
        {
            var lectuereDashboardViewModel = new LectuereDashboardViewModel();

            var courses = GetByPredicate<Course>(c => c.LecturerId == userId && c.Active)
                .Include(t => t.Level)
                .Include(t => t.Semester)
                .Select(c => new CourseViewModel
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Level = c.Level.Name,
                    Semester = c.Semester.Name,

                }).ToList();
            var quizAnwers = db.QuizAnswers
               .Include(t => t.Answer)
               .Include(t => t.Student)
               .Include(t => t.Quiz)
               .ThenInclude(x=>x.Course)
               .Where(x=>x.Quiz.LecturerId == userId && x.Active)
               .Select(c => new QuizAnswersViewModel
                {
                    Id = c.Id,
                    StudentFullName = c.Student.FirstName,
                    AnswerFile = c.Answer.PhysicalPath,
                    CourseCode = c.Quiz.Course.Code,
                    DateSubmitted = c.DateSubmitted.ToFormattedDate(),

                }).ToList();
            lectuereDashboardViewModel.CourseViewModels = courses;
            lectuereDashboardViewModel.QuizAnswers = quizAnwers;
            return lectuereDashboardViewModel;
        }
    }
}
