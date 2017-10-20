﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public class DataHandleSystem {
		public static Dictionary<string, List<Doctor>> GetDoctorsDictionary() {
			FBClient fbClient = new FBClient(
				Properties.Settings.Default.MisInfoclinicaDbAddress,
				Properties.Settings.Default.MisInfoclinicaDbName,
				Properties.Settings.Default.MisInfoclinicaDbUser,
				Properties.Settings.Default.MisInfoclinicaDbPassword);

			LoggingSystem.LogMessageToFile("Обновление данных из базы ИК");
			DataTable dataTable = fbClient.GetDataTable(Properties.Settings.Default.SqlQueryDoctors);

			Dictionary<string, List<Doctor>> dictionary = new Dictionary<string, List<Doctor>>();

			if (dataTable.Rows.Count == 0) {
				LoggingSystem.LogMessageToFile("Из базы ИК вернулась пустая таблица");
				NotificationSystem.DataBaseEmptyResponse();
			}

			int clinicRestriction = Properties.Settings.Default.ClinicRestrictions1AdultOnly2ChildOnly;
			string[] adultSections = { "стоматология", "медицина - взрослая" };
			string[] kidsSections = { "педиатрия", "пнд-педиатрия" };

			foreach (DataRow dataRow in dataTable.Rows) {
				try {
					string sectionName = dataRow["SECTIONNAME"].ToString().ToLower();

					if ((clinicRestriction == 1 && kidsSections.Contains(sectionName)) ||
						(clinicRestriction == 2 && adultSections.Contains(sectionName)))
						continue;

					string department = dataRow["DEPARTMENT"].ToString().ToLower();
					string docname = dataRow["DOCNAME"].ToString();
					string docposition = dataRow["DOCPOSITION"].ToString();
					string dcode = dataRow["DCODE"].ToString();
					string deptCode = dataRow["DEPNUM"].ToString();

					if (string.IsNullOrEmpty(department) ||
						string.IsNullOrEmpty(docname))
						continue;

					Doctor doctor = new Doctor(docname, docposition, department, dcode, deptCode);

					if (dictionary.ContainsKey(department)) {
						bool isAlreadyExist = false;

						foreach (Doctor existedDoctor in dictionary[department])
							if (existedDoctor.Name.ToLower().Equals(doctor.Name.ToLower())) {
								isAlreadyExist = true;
								break;
							}

						if (!isAlreadyExist)
							dictionary[department].Add(doctor);
					} else
						dictionary.Add(department, new List<Doctor>() { doctor });
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile("Не удалось обработать строку с данными: " + dataRow.ToString() + ", " + e.Message);
				}
			}

			LoggingSystem.LogMessageToFile("Обработано строк:" + dataTable.Rows.Count);

			return dictionary;
		}

		public static void UpdateDoctorsPhoto(Dictionary<string, List<Doctor>> departments) {
			LoggingSystem.LogMessageToFile("Обновление фотографий докторов");
			string searchPath = @Properties.Settings.Default.PathDoctorsPhotoSource;
			string destinationPath = Directory.GetCurrentDirectory() + "\\DoctorsPhotos\\";
			if (!Directory.Exists(destinationPath))
				Directory.CreateDirectory(destinationPath);

			if (!Directory.Exists(searchPath)) {
				NotificationSystem.DoctorsPhotoPathError();
				return;
			}

			string[] photos = Directory.GetFiles(searchPath, "*.jpg", SearchOption.AllDirectories);
			List<string> missedPhotos = new List<string>();
			foreach (KeyValuePair<string, List<Doctor>> department in departments)
				foreach (Doctor doctor in department.Value) {
					string photoLink = "";

					foreach (string photo in photos) {
						string fileName = Path.GetFileNameWithoutExtension(photo);
						if (!fileName.ToLower().Replace('ё', 'е').Replace("  ", " ").Contains(doctor.Name.ToLower().Replace('ё', 'е')))
							continue;

						photoLink = photo;
						string destFileName = destinationPath + doctor.Name + "@" + doctor.Code + ".jpg";
						
						try {
							if (File.Exists(destFileName) && File.GetLastWriteTime(destFileName).Equals(File.GetLastWriteTime(photo))) {
								LoggingSystem.LogMessageToFile("Пропуск копирования файла (скопирован ранее) " + photo);
								break;
							}

							File.Copy(photo, destFileName);
							LoggingSystem.LogMessageToFile("Копирование файла " + photo + " в файл " + destFileName);
							break;
						} catch (Exception e) {
							LoggingSystem.LogMessageToFile("UpdateDoctorsPhoto exception: " + e.Message +
								Environment.NewLine + e.StackTrace);
							photoLink = "";
						}
					}

					if (String.IsNullOrEmpty(photoLink))
						missedPhotos.Add(doctor.Name + " | " + doctor.Code + " | " + doctor.Department);
				}

			if (missedPhotos.Count == 0)
				return;

			missedPhotos.Sort();
			NotificationSystem.DoctorsPhotoMissed(missedPhotos);
		}
	}
}
