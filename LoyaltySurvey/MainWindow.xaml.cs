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

			//Navigating += MainFrame_OnNavigating;
		}

		private void NavigationWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape))
				Application.Current.Shutdown();
		}

		//private void MainFrame_OnNavigating(object sender, NavigatingCancelEventArgs e) {
		//	DoubleAnimation doubleAnimation = new DoubleAnimation();
		//	doubleAnimation.Duration = TimeSpan.FromSeconds(1);
		//	doubleAnimation.From = 0;
		//	doubleAnimation.To = Height;
		//	(e.Content as Page).BeginAnimation(HeightProperty, doubleAnimation);
		//}
	}
}
