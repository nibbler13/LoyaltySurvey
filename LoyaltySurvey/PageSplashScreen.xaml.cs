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
		private BackgroundWorker backgroundWorkerUpdateData;
		private PageDepartmentSelect pageDepartmentSelect;
		private Timer timerUpdateData;

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

			PreviewMouseLeftButtonDown += PageSplashScreen_PreviewMouseDown;
			HideButtonBack();

			backgroundWorkerUpdateData = new BackgroundWorker();
			backgroundWorkerUpdateData.DoWork += BackgroundWorker_DoWork;
			backgroundWorkerUpdateData.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
			backgroundWorkerUpdateData.RunWorkerAsync(true);

			DisablePageAutoCloseTimer();
			DisablePageAutoCloseTimerResetByClick();

			TimerUpdateDataSetup();
		}

		private void TimerUpdateDataSetup() {
			DateTime nowTime = DateTime.Now;
			DateTime fireTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 7, 0, 0, 0);
			if (nowTime > fireTime)
				fireTime = fireTime.AddDays(1);

			double tickTime = (fireTime - nowTime).TotalMilliseconds;
			timerUpdateData = new Timer(tickTime);
			timerUpdateData.Elapsed += TimerUpdateData_Elapsed;
			timerUpdateData.Start();
		}

		private void TimerUpdateData_Elapsed(object sender, ElapsedEventArgs e) {
			timerUpdateData.Stop();
			backgroundWorkerUpdateData.RunWorkerAsync(false);
			TimerUpdateDataSetup();
		}

		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				dictionaryOfDoctors = DataHandleSystem.GetDoctorsDictionary();

				if ((bool)e.Argument == true &&
					Directory.GetFiles(Directory.GetCurrentDirectory() + "\\DoctorsPhotos\\", "*.jpg", SearchOption.AllDirectories).Length != 0) 
					return;

				DataHandleSystem.UpdateDoctorsPhoto(dictionaryOfDoctors);
			} catch (Exception exception) {
				LoggingSystem.LogMessageToFile("BackgroundWorker_DoWork exception: " + exception.Message +
					Environment.NewLine + exception.StackTrace);
			}
		}

		private void PageSplashScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);
				return;
			}

			TimeSpan timeSpanTimeAfterClosing = DateTime.Now - ((MainWindow)Application.Current.MainWindow).previousThankPageCloseTime;
			if (timeSpanTimeAfterClosing.TotalSeconds > Properties.Settings.Default.PageAutocloseTimeoutInSeconds / 2)
				((MainWindow)Application.Current.MainWindow).previousRatesDcodes.Clear();

			NavigationService.Navigate(pageDepartmentSelect);
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			Application.Current.Dispatcher.Invoke(new Action(() => {
				if (dictionaryOfDoctors.Count == 0) {
					NotificationSystem.EmptyResults();
					PageError pageError = new PageError();
					NavigationService.Navigate(pageError);

					Timer timerTryToUpdate = new Timer(30 * 60 * 1000);
					timerTryToUpdate.Elapsed += TimerTryToUpdate_Elapsed;
					timerTryToUpdate.Start();
				} else {
					pageDepartmentSelect = new PageDepartmentSelect(dictionaryOfDoctors);
					CloseAllPagesExceptSplashScreen();
				}
			}));
		}

		private void TimerTryToUpdate_Elapsed(object sender, ElapsedEventArgs e) {
			backgroundWorkerUpdateData.RunWorkerAsync();
		}
	}
}
