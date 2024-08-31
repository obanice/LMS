using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
	public class CommonDropDown : BaseModel
	{
		[Display(Name = "DropDown Key")]
		public int DropDownKey { get; set; }
	}
}
