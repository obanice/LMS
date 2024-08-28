﻿using System.ComponentModel;

namespace Core.Enum
{
    public class eLearningEnum
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
	}
}
