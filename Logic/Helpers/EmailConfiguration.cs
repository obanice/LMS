﻿using Logic.IHelpers;

namespace Logic.Helpers
{
	public interface IEmailConfiguration
	{
		string SmtpServer { get; }

		int SmtpPort { get; }

		string SmtpUsername { get; set; }

		string SmtpPassword { get; set; }
	}
	public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }
    }
}
