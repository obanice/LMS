﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class StudyMaterialViewModel
	{
		public int Id { get; set; }
		public long? MediaTypeId { get; set; }
		public int? CourseId { get; set; }
		public string? CourseCode { get; set; }
		public string Date { get; set; }
		public string? File { get; set; }
		public string? FileExtension { get; set; }
	}
}
