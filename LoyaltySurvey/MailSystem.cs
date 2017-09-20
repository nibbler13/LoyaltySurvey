using System.Net.Mail;
using System;
using System.Reflection;

namespace LoyaltySurvey {
	public class MailSystem {
		public static void SendMail (string subject, string body, string receiver) {
			LoggingSystem.LogMessageToFile("Отправка сообщения, тема: " + subject + ", текст: " + body);

			try {
				string appName = Assembly.GetExecutingAssembly().GetName().Name;

				MailAddress from = new MailAddress(
					Properties.Settings.Default.MailUser + "@" + 
					Properties.Settings.Default.MailDomain, appName);
				MailAddress to = new MailAddress(receiver);
				
				body += Environment.NewLine + Environment.NewLine + 
					"Это автоматически сгенерированное сообщение" + Environment.NewLine + 
					"Просьба не отвечать на него" + Environment.NewLine +
 					"Имя системы: " + Environment.MachineName;

				MailMessage message = new MailMessage(from, to);
				message.Subject = subject;
				message.Body = body;
				if (!string.IsNullOrEmpty(Properties.Settings.Default.MailCopy))
					message.CC.Add(Properties.Settings.Default.MailCopy);

				SmtpClient client = new SmtpClient(Properties.Settings.Default.MailSmtpServer, 25);
				client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential(
					Properties.Settings.Default.MailUser,
					Properties.Settings.Default.MailPassword,
					Properties.Settings.Default.MailDomain);

				client.SendCompleted += (s, e) => {
					client.Dispose();
					message.Dispose();
					LoggingSystem.LogMessageToFile("Письмо отправлено успешно");
				};

				client.SendAsync(message, null);
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("SendMail exception: " + e.Message + Environment.NewLine + e.StackTrace);
			}
		}
	}
}
