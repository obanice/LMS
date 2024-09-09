using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Enums.LMSEnum;

namespace Core.ViewModels
{
	namespace Core.ViewModels
	{
		public class ScreenRolesViewModel
		{
			public List<RoleViewModel> Roles { get; set; }
			public Guid? CompanyId { get; set; }
			public List<ScreenViewModel> Screens { get; set; }
		}
		public class ScreenViewModel
		{
			public string? Name { get; set; }
			public int? Id { get; set; }
			public string Class { get; set; }
			public string Url { get; set; }
			public bool ScreenChecked { get; set; }
			public string? RoleId { get; set; }
			public Guid? CompanyId { get; set; }
			public DateTime? DateCreated { get; set; }
			public ScreenGroupEnum? ScreenGroupEnumId { get; set; }
			public int? Priority { get; set; }
		}
		public class ScreenRoleUpdateModel
		{
			public int ScreenId { get; set; }
			public string RoleId { get; set; }
			public Guid? CompanyId { get; set; }
			public bool ScreenChecked { get; set; }
			public int? Priority { get; set; }
		}
	}
}
