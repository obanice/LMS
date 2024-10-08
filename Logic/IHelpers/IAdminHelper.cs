﻿using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Logic.IHelpers
{
	public interface IAdminHelper
	{
		bool AddCourse(CourseViewModel courseViewModel);
		Task<ApplicationUser> AddLecturer(LecturerViewModel lecturerViewModel);
		bool AddMaterial(int? courseId, int? mediaId);
		IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, string lecturerId, int page);
		List<CourseViewModel> CoursesByDepartmentId(int? departmentId);
        bool DeleteMaterial(int? materialId);
        AdminDashboardViewModel FetchAdminData(int? departmentId);
		List<StudyMaterialViewModel> GetStudyMaterialsByCoursesById(int? courseId);
		IPagedList<LecturerViewModel> Lectures(IPageListModel<LecturerViewModel> model, int? departmentId, int page);
	}
}
