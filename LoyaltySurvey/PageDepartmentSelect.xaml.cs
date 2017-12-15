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
	public partial class PageDepartmentSelect : PageTemplate {
		private Dictionary<string, List<ItemDoctor>> _dictionaryOfDoctors;

		public PageDepartmentSelect(Dictionary<string, List<ItemDoctor>> dictionaryOfDoctors) {
			InitializeComponent();

			KeepAlive = true;

			this._dictionaryOfDoctors = dictionaryOfDoctors;

			SystemLogging.LogMessageToFile("Количество отделений: " + dictionaryOfDoctors.Count);

			SetLabelsContent(
				Properties.Resources.StringPageDepartmentSelectTitle,
				Properties.Resources.StringPageDepartmentSelectSubtitle);

			HideLogo();
			HideButtonBack();

			double originalAvailableWidth = AvailableWidth;

			CreateRootPanel(
				Properties.Settings.Default.PageDepartmentSelectElementsInLine,
				Properties.Settings.Default.PageDepartmentSelectElementsLinesCount,
				dictionaryOfDoctors.Count, type: PageControlsFactory.ElementType.Department);

			List<string> keys = dictionaryOfDoctors.Keys.ToList();
			FillPanelWithElements(keys, PageControlsFactory.ElementType.Department, PanelDepartment_Click);

			Button buttonSearch = PageControlsFactory.CreateButtonWithImageAndText(
				Properties.Resources.StringPageDepartmentSelectSearchButton, 
				DefaultButtonWidth * 6, 
				DefaultButtonHeight,
				PageControlsFactory.ElementType.Custom,
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
			SystemLogging.LogMessageToFile("Нажата кнопка 'Поиск'");

			PageDoctorSearch pageDoctorSearch = new PageDoctorSearch(_dictionaryOfDoctors);
			NavigationService.Navigate(pageDoctorSearch);
		}

		private void PanelDepartment_Click(object sender, RoutedEventArgs e) {
			string depname = (sender as Control).Tag.ToString();
			SystemLogging.LogMessageToFile("Выбрано отделение: " + depname);

			PageDoctorSelect pageDoctorSelect = new PageDoctorSelect(_dictionaryOfDoctors[depname], depname);
			NavigationService.Navigate(pageDoctorSelect);
		}
	}
}
