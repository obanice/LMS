using System.ComponentModel;

namespace Core.Enum
{
    public class LMSEnum
    {
        public enum Levels
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
        }
        public enum Semester
        {
            Rain = 1,
            Harmatan = 2,
        }
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
	}
}
