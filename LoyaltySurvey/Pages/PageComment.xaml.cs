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
	/// Логика взаимодействия для PageComment.xaml
	/// </summary>
	public partial class PageComment : PageTemplate {
		private Button buttonNext;
		private TextBox textBox;
		private readonly double buttonWidth;

		public PageComment(ItemSurveyResult surveyResult) {
			InitializeComponent();

			this.SurveyResult = surveyResult;

			buttonWidth = DefaultButtonWidth * 3;

			//HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageCommentTitle,
				Properties.Resources.StringPageCommentSubtitle);

			CreateQuestionControlsAnd(Properties.Resources.StringPageCommentQuestion, Properties.Resources.BackgroundComment,
				ButtonNoOrNext_Click, ButtonYes_Click);
		}

		private void ButtonYes_Click(object sender, RoutedEventArgs e) {
			Logging.ToLog("Нажата кнопка 'Да'");

			textBox = PageControlsFactory.CreateTextBox(FontFamilySub, FontSizeMain, false);
			PageOnscreenKeyboard onscreenKeyboard = new PageOnscreenKeyboard(textBox, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, PageOnscreenKeyboard.KeyboardType.Full);
			Canvas canvasKeyboard = onscreenKeyboard.CreateOnscreenKeyboard();
			Canvas.SetLeft(canvasKeyboard, StartX + AvailableWidth / 2 - canvasKeyboard.Width / 2);
			Canvas.SetTop(canvasKeyboard, StartY + AvailableHeight - canvasKeyboard.Height);
			CanvasMain.Children.Add(canvasKeyboard);

			textBox.Width = canvasKeyboard.Width;
			textBox.Height = Canvas.GetTop(canvasKeyboard) - StartY - Gap * 2;
			Canvas.SetLeft(textBox, StartX + AvailableWidth / 2 - textBox.Width / 2);
			Canvas.SetTop(textBox, StartY);
			CanvasMain.Children.Add(textBox);

			SetLabelsContent(
				Properties.Resources.StringPageCommentTitleTextBox,
				Properties.Resources.StringPageCommentSubtitleTextBox);
			
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
				Canvas.GetLeft(textBox) + textBox.Width + Gap,
				Canvas.GetTop(textBox),
				CanvasMain);
			buttonClear.Click += ButtonClear_Click;
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e) {
			textBox.Clear();
		}

		private void ButtonNoOrNext_Click(object sender, RoutedEventArgs e) {
			string comment = "Refused";

			if ((sender as Button).Tag.ToString().Equals("Далее")) {
				comment = textBox.Text;
				Logging.ToLog("Нажата кнопка 'Далее', введенный комментарий: " + comment);
			} else
				Logging.ToLog("Нажата кнопка 'Нет'");

			SurveyResult.Comment = comment;

			PageCallback pageCallback = new PageCallback(SurveyResult);
			NavigationService.Navigate(pageCallback);
		}
	}
}
