using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public static class SystemNotification {
		public static void EmptyResults() {
			string subject = "Ошибка обработки данных";
			string body = 
				"Приложению " + Assembly.GetExecutingAssembly().GetName().Name + 
				" не удалось разобрать список докторов, полученный из базы ИК." +
				Environment.NewLine + "Произведен переход на страницу с ошибкой.";
			string receiver = Properties.Settings.Default.MailErrorsReceiverAddress;

			ClientMail.SendMail(subject, body, receiver);
		}

		public static void DataBaseEmptyResponse() {
			string subject = "Ошибка получения данных";
			string body =
				"Приложению " + Assembly.GetExecutingAssembly().GetName().Name + 
				" не удалось получить данные из базы ИК: " +
				Properties.Settings.Default.MisInfoclinicaDbAddress + ":" + 
				Properties.Settings.Default.MisInfoclinicaDbName +
				Environment.NewLine +
				"Запрос: " + Properties.Settings.Default.SqlQueryDoctors;
			string receiver = Properties.Settings.Default.MailErrorsReceiverAddress;

			ClientMail.SendMail(subject, body, receiver);
		}

		public static void AppStart() {
			string subject = "Запуск приложения";
			string body = "Приложение успешно запущено";
			string receiver = Properties.Settings.Default.MailCopy;

			ClientMail.SendMail(subject, body, receiver);
		}
		
		public static void NegativeMark(ItemSurveyResult surveyResult) {
			if (surveyResult == null)
				throw new ArgumentNullException(nameof(surveyResult));

			string header = "";

			if (surveyResult.PhoneNumber.Length == 10 &&
				surveyResult.PhoneNumber.StartsWith("9"))
				header = "Пациент указал, что ему можно позвонить для уточнения подробностей " +
				"о его негативной оценке.";
			else if (!string.IsNullOrEmpty(surveyResult.Comment) &&
				!string.IsNullOrWhiteSpace(surveyResult.Comment) &&
				!surveyResult.Comment.Equals("Refused"))
				header = "Пациент оставил комментарий к своей негативной оценке";
			
			if (string.IsNullOrEmpty(header)) {
				SystemLogging.ToLog("Пропуск отправки сообщения об обратной связи - " +
					"неверный формат номера телефона и отсутствует комментарий");
				return;
			}

			string subject = Properties.Settings.Default.ClinicName + " - обратная связь с пациентом через монитор лояльности";
			string body =
				header + "<br><br>" +
				"<table border=\"1\">" +
				"<tr><td>Сотрудник</td><td><b>" + surveyResult.DocName + "</b></td></tr>" +
				"<tr><td>Отделение</td><td><b>" + surveyResult.DocDepartment + "</b></td></tr>" +
				"<tr><td>Оценка</td><td><b>" + Pages.Helpers.PageControlsFactory.GetNameForRate(surveyResult.DocRate) + "</b></td></tr>" +
				"<tr><td>Комментарий</td><td><b>" +
				(surveyResult.Comment.Equals("Refused") ? "отказался" : surveyResult.Comment) + "</b></td></tr>" +
				"<tr><td>Номер телефона для связи</td><td><b>" +
				(surveyResult.PhoneNumber.Equals("Refused") ? "отказался" : surveyResult.PhoneNumber) + "</b></td></tr>" +
				"</table><br>";

			string receiver;
			switch (surveyResult.MarkType) {
				case ItemSurveyResult.Type.Doctor:
					receiver = Properties.Settings.Default.MailNegativeDoctorMarkReceiverAddress;
					break;
				case ItemSurveyResult.Type.Registry:
					receiver = Properties.Settings.Default.MailNegativeRegistyMarkReceiverAddress;
					break;
				default:
					receiver = Properties.Settings.Default.MailCopy;
					break;
			}

			string attachmentPath = surveyResult.PhotoLink;

			if (File.Exists(attachmentPath))
				body += "Фотография с камеры терминала:";
			else
				body += "Фотография отсутствует";
			body += "</b>";

			ClientMail.SendMail(subject, body, receiver, attachmentPath);
		}

		public static void DoctorsPhotoPathError() {
			string subject = "Ошибка обработки фотографий докторов";
			string body = "Папка с фотографиями " + Properties.Settings.Default.PathDoctorsPhotoSource + 
				" не существует, или к ней нет доступа";
			string receiver = Properties.Settings.Default.MailErrorsReceiverAddress;

			ClientMail.SendMail(subject, body, receiver);
		}

		public static void DoctorsPhotoMissed(List<string> missedPhotos) {
			if (missedPhotos == null)
				throw new ArgumentNullException(nameof(missedPhotos));

			string subject = "Отсутствуют фотографии докторов";
			string body = "Приложению " + Assembly.GetExecutingAssembly().GetName().Name + " (" + Environment.MachineName +
				") не удалось найти фотографии в папке '" + Properties.Settings.Default.PathDoctorsPhotoSource +
				"' для сотрудников перечисленных ниже." + Environment.NewLine;

			string receiver = Properties.Settings.Default.MailMissedPhotosReceiverAddress;
			if (!receiver.ToLower().Contains("stp"))
				body = "На мониторе лояльности имеются сотрудники без фотографий. Просьба сфотографировать указанных " +
					"сотрудников на светлом ровном фоне и отправить фотографии в службу технической поддержки для дальнейшей " +
					"обработки и добавления на монитор лояльности." + Environment.NewLine;

			body += Environment.NewLine;

			foreach (string missedPhoto in missedPhotos)
				body += missedPhoto + Environment.NewLine;

			if (!string.IsNullOrEmpty(receiver))
				ClientMail.SendMail(subject, body, receiver);

			SystemLogging.WriteStringToFile(body, Directory.GetCurrentDirectory() + "\\DoctorsPhotos\\MissedPhotos.txt");
		}
	}
}
