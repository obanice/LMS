using Core.Db;
using Core.ViewModels.Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
	public class AdminHelper : IAdminHelper
	{
		private readonly AppDbContext db;
		public AdminHelper(AppDbContext dbContext)
		{
			dbContext = db;
		}

	}
}
