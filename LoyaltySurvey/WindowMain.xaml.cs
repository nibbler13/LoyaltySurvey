using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : NavigationWindow {
		public List<string> previousRatesDcodes = new List<string>();
		public DateTime previousThankPageCloseTime = DateTime.Now;

		public MainWindow() {
			InitializeComponent();

			SystemLogging.ToLog("==================================" + 
				Environment.NewLine + "Создание основного окна");
			SystemNotification.AppStart();

			if (!Properties.Settings.Default.IsDebug &&
				!Environment.MachineName.Equals("MSSU-DEV")) {
				Topmost = true;
				Cursor = Cursors.None;
			}
		}

		private void NavigationWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape)) {
				SystemLogging.ToLog("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			}
		}
	}
}
