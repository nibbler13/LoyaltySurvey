using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using LoyaltySurvey.Utilities;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : NavigationWindow {
		public List<string> PreviousRatesDcodes { get; } = new List<string>();
		public DateTime PreviousThankPageCloseTime { get; set; } = DateTime.Now;

		public MainWindow() {
			InitializeComponent();

			Logging.ToLog("==================================" + 
				Environment.NewLine + "Создание основного окна");
			Notification.AppStart();

			if (!Properties.Settings.Default.IsDebug &&
				!Environment.MachineName.Equals("MSSU-DEV")) {
				Topmost = true;
				Cursor = Cursors.None;
			}
		}

		private void NavigationWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape)) {
				Logging.ToLog("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			}
		}
	}
}
