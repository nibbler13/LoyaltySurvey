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
	public partial class PageSplashScreen : PageTemplate {
		private Dictionary<string, List<ItemDoctor>> dictionaryOfDoctors = new Dictionary<string, List<ItemDoctor>>();
		private BackgroundWorker backgroundWorkerUpdateData;
		private PageDepartmentSelect pageDepartmentSelect;
		private PageSelectSurvey pageSelectSurvey;
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
			mediaElement.IsEnabled = false;
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
			DateTime fireTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 6, 0, 0, DateTimeKind.Local);

			if (nowTime.CompareTo(new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 5, 58, 0, DateTimeKind.Local)) > 0)
				fireTime = fireTime.AddDays(1);

			double tickTime = (fireTime - nowTime).TotalMilliseconds;
			timerUpdateData = new Timer(tickTime);
			timerUpdateData.Elapsed += TimerUpdateData_Elapsed;
			timerUpdateData.Start();
		}

		private void TimerUpdateData_Elapsed(object sender, ElapsedEventArgs e) {
			timerUpdateData.Stop();
			backgroundWorkerUpdateData.RunWorkerAsync(false);
			//TimerUpdateDataSetup();
		}

		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				dictionaryOfDoctors = SystemDataHandle.GetDoctorsDictionary();
				//dictionaryOfDoctors.Add("test", new List<ItemDoctor>() { new ItemDoctor("test", "test", "test", "123", "123") });

				if ((bool)e.Argument == true &&
					Directory.GetFiles(Directory.GetCurrentDirectory() + "\\DoctorsPhotos\\", "*.jpg", SearchOption.AllDirectories).Length != 0) 
					return;

				SystemDataHandle.UpdateDoctorsPhoto(dictionaryOfDoctors);
				Application.Current.Dispatcher.Invoke(new Action(() => {
					System.Threading.Thread.Sleep(60 * 1000);
					Application.Current.Shutdown();
				}));
			} catch (Exception exception) {
				SystemLogging.LogMessageToFile("BackgroundWorker_DoWork exception: " + exception.Message +
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
			if (timeSpanTimeAfterClosing.TotalSeconds > Properties.Settings.Default.PageAutocloseTimeoutInSeconds * 2)
				((MainWindow)Application.Current.MainWindow).previousRatesDcodes.Clear();

			if (Properties.Settings.Default.EnableRegistrySurvey)
				NavigationService.Navigate(pageSelectSurvey);
			else
				NavigationService.Navigate(pageDepartmentSelect);
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				SystemNotification.EmptyResults();
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);

				Timer timerTryToUpdate = new Timer(30 * 60 * 1000);
				timerTryToUpdate.Elapsed += TimerTryToUpdate_Elapsed;
				timerTryToUpdate.Start();
			} else {
				pageDepartmentSelect = new PageDepartmentSelect(dictionaryOfDoctors);
				pageSelectSurvey = new PageSelectSurvey(pageDepartmentSelect);

				try {
					(Application.Current.MainWindow as NavigationWindow).NavigationService.GoBack();
				} catch (Exception exc) {
					SystemLogging.LogMessageToFile(exc.Message + Environment.NewLine + exc.StackTrace);
				}
			}
		}

		private void TimerTryToUpdate_Elapsed(object sender, ElapsedEventArgs e) {
			backgroundWorkerUpdateData.RunWorkerAsync();
		}
	}
}
