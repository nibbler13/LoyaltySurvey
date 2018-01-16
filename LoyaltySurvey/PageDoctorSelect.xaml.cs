using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorSelect.xaml
	/// </summary>
	public partial class PageDoctorSelect : PageTemplate {
		private List<ItemDoctor> doctors;
		private WrapPanel wrapPanel;

		public PageDoctorSelect(List<ItemDoctor> doctors, string depName) {
			InitializeComponent();

			HideLogo();

			KeepAlive = true;

			SetLabelsContent(
				Properties.Resources.StringPageDoctorSelectTitle,
				Properties.Resources.StringPageDoctorSelectSubtitle);

			wrapPanel = new WrapPanel();

			Image imageDepartment = PageControlsFactory.CreateImage((System.Drawing.Bitmap)PageControlsFactory.GetImageForDepartment(depName));
			wrapPanel.Children.Add(imageDepartment);

			Label labelDep = PageControlsFactory.CreateLabel(
				PageControlsFactory.FirstCharToUpper(depName),
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				-1,
				-1,
				-1,
				-1,
				wrapPanel);
			
			wrapPanel.Height = DefaultButtonHeight;
			wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
			wrapPanel.VerticalAlignment = VerticalAlignment.Center;

			if (IsDebug)
				wrapPanel.Background = new SolidColorBrush(Colors.Aqua);

			Loaded += PageDoctorSelect_Loaded;
			Canvas.SetTop(wrapPanel, StartY);
			CanvasMain.Children.Add(wrapPanel);

			this.doctors = doctors;
			this.doctors.Sort(delegate (ItemDoctor doc1, ItemDoctor doc2) { return doc1.Name.CompareTo(doc2.Name); });

			CreateRootPanel(
				Properties.Settings.Default.PageDoctorSelectElementsInLine,
				Properties.Settings.Default.PageDoctorSelectElementsLinesCount,
				doctors.Count);

			List<string> keys = new List<string>();
			foreach (ItemDoctor doctor in doctors)
				keys.Add(doctor.Code + "|" + doctor.Name); //doctor.Code is using to find a photo

			FillPanelWithElements(keys, PageControlsFactory.ElementType.Doctor, PanelDoctor_Click);

			IsVisibleChanged += PageDoctorSelect_IsVisibleChanged;
		}

		private void PageDoctorSelect_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (IsVisible)
				ScrollViewer.ScrollToTop();
		}

		private void PageDoctorSelect_Loaded(object sender, RoutedEventArgs e) {
			Canvas.SetLeft(wrapPanel, StartX + AvailableWidth / 2 - wrapPanel.ActualWidth / 2);
		}

		private void PanelDoctor_Click(object sender, RoutedEventArgs e) {
			string docname = (sender as Control).Tag.ToString().Split('|')[1];
			SystemLogging.LogMessageToFile("Выбран доктор: " + docname);
			ItemDoctor selectedDoctor = new ItemDoctor("", "", "", "", "");

			foreach (ItemDoctor doctor in doctors) {
				if (doctor.Name.Equals(docname)) {
					selectedDoctor = doctor;
					break;
				}
			}

			PageDoctorRate pageDoctorRate = new PageDoctorRate(selectedDoctor);
			NavigationService.Navigate(pageDoctorRate);
		}
	}
}
