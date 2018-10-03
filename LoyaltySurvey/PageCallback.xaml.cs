using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageCallback.xaml
	/// </summary>
	public partial class PageCallback : PageTemplate {
		private Button buttonNext;
		private TextBox textBoxData;
		private TextBox textBoxVisible;
		private double buttonWidth;
		private string mask = "+7 (___) ___-__-__";

		public PageCallback(ItemSurveyResult surveyResult) {
			InitializeComponent();

			this._surveyResult = surveyResult;

			buttonWidth = DefaultButtonWidth * 3;

			//HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageCallbackTitle,
				Properties.Resources.StringPageCallbackSubtitle);

			CreateQuestionControlsAnd(Properties.Resources.StringPageCallbackQuestion, Properties.Resources.BackgroundCallBack,
				ButtonNoOrNext_Click, ButtonYes_Click);
		}

		private void ButtonYes_Click(object sender, RoutedEventArgs e) {
			SystemLogging.LogMessageToFile("Нажата кнопка 'Да'");

			textBoxData = new TextBox();
			textBoxData.TextChanged += TextBoxData_TextChanged;

			textBoxVisible = PageControlsFactory.CreateTextBox(FontFamilySub, FontSizeMain * 1.3);
			PageOnscreenKeyboard onscreenKeyboard = new PageOnscreenKeyboard(textBoxData, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, PageOnscreenKeyboard.KeyboardType.Number);
			Canvas canvasKeyboard = onscreenKeyboard.CreateOnscreenKeyboard();
			Canvas.SetLeft(canvasKeyboard, StartX + AvailableWidth / 2 - canvasKeyboard.Width / 2);
			Canvas.SetTop(canvasKeyboard, StartY + AvailableHeight / 2 - (canvasKeyboard.Height - Gap - DefaultButtonWidth) / 2);
			CanvasMain.Children.Add(canvasKeyboard);

			textBoxVisible.Width = canvasKeyboard.Width * 1.8;
			textBoxVisible.Height = DefaultButtonWidth;
			Canvas.SetLeft(textBoxVisible, StartX + AvailableWidth / 2 - textBoxVisible.Width / 2);
			Canvas.SetTop(textBoxVisible, Canvas.GetTop(canvasKeyboard) - Gap - textBoxVisible.Height);
			CanvasMain.Children.Add(textBoxVisible);
			textBoxVisible.Text = mask;

			SetLabelsContent(
				Properties.Resources.StringPageCallbackTitleTextBox,
				Properties.Resources.StringPageCallbackSubtitleTextBox);
			
			buttonNext = PageControlsFactory.CreateButtonWithImageAndText(
				"Далее",
				buttonWidth,
				DefaultButtonWidth,
				PageControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonOk,
				StartX + AvailableWidth - buttonWidth,
				StartY + AvailableHeight + Gap,
				CanvasMain);
			buttonNext.Style = Application.Current.MainWindow.FindResource("RoundCornerGreen") as Style;
			buttonNext.Click += ButtonNoOrNext_Click;
			buttonNext.Background = new SolidColorBrush(Properties.Settings.Default.ColorHeaderBackground);
			buttonNext.Foreground = new SolidColorBrush(Properties.Settings.Default.ColorHeaderForeground);

			Button buttonClear = PageControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonClear, 
				DefaultButtonWidth, 
				DefaultButtonWidth,
				Canvas.GetLeft(textBoxVisible) + textBoxVisible.Width + Gap,
				Canvas.GetTop(textBoxVisible),
				CanvasMain);
			buttonClear.Click += ButtonClear_Click;
		}

		private void TextBoxData_TextChanged(object sender, TextChangedEventArgs e) {
			string enteredText = textBoxData.Text;

			if (enteredText.Length > 10) {
				enteredText = enteredText.Substring(0, 10);
				textBoxData.Text = enteredText;
			}

			string updatedMask = mask;

			Regex regex = new Regex(Regex.Escape("_"));

			foreach (char c in enteredText) {
				updatedMask = regex.Replace(updatedMask, c.ToString(), 1);
			}

			textBoxVisible.Text = updatedMask;
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e) {
			textBoxData.Clear();
		}

		private void ButtonNoOrNext_Click(object sender, RoutedEventArgs e) {
			string phoneNumber = "Refused";
			bool isNextPressed = (sender as Button).Tag.ToString().Equals("Далее");

			if (isNextPressed) {
				phoneNumber = textBoxData.Text;
				SystemLogging.LogMessageToFile("Нажата кнопка 'Далее', введенный номер телефона: " + phoneNumber);
			} else
				SystemLogging.LogMessageToFile("Нажата кнопка 'Нет'");

			_surveyResult.PhoneNumber = phoneNumber;
			SystemNotification.NegativeMark(_surveyResult);

			Page page;
			if (((MainWindow)Application.Current.MainWindow).previousRatesDcodes.Count > 0 ||
				_surveyResult.DCode.Equals("0")) {
				_surveyResult. ClinicRecommendMark = "Don't need";
				page = new PageThanks(_surveyResult);
			} else
				page = new PageClinicRate(_surveyResult);

			NavigationService.Navigate(page);
		}
	}
}
