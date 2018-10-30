using LoyaltySurvey.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application {
		private void Application_Startup(object sender, StartupEventArgs e) {
			DispatcherUnhandledException += App_DispatcherUnhandledException;

			MainWindow window = new MainWindow();
			window.Show();
		}

		private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			SystemLogging.ToLog(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
			SystemMail.SendMail(
				"Необработанное исключение", 
				e.Exception.Message + Environment.NewLine + e.Exception.StackTrace, 
				Settings.Default.MailCopy);
			SystemLogging.ToLog("!!!App - Аварийное завершение работы");
			Process.GetCurrentProcess().Kill();
		}
	}
}
