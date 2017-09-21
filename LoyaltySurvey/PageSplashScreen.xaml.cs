using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageSplashScreen.xaml
	/// </summary>
	public partial class PageSplashScreen : ClassPageTemplate {
		private Dictionary<string, List<Doctor>> dictionaryOfDoctors = new Dictionary<string, List<Doctor>>();
		private BackgroundWorker backgroundWorker;
		private PageDepartmentSelect pageDepartmentSelect;

		public PageSplashScreen() {
			InitializeComponent();

			KeepAlive = true;

			Rect rect = CreateFirstOrLastPageControls(
				Properties.Resources.StringPageSplashScreenTitleLeftTop,
				Properties.Resources.StringPageSplashScreenTitleLeftBottom,
				Properties.Resources.StringPageSplashScreenTitleRight,
				Properties.Resources.StringPageSplashScreenSubtitle,
				true);

			double mediaElementWidth = rect.Width;
			double mediaElementHeight = rect.Height;
			MediaElement mediaElement = new MediaElement();
			mediaElement.Source = new Uri(Directory.GetCurrentDirectory() + "\\" + Properties.Settings.Default.WelcomeAnimationFileName);
			mediaElement.Width = mediaElementWidth;
			mediaElement.Height = mediaElementHeight;
			mediaElement.Stretch = Stretch.Uniform;
			Canvas.SetLeft(mediaElement, rect.Location.X);
			Canvas.SetTop(mediaElement, rect.Location.Y);
			CanvasMain.Children.Add(mediaElement);
			mediaElement.UnloadedBehavior = MediaState.Manual;
			mediaElement.MediaEnded += MediaElement_MediaEnded;

			//need to work previewmouseleftbutton on full screen area
			ControlsFactory.CreateLabel("", Colors.Transparent, Colors.Transparent, FontFamily, FontSize, FontWeights.Normal, ScreenWidth, ScreenHeight, 0, 0, CanvasMain);

			PreviewMouseLeftButtonDown += PageSplashScreen_PreviewMouseDown;
			HideButtonBack();

			backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
			backgroundWorker.RunWorkerAsync();

			DisableTimer();
			DisableTimerResetByClick();

			MidnightNotifier.DayChanged += MidnightNotifier_DayChanged;
		}

		private void MidnightNotifier_DayChanged(object sender, EventArgs e) {
			backgroundWorker.RunWorkerAsync();
		}

		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			dictionaryOfDoctors = DataHandleSystem.GetDoctorsDictionary();
		}

		private void PageSplashScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);
				return;
			}
			
			NavigationService.Navigate(pageDepartmentSelect);
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				NotificationSystem.EmptyResults();
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);

				Timer timer = new Timer(60 * 60 * 1000);
				timer.Elapsed += Timer_Elapsed;
				timer.Start();
			} else {
				pageDepartmentSelect = new PageDepartmentSelect(dictionaryOfDoctors);
			}
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			backgroundWorker.RunWorkerAsync();
		}
	}
}
