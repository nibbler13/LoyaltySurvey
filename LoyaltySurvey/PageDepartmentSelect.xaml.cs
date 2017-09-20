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

			LoggingSystem.LogMessageToFile("Количество отделений: " + dictionaryOfDoctors.Count);

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

			IsVisibleChanged += PageDepartmentSelect_IsVisibleChanged;
		}

		private void PageDepartmentSelect_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (IsVisible)
				ScrollViewer.ScrollToTop();
		}

		private void ButtonSearch_Click(object sender, RoutedEventArgs e) {
			LoggingSystem.LogMessageToFile("Нажата кнопка 'Поиск'");

			PageDoctorSearch pageDoctorSearch = new PageDoctorSearch(dictionaryOfDoctors);
			NavigationService.Navigate(pageDoctorSearch);
		}

		private void PanelDepartment_Click(object sender, RoutedEventArgs e) {
			string depname = (sender as Control).Tag.ToString();
			LoggingSystem.LogMessageToFile("Выбрано отделение: " + depname);

			PageDoctorSelect pageDoctorSelect = new PageDoctorSelect(dictionaryOfDoctors[depname], depname);
			NavigationService.Navigate(pageDoctorSelect);
		}
	}
}
