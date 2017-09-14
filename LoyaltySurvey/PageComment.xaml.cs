using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageComment.xaml
	/// </summary>
	public partial class PageComment : ClassPageTemplate {
		private Button buttonNext;
		private TextBox textBox;
		private double buttonWidth;

		public PageComment() {
			InitializeComponent();

			buttonWidth = DefaultButtonWidth * 3;

			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageCommentTitle,
				Properties.Resources.StringPageCommentSubtitle);

			CreateQuestionControlsAnd(Properties.Resources.StringPageCommentQuestion, Properties.Resources.Background_Comment,
				ButtonNo_Click, ButtonYes_Click);
		}

		private void ButtonYes_Click(object sender, RoutedEventArgs e) {
			textBox = ControlsFactory.CreateTextBox(FontFamilySub, FontSizeMain, false);
			OnscreenKeyboard onscreenKeyboard = new OnscreenKeyboard(textBox, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, OnscreenKeyboard.KeyboardType.Full);
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
			
			buttonNext = ControlsFactory.CreateButtonWithImageAndText(
				"Далее",
				buttonWidth,
				DefaultButtonWidth,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonOk,
				StartX + AvailableWidth - buttonWidth,
				StartY + AvailableHeight + Gap,
				CanvasMain);
			buttonNext.Click += ButtonNext_Click;
			buttonNext.Background = new SolidColorBrush(Properties.Settings.Default.ColorHeaderBackground);
			buttonNext.Foreground = new SolidColorBrush(Properties.Settings.Default.ColorHeaderForeground);

			Button buttonClear = ControlsFactory.CreateButtonWithImageOnly(
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

		private void ButtonNext_Click(object sender, RoutedEventArgs e) {
			PageCallback pageCallback = new PageCallback();
			NavigationService.Navigate(pageCallback);
		}

		private void ButtonNo_Click(object sender, RoutedEventArgs e) {
			PageCallback pageCallback = new PageCallback();
			NavigationService.Navigate(pageCallback);
		}
	}
}
