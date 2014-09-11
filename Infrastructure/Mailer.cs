using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Nancy;
using System.Net.Mail;
using System.Net;

namespace SWPCCBilling2.Infrastructure
{

	public class Mailer
	{
		private readonly SettingsStore _settingsStore;

		public Mailer()
		{
			_settingsStore = SettingsStore.DefaultSettingsStore;
		}

		public bool SendSecretly(string subject, string htmlBody, string emailPassword, IList<string> emailTo)
		{
			return Send(subject, htmlBody, emailPassword, emailTo, true);
		}

		public bool Send(string subject, string htmlBody, string emailPassword, IList<string> emailTo, bool secretly = false)
		{
			bool result = true;

			try 
			{
				Settings settings = _settingsStore.Load();

				var client = new SmtpClient
				{
					Host = settings.EmailServer,
					Port = settings.EmailPort,
					DeliveryMethod = SmtpDeliveryMethod.Network,
				};

				if (settings.EmailSecure)
				{
					client.EnableSsl = true;
					client.UseDefaultCredentials = false;
					client.Credentials = new NetworkCredential(settings.EmailFrom, emailPassword);
				}

				var msg = new MailMessage
				{
					From = new MailAddress(settings.EmailFrom),
					Subject = subject
				};

				if (!secretly)
				{
					foreach (string to in emailTo)
						msg.To.Add(to);
				}
				else
				{
					msg.To.Add(settings.EmailFrom);

					foreach (string to in emailTo)
						msg.Bcc.Add(to);
				}

				msg.IsBodyHtml = true;
				msg.Body = htmlBody;

				client.Send(msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				result = false;
			}

			return result;
		}
	}
}
