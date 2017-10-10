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

			LoggingSystem.LogMessageToFile("==================================" + 
				Environment.NewLine + "Создание основного окна");
			NotificationSystem.AppStart();

			if (!Properties.Settings.Default.IsDebug) {
				Topmost = true;
				Cursor = Cursors.None;
			}
		}

		private void NavigationWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape)) {
				LoggingSystem.LogMessageToFile("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			}
		}
	}
}
