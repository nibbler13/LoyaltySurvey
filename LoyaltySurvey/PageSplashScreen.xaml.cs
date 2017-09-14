using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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
		private FBClient fbClient = new FBClient(
			Properties.Settings.Default.MisInfoclinicaDbAddress,
			Properties.Settings.Default.MisInfoclinicaDbName,
			Properties.Settings.Default.MisInfoclinicaDbUser,
			Properties.Settings.Default.MisInfoclinicaDbPassword);
		private Dictionary<string, List<Doctor>> dictionaryOfDoctors = new Dictionary<string, List<Doctor>>();
		private BackgroundWorker backgroundWorker;
		private PageDepartmentSelect pageDepartmentSelect;

		public PageSplashScreen() {
			InitializeComponent();

			KeepAlive = true;

			SetLabelsContent(
				Properties.Resources.StringPageSplashScreenTitle,
				Properties.Resources.StringPageSplashScreenSubtitle);

			Label labelWelcome = ControlsFactory.CreateLabel(
				Properties.Resources.StringPageSplashScreenWelcome,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight,
				StartX,
				StartY,
				CanvasMain);
			Canvas.SetZIndex(labelWelcome, 0);
			//labelWelcome.VerticalContentAlignment = VerticalAlignment.Bottom;

			MediaElement mediaElement = new MediaElement();
			mediaElement.Source = new Uri(Directory.GetCurrentDirectory() + "\\simple.gif");
			mediaElement.Width = AvailableWidth;
			mediaElement.Height = AvailableHeight - labelWelcome.Height - Gap;
			mediaElement.Stretch = Stretch.Uniform;
			Canvas.SetLeft(mediaElement, StartX);
			Canvas.SetTop(mediaElement, Canvas.GetTop(labelWelcome) + labelWelcome.Height + Gap);
			CanvasMain.Children.Add(mediaElement);
			mediaElement.UnloadedBehavior = MediaState.Manual;
			mediaElement.MediaEnded += MediaElement_MediaEnded;

			PreviewMouseLeftButtonDown += PageSplashScreen_PreviewMouseDown;
			HideButtonBack();

			backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
			backgroundWorker.RunWorkerAsync();
		}

		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
			Console.WriteLine("MediaElement_MediaEnded");
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			UpdateListOfDoctors();
		}

		private void PageSplashScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);
				return;
			}
			
			NavigationService.Navigate(pageDepartmentSelect);
		}

		private void UpdateListOfDoctors() {
			LoggingSystem.LogMessageToFile("Обновление данных из базы ИК");
			DataTable dataTable = fbClient.GetDataTable(Properties.Settings.Default.SqlQueryDoctors);

			if (dataTable.Rows.Count == 0) {
				LoggingSystem.LogMessageToFile("Из базы ИК вернулась пустая таблица");
				return;
			}

			Dictionary<string, List<Doctor>> dictionary = new Dictionary<string, List<Doctor>>();

			foreach (DataRow dataRow in dataTable.Rows) {
				try {
					string department = dataRow["DEPARTMENT"].ToString().ToLower();
					string docname = dataRow["DOCNAME"].ToString();
					string docposition = dataRow["DOCPOSITION"].ToString();

					Doctor doctor = new Doctor(docname, docposition, department, "123");

					if (dictionary.ContainsKey(department)) {
						if (dictionary[department].Contains(doctor))
							continue;

						dictionary[department].Add(doctor);
					} else {
						dictionary.Add(department, new List<Doctor>() { doctor });
					}
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile("Не удалось обработать строку с данными: " + dataRow.ToString() + ", " + e.Message);
				}
			}

			LoggingSystem.LogMessageToFile("Обработано строк:" + dataTable.Rows.Count);

			dictionaryOfDoctors = dictionary;
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (dictionaryOfDoctors.Count == 0) {
				PageError pageError = new PageError();
				NavigationService.Navigate(pageError);
			} else {
				pageDepartmentSelect = new PageDepartmentSelect(dictionaryOfDoctors);

				//if (NavigationService.CanGoBack)
				//	NavigationService.GoBack();
			}
		}
	}
}
