using System.Net.Mail;
using System;

namespace LoyaltySurvey {
	class SmsSystem {
		public static string SendMailToSmsGate (string subject, string body, bool stpFbClient = false) {
			LoggingSystem.LogMessageToFile("Отправка сообщения, тема: " + subject + ", текст: " + body);

			try {
				MailAddress from = new MailAddress(
					Properties.Settings.Default.MailUser + "@" + 
					Properties.Settings.Default.MailDomain, "TrueConfApiTest");
				MailAddress to = new MailAddress(Properties.Settings.Default.MailTo);

				if (stpFbClient) {
					to = new MailAddress("stp@bzklinika.ru");
					subject = "Ошибки в работе LoyaltyQuizWpf";
					body = "На группу поддержки бизнес-приложений" + Environment.NewLine +
						"Сервису LoyaltyQuizWpf не удалось корректно загрузить" +
						" данные с сервера @ в течение длительного периода" + Environment.NewLine +
						Environment.NewLine + "Это автоматически сгенерированное сообщение" +
						Environment.NewLine + "Просьба не отвечать на него" + Environment.NewLine +
 						"Имя системы: " + Environment.MachineName;
					body = body.Replace("@", "FireBird " + Properties.Settings.Default.MisInfoclinicaDbAddress + ":" +
						Properties.Settings.Default.MisInfoclinicaDbName);
				}
		
				using (MailMessage message = new MailMessage(from, to)) {
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

					client.Send(message);
					return "";
				}
			} catch (Exception e) {
				return e.Message + " " + e.StackTrace;
			}
		}
	}
}
