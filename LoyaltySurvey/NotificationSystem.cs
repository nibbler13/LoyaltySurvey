using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public class NotificationSystem {
		public static void EmptyResults() {
			string subject = "Ошибка обработки данных";
			string body = 
				"Приложению " + Assembly.GetExecutingAssembly().GetName().Name + 
				" не удалось разобрать список докторов, полученный из базы ИК." +
				Environment.NewLine + "Произведен переход на страницу с ошибкой.";
			string receiver = Properties.Settings.Default.MailTo;

			MailSystem.SendMail(subject, body, receiver);
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
			string receiver = Properties.Settings.Default.MailTo;

			MailSystem.SendMail(subject, body, receiver);
		}

		public static void AppStart() {
			string subject = "Запуск приложение";
			string body = "Приложение успешно запущено";
			string receiver = Properties.Settings.Default.MailCopy;

			MailSystem.SendMail(subject, body, receiver);
		}

		public static void CallbackAccepted(SurveyResult surveyResult) {
			string subject = Properties.Settings.Default.ClinicName + " - обратная связь с пациентом через монитор лояльности";
			string body = 
				"Пациент указал, что ему можно позвонить для уточнения подробностей " +
				"о его негативной оценке качества приема у врача." + 
				Environment.NewLine + Environment.NewLine +
				"Врач: " + surveyResult.DocName + Environment.NewLine +
				"Оценка качества приема: " + surveyResult.DocRate + Environment.NewLine +
				"Комментарий: " +
				(surveyResult.Comment.Equals("Refused") ? "отказался" : surveyResult.Comment) + Environment.NewLine +
				"Номер телефона для связи: " + surveyResult.PhoneNumber + Environment.NewLine + Environment.NewLine +
				"Фотография с камеры терминала во вложении";
			string receiver = Properties.Settings.Default.MailCallbackTo;
			string attachmentPath = surveyResult.PhotoLink;

			MailSystem.SendMail(subject, body, receiver, attachmentPath);
		}

		public static void DoctorsPhotoPathError() {
			string subject = "Ошибка обработки фотографий докторов";
			string body = "Папка с фотографиями " + Properties.Settings.Default.DoctorsPhotoPath + 
				" не существует, или к ней нет доступа";
			string receiver = Properties.Settings.Default.MailTo;

			MailSystem.SendMail(subject, body, receiver);
		}

		public static void DoctorsPhotoMissed(List<string> missedPhotos) {
			string subject = "Отсутствуют фотографии докторов";
			string body = "Не удалось найти фотографии для следующих докторов: " + Environment.NewLine + Environment.NewLine;
			foreach (string missedPhoto in missedPhotos)
				body += missedPhoto + Environment.NewLine;
			string receiver = Properties.Settings.Default.MailMissedPhotosTo;

			MailSystem.SendMail(subject, body, receiver);
		}
	}
}
