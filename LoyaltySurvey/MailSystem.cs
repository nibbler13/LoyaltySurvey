using System.Net.Mail;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace LoyaltySurvey {
	public class MailSystem {
		public static void SendMail (string subject, string body, string receiver, string attachmentPath = "") {
			LoggingSystem.LogMessageToFile("Отправка сообщения, тема: " + subject + ", текст: " + body);

			if (Properties.Settings.Default.IsDebug ||
				string.IsNullOrEmpty(receiver))
				return;

			try {
				string appName = Assembly.GetExecutingAssembly().GetName().Name;

				MailAddress from = new MailAddress(
					Properties.Settings.Default.MailUser + "@" + 
					Properties.Settings.Default.MailDomain, appName);

				List<MailAddress> mailAddressesTo = new List<MailAddress>();

				if (receiver.Contains(";")) {
					string[] receivers = receiver.Split(';');
					foreach (string address in receivers)
						mailAddressesTo.Add(new MailAddress(address));
				} else
					mailAddressesTo.Add(new MailAddress(receiver));
				
				body += Environment.NewLine + Environment.NewLine + 
					"Это автоматически сгенерированное сообщение" + Environment.NewLine + 
					"Просьба не отвечать на него" + Environment.NewLine +
 					"Имя системы: " + Environment.MachineName;

				MailMessage message = new MailMessage();

				foreach (MailAddress mailAddress in mailAddressesTo)
					message.To.Add(mailAddress);

				message.From = from;
				message.Subject = subject;
				message.Body = body;
				if (!string.IsNullOrEmpty(Properties.Settings.Default.MailCopy))
					message.CC.Add(Properties.Settings.Default.MailCopy);

				if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath)) {
					Attachment attachment = new Attachment(attachmentPath);
					message.Attachments.Add(attachment);
				}

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
