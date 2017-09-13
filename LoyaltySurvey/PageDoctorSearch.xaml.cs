using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorSearch.xaml
	/// </summary>
	public partial class PageDoctorSearch : ClassPageTemplate {
		private Dictionary<string, List<Doctor>> dictionaryOfDoctors;
		private Button buttonScrollLeft;
		private Button buttonScrollRight;
		private Label labelInfo;
		private int minTextBoxSearchLength = 3;
		private TextBox textBox;


		public PageDoctorSearch(Dictionary<string, List<Doctor>> dictionaryOfDoctors) {
			InitializeComponent();
			
			this.dictionaryOfDoctors = dictionaryOfDoctors;
			
			SetLabelsContent(
				Properties.Resources.StringPageDoctorSearchTitle,
				Properties.Resources.StringPageDoctorSearchSubtitleEmpty);

			HideLogo();

			textBox = ControlsFactory.CreateTextBox(
				FontFamilySub, 
				FontSizeMain);

			OnscreenKeyboard onscreenKeyboard = new OnscreenKeyboard(
				textBox, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, OnscreenKeyboard.KeyboardType.Short);
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

			Button buttonClear = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonClear, 
				DefaultButtonWidth, 
				DefaultButtonWidth,
				Canvas.GetLeft(textBox) + textBox.Width + Gap,
				Canvas.GetTop(textBox),
				CanvasMain);
			buttonClear.Click += ButtonClear_Click;

			double scrollViewerX = StartX - leftCornerShadow;
			double scrollViewerY = Canvas.GetTop(textBox) + textBox.Height + Gap - rightCornerShadow;
			double scrollViewerWidth = AvailableWidth + leftCornerShadow + rightCornerShadow;
			double scrollViewerHeight = Canvas.GetTop(canvasKeyboard) - Gap - scrollViewerY + leftCornerShadow + rightCornerShadow;

			CreateRootPanel(3, 1, 0);
			ScrollViewer.Width = scrollViewerWidth;
			ScrollViewer.Height = scrollViewerHeight;
			Canvas.SetLeft(ScrollViewer, scrollViewerX);
			Canvas.SetTop(ScrollViewer, scrollViewerY);
			CanvasForElements.Orientation = Orientation.Horizontal;
			CanvasForElements.VerticalAlignment = VerticalAlignment.Top;
			
			double elementHeight = ScrollViewer.Height - leftCornerShadow - rightCornerShadow;
			double elementWidth = (ScrollViewer.Width - leftCornerShadow - rightCornerShadow - Gap * 2) / 3;
			SetElementsWidthAndHeight(elementWidth, elementHeight);
			
			CreateLeftRightButtons(Canvas.GetTop(canvasKeyboard));

			labelInfo = ControlsFactory.CreateLabel(
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

		private void CreateLeftRightButtons(double top) {
			buttonScrollLeft = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonLeft, 
				DefaultButtonWidth, 
				DefaultButtonWidth);
			buttonScrollLeft.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);

			buttonScrollRight = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonRight, 
				DefaultButtonWidth, 
				DefaultButtonWidth);
			buttonScrollRight.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);

			Canvas.SetLeft(buttonScrollLeft, StartX);
			Canvas.SetTop(buttonScrollLeft, top);

			Canvas.SetLeft(buttonScrollRight, StartX + AvailableWidth - DefaultButtonWidth);
			Canvas.SetTop(buttonScrollRight, top);

			CanvasMain.Children.Add(buttonScrollLeft);
			CanvasMain.Children.Add(buttonScrollRight);

			buttonScrollLeft.Click += ButtonScrollLeft_Click;
			buttonScrollRight.Click += ButtonScrollRight_Click;
		}

		private void ButtonScrollLeft_Click(object sender, RoutedEventArgs e) {
			Console.WriteLine("ButtonScrollLeft_Click");
		}

		private void ButtonScrollRight_Click(object sender, RoutedEventArgs e) {
			Console.WriteLine("ButtonScrollRight_Click");
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
			SetButtonVisible(buttonScrollLeft, false);
			SetButtonVisible(buttonScrollRight, false);
		}

		private void SetLabelInfoToInitial() {
			Console.WriteLine("SetLabelInfoToInitial");
			SetLabelInfoText(Properties.Resources.StringPageDoctorSearchInitial);
		}

		private void SetLabelInfoToNothingFound() {
			Console.WriteLine("SetLabelInfoToNothingFoun");
			SetLabelInfoText(Properties.Resources.StringPageDoctorSearchNothingFound);
		}

		private void SetLabelInfoText(string str) {
			SetPanelResultVisible(false);
			SetLabelSubtitleText(Properties.Resources.StringPageDoctorSearchSubtitleEmpty);
			labelInfo.Content = ControlsFactory.CreateTextBlock(
				str, 
				FontFamilySub, 
				FontSizeMain, 
				FontWeights.Normal);
		}

		private void SetButtonVisible(Button button, bool isVisible) {
			Visibility visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
			button.Visibility = visibility;
			button.Visibility = visibility;
		}

		private void TextBox_TextChanged(object sender, RoutedEventArgs e) {
			if (textBox.Text.Length < minTextBoxSearchLength) {
				SetLabelInfoToInitial();
				return;
			}

			StartSearch();
		}

		private string NormalizeString(string str) {
			return str.ToLower().Replace("ё", "е");
		}

		private void StartSearch() {
			string text = textBox.Text;

			if (string.IsNullOrWhiteSpace(text) ||
				string.IsNullOrEmpty(text)) {
				SetLabelInfoToInitial();
				return;
			}

			List<Doctor> doctors = new List<Doctor>();

			foreach (KeyValuePair<string, List<Doctor>> dictionaryDepartment in dictionaryOfDoctors)
				foreach (Doctor doctor in dictionaryDepartment.Value)
					if (NormalizeString(doctor.Name).StartsWith(NormalizeString(textBox.Text)))
						doctors.Add(doctor);

			if (doctors.Count == 0) {
				SetLabelInfoToNothingFound();
				return;
			}

			doctors.Sort(delegate (Doctor doc1, Doctor doc2) { return doc1.Name.CompareTo(doc2.Name); });
			UpdateResultPanelContent(doctors);
		}

		private void UpdateResultPanelContent(List<Doctor> doctors) {
			//Console.WriteLine("UpdateResultPanelContent: " + doctors.Count);

			CanvasForElements.Children.Clear();
			SetLabelSubtitleText(Properties.Resources.StringPageDoctorSearchSubtitleFound);
			SetPanelResultVisible(true);

			foreach (Doctor doctor in doctors) {
				string info = doctor.Name + Environment.NewLine + Environment.NewLine +
					doctor.Position;
				Button buttonDoctor = ControlsFactory.CreateButtonWithImageAndText(
					info,
					ElementWidth,
					ElementHeight,
					ControlsFactory.ElementType.Search,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal);
				buttonDoctor.Margin = new Thickness(0, 0, doctors.Count > 1 ? Gap : 0, 0);
				CanvasForElements.Children.Add(buttonDoctor);

				buttonDoctor.Tag = doctor;
				buttonDoctor.Click += PanelDoctor_Click;
			}

			if (doctors.Count > 3) {
				SetButtonVisible(buttonScrollRight, true);
				CanvasForElements.HorizontalAlignment = HorizontalAlignment.Left;
			} else {
				CanvasForElements.HorizontalAlignment = HorizontalAlignment.Center;
			}

			CanvasForElements.Width = doctors.Count * (ElementWidth + Gap);
		}

		private void PanelDoctor_Click(object sender, RoutedEventArgs e) {
			Doctor doctor = (sender as Control).Tag as Doctor;
			PageDoctorRate pageDoctorRate = new PageDoctorRate(doctor);
			NavigationService.Navigate(pageDoctorRate);
		}
	}
}
