using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : NavigationWindow {
		public MainWindow() {
			InitializeComponent();
			LoggingSystem.LogMessageToFile("==================================" + 
				Environment.NewLine + "Создание основного окна");
			MailSystem.SendMail("Запуск приложение", "Приложение успешно запущено", Properties.Settings.Default.MailCopy);
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
