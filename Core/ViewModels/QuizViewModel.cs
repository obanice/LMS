﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class QuizViewModel
	{
        public int Id { get; set; }
        public string? CourseCode { get; set; }
        public string? QuestionFile { get; set; }
        public string? AnswerFile { get; set; }
        public string? DateCreated { get; set; }
		public int? CourseId { get; set; }
		public string? Lecturer { get; set; }
	}
	public class QuizDTO
	{
		public int? CourseId { get; set; }
		public int? QuestionId { get; set; }
		public string? LecturerId { get; set; }
	}
}
