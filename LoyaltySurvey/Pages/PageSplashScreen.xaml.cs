using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using LoyaltySurvey.Utilities;
using LoyaltySurvey.Pages.Helpers;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageSplashScreen.xaml
	/// </summary>
	public partial class PageSplashScreen : PageTemplate {
		private Dictionary<string, List<ItemDoctor>> dictionaryOfDoctors = new Dictionary<string, List<ItemDoctor>>();
		private PageDepartmentSelect pageDepartmentSelect;
		private PageSelectSurvey pageSelectSurvey;

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
            MediaElement mediaElement = new MediaElement {
                Source = new Uri(Directory.GetCurrentDirectory() + "\\" + Properties.Settings.Default.WelcomeAnimationFileName),
                Width = mediaElementWidth,
                Height = mediaElementHeight,
                Stretch = Stretch.Uniform,
                IsEnabled = false,
                UnloadedBehavior = MediaState.Manual
            };

            Canvas.SetLeft(mediaElement, rect.Location.X);
			Canvas.SetTop(mediaElement, rect.Location.Y);
			CanvasMain.Children.Add(mediaElement);
			mediaElement.MediaEnded += MediaElement_MediaEnded;

			HideButtonBack();
			DisablePageAutoCloseTimer();
			DisablePageAutoCloseTimerResetByClick();
			SetupTimerAutoClose();
			LoadData();
		}

		private async void LoadData() {
			await Task.Run(() => {
				dictionaryOfDoctors = DataHandle.GetDoctorsDictionary();
				DataHandle.UpdateDoctorsPhotos(dictionaryOfDoctors);

				Application.Current.Dispatcher.Invoke(() => {
					if (dictionaryOfDoctors.Count == 0) {
						SystemNotification.EmptyResults();
						PageError pageError = new PageError();
						NavigationService.Navigate(pageError);
					} else {
						pageDepartmentSelect = new PageDepartmentSelect(dictionaryOfDoctors);
						pageSelectSurvey = new PageSelectSurvey(pageDepartmentSelect);
					}

					PreviewMouseLeftButtonDown += PageSplashScreen_PreviewMouseDown;
				});
			}).ConfigureAwait(false);
		}

		private void SetupTimerAutoClose() {
			DateTime nowTime = DateTime.Now;
			DateTime fireTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 6, 0, 0, DateTimeKind.Local);

			if (nowTime.CompareTo(new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 5, 58, 0, DateTimeKind.Local)) > 0)
				fireTime = fireTime.AddDays(1);

			double tickTime = (fireTime - nowTime).TotalMilliseconds;
			DispatcherTimer timerAutoClose = new DispatcherTimer {
				Interval = TimeSpan.FromMilliseconds(tickTime)
			};

			timerAutoClose.Tick += (s, e) => {
				Application.Current.Dispatcher.Invoke(() => {
					SystemLogging.ToLog("Автоматическое завершение работы");
					Application.Current.Shutdown();
				});
			};

			timerAutoClose.Start();
		}

		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void PageSplashScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);
				return;
			}

			TimeSpan timeSpanTimeAfterClosing = DateTime.Now - ((MainWindow)Application.Current.MainWindow).PreviousThankPageCloseTime;
			if (timeSpanTimeAfterClosing.TotalSeconds > Properties.Settings.Default.PageAutocloseTimeoutInSeconds * 2)
				((MainWindow)Application.Current.MainWindow).PreviousRatesDcodes.Clear();

			if (Properties.Settings.Default.EnableRegistrySurvey)
				NavigationService.Navigate(pageSelectSurvey);
			else
				NavigationService.Navigate(pageDepartmentSelect);
		}
	}
}
