using System.ComponentModel;

namespace Core.Enums
{
    public class LMSEnum
    {
		public enum ScreenGroupEnum
		{
			[Description("Lecturer")]
			Lecturer = 1,
			[Description("Student")]
			Student = 2
		}
		public enum DropDownEnums
		{
			Gender = 1,
			Level = 2,
			Semester = 3,
		}
		public enum EventMediaType
		{
			Image = 1,
			Application = 5,
			Video = 10,
			Music = 15,
			Text = 20,
			Other = 25,
			Document = 30,
			Pdf = 35,
			SpreadSheet = 40,
			Presentation = 50,
			Null = 55

		}
	}
}
