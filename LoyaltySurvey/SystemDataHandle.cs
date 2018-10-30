using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public class SystemDataHandle {
		public static Dictionary<string, List<ItemDoctor>> GetDoctorsDictionary() {
			string misDbAddress = Properties.Settings.Default.MisInfoclinicaDbAddress;
			string misDbName = Properties.Settings.Default.MisInfoclinicaDbName;
			string misDbUser = Properties.Settings.Default.MisInfoclinicaDbUser;
			string misDbPass = Properties.Settings.Default.MisInfoclinicaDbPassword;
			string sqlQueryDoctors = Properties.Settings.Default.SqlQueryDoctors;

			SystemFirebirdClient fbClient = new SystemFirebirdClient(
				misDbAddress,
				misDbName,
				misDbUser,
				misDbPass);

			SystemLogging.ToLog("Обновление данных из базы ИК");
			DataTable dataTable = fbClient.GetDataTable(Properties.Settings.Default.SqlQueryDoctors);

			Dictionary<string, List<ItemDoctor>> dictionary = new Dictionary<string, List<ItemDoctor>>();

			string misDbAddressPnd = Properties.Settings.Default.MisInfoclinicaDbAddressPnd;
			string misDbNamePnd = Properties.Settings.Default.MisInfoclinicaDbNamePnd;
			if (!string.IsNullOrEmpty(misDbAddressPnd) && 
				!string.IsNullOrEmpty(misDbNamePnd)) {
				SystemFirebirdClient fbClientPnd = new SystemFirebirdClient(
					misDbAddressPnd,
					misDbNamePnd,
					misDbUser,
					misDbPass);
				SystemLogging.ToLog("Обновление данных из базы ИК для ПНД");
				DataTable dataTablePnd = fbClientPnd.GetDataTable(sqlQueryDoctors);

				foreach (DataRow row in dataTablePnd.Rows) {
					if (!row["DEPNUM"].ToString().Equals("10029098"))
						continue;

					dataTable.ImportRow(row);
				}
			}

			if (dataTable.Rows.Count == 0) {
				SystemLogging.ToLog("Из базы ИК вернулась пустая таблица");
				SystemNotification.DataBaseEmptyResponse();
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

					ItemDoctor doctor = new ItemDoctor(docname, docposition, department, dcode, deptCode);

					Console.WriteLine("doctor: " + doctor.Name + ", " + doctor.Department);

					if (dictionary.ContainsKey(department)) {
						bool isAlreadyExist = false;

						foreach (ItemDoctor existedDoctor in dictionary[department])
							if (existedDoctor.Name.ToLower().Equals(doctor.Name.ToLower())) {
								isAlreadyExist = true;
								break;
							}

						if (!isAlreadyExist)
							dictionary[department].Add(doctor);
					} else
						dictionary.Add(department, new List<ItemDoctor>() { doctor });
				} catch (Exception e) {
					SystemLogging.ToLog("Не удалось обработать строку с данными: " + dataRow.ToString() + ", " + e.Message);
				}
			}

			SystemLogging.ToLog("Обработано строк:" + dataTable.Rows.Count);

			return dictionary;
		}

		public static void UpdateDoctorsPhoto(Dictionary<string, List<ItemDoctor>> departments) {
			SystemLogging.ToLog("Обновление фотографий докторов");
			string searchPath = @Properties.Settings.Default.PathDoctorsPhotoSource;
			string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "DoctorsPhotos");
			if (!Directory.Exists(destinationPath)) {
				try {
					Directory.CreateDirectory(destinationPath);
				} catch (Exception e) {
					SystemLogging.ToLog("SystemDataHandle - UpdateDoctorsPhoto - " +
						e.Message + Environment.NewLine + e.StackTrace);
					return;
				}
			}

			if (!Directory.Exists(searchPath)) {
				SystemNotification.DoctorsPhotoPathError();
				return;
			}

			string[] photos = Directory.GetFiles(searchPath, "*.jpg", SearchOption.AllDirectories);
			List<string> missedPhotos = new List<string>();
			foreach (KeyValuePair<string, List<ItemDoctor>> department in departments)
				foreach (ItemDoctor doctor in department.Value) {
					string photoLink = "";

					foreach (string photo in photos) {
						string fileName = Path.GetFileNameWithoutExtension(photo);
						if (!fileName.ToLower().Replace('ё', 'е').Replace("  ", " ").Contains(doctor.Name.ToLower().Replace('ё', 'е')))
							continue;

						photoLink = photo;
						string destFileName = Path.Combine(destinationPath, doctor.Name + "@" + doctor.Code + ".jpg");
						
						try {
							if (File.Exists(destFileName) && File.GetLastWriteTime(destFileName).Equals(File.GetLastWriteTime(photo))) {
								SystemLogging.ToLog("Пропуск копирования файла (скопирован ранее) " + photo);
								break;
							}

							File.Copy(photo, destFileName, true);
							SystemLogging.ToLog("Копирование файла " + photo + " в файл " + destFileName);
							break;
						} catch (Exception e) {
							SystemLogging.ToLog("UpdateDoctorsPhoto exception: " + e.Message +
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
			SystemNotification.DoctorsPhotoMissed(missedPhotos);
		}
	}
}
