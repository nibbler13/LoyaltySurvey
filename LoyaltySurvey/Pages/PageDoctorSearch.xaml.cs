using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using LoyaltySurvey.Pages.Helpers;
using LoyaltySurvey.Utilities;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageDoctorSearch.xaml
	/// </summary>
	public partial class PageDoctorSearch : PageTemplate {
		private readonly Dictionary<string, List<ItemDoctor>> dictionaryOfDoctors;
		private readonly Label labelInfo;
		private const int MIN_TEXTBOX_SEARCH_LENGHT = 1;
		private readonly TextBox textBox;


		public PageDoctorSearch(Dictionary<string, List<ItemDoctor>> dictionaryOfDoctors) {
			InitializeComponent();
			
			this.dictionaryOfDoctors = dictionaryOfDoctors;
			
			SetLabelsContent(
				Properties.Resources.StringPageDoctorSearchTitle,
				Properties.Resources.StringPageDoctorSearchSubtitleEmpty);

			HideLogo();

			textBox = PageControlsFactory.CreateTextBox(
				FontFamilySub, 
				FontSizeMain);

			PageOnscreenKeyboard onscreenKeyboard = new PageOnscreenKeyboard(
				textBox, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, PageOnscreenKeyboard.KeyboardType.Alphabet);
			Canvas canvasKeyboard = onscreenKeyboard.CreateOnscreenKeyboard();
			CanvasMain.Children.Add(canvasKeyboard);
			Canvas.SetLeft(canvasKeyboard, StartX + AvailableWidth / 2 - canvasKeyboard.Width / 2);
			Canvas.SetTop(canvasKeyboard, StartY + AvailableHeight - canvasKeyboard.Height);
			
			textBox.Width = canvasKeyboard.Width;
			textBox.Height = DefaultButtonWidth;
			Canvas.SetLeft(textBox, StartX + AvailableWidth / 2 - textBox.Width / 2);
			Canvas.SetTop(textBox, StartY);
			CanvasMain.Children.Add(textBox);
			textBox.TextChanged += TextBox_TextChanged;

			Button buttonClear = PageControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonClear, 
				DefaultButtonWidth, 
				DefaultButtonWidth,
				Canvas.GetLeft(textBox) + textBox.Width + Gap,
				Canvas.GetTop(textBox),
				CanvasMain);
			buttonClear.Click += ButtonClear_Click;

			double scrollViewerX = StartX - LeftCornerShadow;
			double scrollViewerY = Canvas.GetTop(textBox) + textBox.Height + Gap - RightCornerShadow;
			double scrollViewerWidth = AvailableWidth + LeftCornerShadow + RightCornerShadow;
			double scrollViewerHeight = Canvas.GetTop(canvasKeyboard) - Gap - scrollViewerY + LeftCornerShadow + RightCornerShadow;

			CreateRootPanel(3, 1, 0, Orientation.Horizontal, scrollViewerWidth, scrollViewerHeight, scrollViewerX, scrollViewerY);
			
			double elementHeight = ScrollViewer.Height - LeftCornerShadow - RightCornerShadow;
			double elementWidth = (ScrollViewer.Width - LeftCornerShadow - RightCornerShadow - Gap * 2) / 3;
			SetElementsWidthAndHeight(elementWidth, elementHeight);

			labelInfo = PageControlsFactory.CreateLabel(
				"", 
				Colors.Transparent, 
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub, 
				FontSizeMain, 
				FontWeights.Normal,
				ScrollViewer.Width,
				ScrollViewer.Height,
				Canvas.GetLeft(ScrollViewer),
				Canvas.GetTop(ScrollViewer),
				CanvasMain);

			SetLabelInfoToInitial();
			onscreenKeyboard.SetEnterButtonClick(ButtonEnter_Click);
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e) {
			textBox.Clear();
			SetLabelInfoToInitial();
		}

		private void ButtonEnter_Click(object sender, RoutedEventArgs e) {
			StartSearch();
		}

		private void SetPanelResultVisible(bool isVisible) {
			ScrollViewer.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
			labelInfo.Visibility = isVisible ? Visibility.Hidden : Visibility.Visible;
		}

		private void SetLabelInfoToInitial() {
			SetLabelInfoText(Properties.Resources.StringPageDoctorSearchInitial);
			CanvasForElements.Children.Clear();
		}

		private void SetLabelInfoToNothingFound() {
			SetLabelInfoText(Properties.Resources.StringPageDoctorSearchNothingFound);
		}

		private void SetLabelInfoText(string str) {
			SetPanelResultVisible(false);
			SetLabelSubtitleText(Properties.Resources.StringPageDoctorSearchSubtitleEmpty);
			labelInfo.Content = PageControlsFactory.CreateTextBlock(
				str, 
				FontFamilySub, 
				FontSizeMain, 
				FontWeights.Normal);
		}

		private void TextBox_TextChanged(object sender, RoutedEventArgs e) {
			if (textBox.Text.Length < MIN_TEXTBOX_SEARCH_LENGHT) {
				SetLabelInfoToInitial();
				return;
			}

			StartSearch();
		}

		private static string NormalizeString(string str) {
			return str.ToLower().Replace("ё", "е");
		}

		private void StartSearch() {
			string text = textBox.Text;
			Logging.ToLog("Поиск докторов по тексту: " + text);

			if (string.IsNullOrWhiteSpace(text) ||
				string.IsNullOrEmpty(text)) {
				SetLabelInfoToInitial();
				return;
			}

			List<ItemDoctor> doctors = new List<ItemDoctor>();

			foreach (KeyValuePair<string, List<ItemDoctor>> dictionaryDepartment in dictionaryOfDoctors)
				foreach (ItemDoctor doctor in dictionaryDepartment.Value)
					if (NormalizeString(doctor.Name).StartsWith(NormalizeString(textBox.Text)))
						doctors.Add(doctor);

			if (doctors.Count == 0) {
				Logging.ToLog("По заданному тексту докторов не найдено");
				SetLabelInfoToNothingFound();
				return;
			}

			doctors.Sort(delegate (ItemDoctor doc1, ItemDoctor doc2) { return doc1.Name.CompareTo(doc2.Name); });
			UpdateResultPanelContent(doctors);
		}

		private void UpdateResultPanelContent(List<ItemDoctor> doctors) {
			Logging.ToLog("Количество найденных докторов: " + doctors.Count);

			CanvasForElements.Children.Clear();
			SetLabelSubtitleText(Properties.Resources.StringPageDoctorSearchSubtitleFound);
			SetPanelResultVisible(true);

			for (int i = 0; i < doctors.Count; i++) {
				string info = doctors[i].Name + Environment.NewLine + Environment.NewLine +
					doctors[i].Position;

				Button buttonDoctor = PageControlsFactory.CreateButtonWithImageAndText(
					info,
					ElementWidth,
					ElementHeight,
					PageControlsFactory.ElementType.Search,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal,
					dcode: doctors[i].Code);
				buttonDoctor.Margin = new Thickness(0, 0, i != doctors.Count - 1 ? Gap : RightCornerShadow, 0);
				CanvasForElements.Children.Add(buttonDoctor);

				buttonDoctor.Tag = doctors[i];
				buttonDoctor.Click += PanelDoctor_Click;
			}

			if (doctors.Count > 3) 
				CanvasForElements.HorizontalAlignment = HorizontalAlignment.Left;
			else 
				CanvasForElements.HorizontalAlignment = HorizontalAlignment.Center;
		}

		private void PanelDoctor_Click(object sender, RoutedEventArgs e) {
			ItemDoctor doctor = (sender as Control).Tag as ItemDoctor;
			Logging.ToLog("Выбран доктор: " + doctor.Name);
			PageDoctorRate pageDoctorRate = new PageDoctorRate(doctor);
			NavigationService.Navigate(pageDoctorRate);
		}
	}
}
