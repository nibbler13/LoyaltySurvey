using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDepartmentsSelect.xaml
	/// </summary>
	public partial class PageDepartmentSelect : ClassPageTemplate {
		private Dictionary<string, List<Doctor>> dictionaryOfDoctors;

		public PageDepartmentSelect(Dictionary<string, List<Doctor>> dictionaryOfDoctors) {
			InitializeComponent();

			KeepAlive = true;

			this.dictionaryOfDoctors = dictionaryOfDoctors;

			Console.WriteLine(dictionaryOfDoctors.Count);

			SetLabelsContent(
				Properties.Resources.StringPageDepartmentSelectTitle,
				Properties.Resources.StringPageDepartmentSelectSubtitle);

			HideLogo();
			HideButtonBack();

			double originalAvailableWidth = AvailableWidth;

			CreateRootPanel(
				Properties.Settings.Default.PageDepartmentSelectElementsInLine,
				Properties.Settings.Default.PageDepartmentSelectElementsLinesCount,
				dictionaryOfDoctors.Count);

			List<string> keys = dictionaryOfDoctors.Keys.ToList();
			FillPanelWithElements(keys, ControlsFactory.ElementType.Department, PanelDepartment_Click);

			//scrollViewer.Height = scrollViewer.Height - elementHeight - Gap;
			//Canvas.SetTop(scrollViewer, StartY + elementHeight + Gap);

			//Canvas.SetTop(buttonScrollUp, StartY + elementHeight + Gap);

			Button buttonSearch = ControlsFactory.CreateButtonWithImageAndText(
				Properties.Resources.StringPageDepartmentSelectSearchButton, 
				DefaultButtonWidth * 6, 
				DefaultButtonHeight,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal, 
				Properties.Resources.ButtonSearch,
				StartX + originalAvailableWidth / 2 - DefaultButtonWidth * 3,
				StartY,
				CanvasMain);
			buttonSearch.Click += ButtonSearch_Click;

			buttonSearch.Background = new SolidColorBrush(Colors.Beige);
		}

		private void ButtonSearch_Click(object sender, RoutedEventArgs e) {
			Console.WriteLine("ButtonSearch_Click");

			PageDoctorSearch pageDoctorSearch = new PageDoctorSearch(dictionaryOfDoctors);
			NavigationService.Navigate(pageDoctorSearch);
		}

		private void PanelDepartment_Click(object sender, RoutedEventArgs e) {
			string depname = (sender as Control).Tag.ToString();
			Console.WriteLine("ButtonDepartment_Click : " + depname);

			PageDoctorSelect pageDoctorSelect = new PageDoctorSelect(dictionaryOfDoctors[depname], depname);
			NavigationService.Navigate(pageDoctorSelect);
		}
	}
}
