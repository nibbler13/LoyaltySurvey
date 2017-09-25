﻿using System;
using System.IO;
using System.Linq;

namespace LoyaltySurvey {
	class LoggingSystem {
		private const string LOG_FILE_NAME = "LoyaltySurvey_*.log";
		private const int MAX_LOGFILES_QUANTITY = 7;

		public static void LogMessageToFile(string msg) {
			string today = DateTime.Now.ToString("yyyyMMdd");
			string logFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + LOG_FILE_NAME.Replace("*", today);

			try {
				using (System.IO.StreamWriter sw = System.IO.File.AppendText(logFileName)) {
					string logLine = System.String.Format("{0:G}: {1}", System.DateTime.Now, msg);
					sw.WriteLine(logLine);
				}
			} catch (Exception e) {
				Console.WriteLine("LogMessageToFile exception: " + logFileName + Environment.NewLine + e.Message + 
					Environment.NewLine + e.StackTrace);
			}

			Console.WriteLine(msg);
			CheckAndCleanOldFiles();
		}

		private static void CheckAndCleanOldFiles() {
			try {
				DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
				FileInfo[] files = dirInfo.GetFiles(LOG_FILE_NAME).OrderBy(p => p.CreationTime).ToArray();

				if (files.Length <= MAX_LOGFILES_QUANTITY)
					return;

				for (int i = 0; i < files.Length - MAX_LOGFILES_QUANTITY; i++)
					files[i].Delete();
			} catch (Exception e) {
				Console.WriteLine("CheckAndCleanOldFiles exception" + e.Message + Environment.NewLine + e.StackTrace);
			}
		}
	}
}
