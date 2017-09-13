using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageCallback.xaml
	/// </summary>
	public partial class PageCallback : ClassPageTemplate {
		private Image image;
		private Label labelQuestion;
		private Button buttonNo;
		private Button buttonYes;
		private Button buttonNext;
		private TextBox textBox;
		private double buttonWidth;

		public PageCallback() {
			InitializeComponent();
			
			buttonWidth = DefaultButtonWidth * 3;

			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageCallbackTitle,
				Properties.Resources.StringPageCallbackSubtitle);

			//double lastHeight = StartY + AvailableHeight - (Canvas.GetTop(labelQuestion) + labelQuestion.Height) - Gap;
			buttonNo = ControlsFactory.CreateButtonWithImageAndText(
				"Нет",
				buttonWidth,
				DefaultButtonHeight,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonClose,
				StartX + AvailableWidth / 2 - Gap * 2 - buttonWidth,
				StartY + AvailableHeight - DefaultButtonHeight,
				CanvasMain);

			buttonYes = ControlsFactory.CreateButtonWithImageAndText(
				"Да",
				buttonNo.Width,
				buttonNo.Height,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonOk,
				Canvas.GetLeft(buttonNo) + buttonNo.Width + Gap * 4,
				Canvas.GetTop(buttonNo),
				CanvasMain);
			buttonYes.Background = new SolidColorBrush(Properties.Settings.Default.ColorHeaderBackground);
			buttonYes.Foreground = new SolidColorBrush(Properties.Settings.Default.ColorHeaderForeground);

			buttonNo.Click += ButtonNoOrNext_Click;
			buttonYes.Click += ButtonYes_Click;



			labelQuestion = ControlsFactory.CreateLabel(
				Properties.Resources.StringPageCallbackQuestion,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight,
				StartX,
				Canvas.GetTop(buttonNo) - Gap - DefaultButtonHeight,
				CanvasMain);

			image = ControlsFactory.CreateImage(
				Properties.Resources.Background_CallBack,
				AvailableWidth,
				Canvas.GetTop(labelQuestion) - StartY - Gap,
				StartX,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonYes_Click(object sender, RoutedEventArgs e) {
			List<Control> controlsToRemove = new List<Control>() {
				labelQuestion,
				buttonNo,
				buttonYes,
			};

			HideLogo();

			foreach (Control control in controlsToRemove)
				CanvasMain.Children.Remove(control);
			CanvasMain.Children.Remove(image);

			textBox = ControlsFactory.CreateTextBox(FontFamilySub, FontSizeMain);
			OnscreenKeyboard onscreenKeyboard = new OnscreenKeyboard(textBox, AvailableWidth, AvailableHeight,
				StartX, StartY, Gap, FontSizeMain, OnscreenKeyboard.KeyboardType.Number);
			Canvas canvasKeyboard = onscreenKeyboard.CreateOnscreenKeyboard();
			Canvas.SetLeft(canvasKeyboard, StartX + AvailableWidth / 2 - canvasKeyboard.Width / 2);
			Canvas.SetTop(canvasKeyboard, StartY + AvailableHeight / 2 - (canvasKeyboard.Height - Gap - DefaultButtonWidth) / 2);
			CanvasMain.Children.Add(canvasKeyboard);

			textBox.Width = canvasKeyboard.Width;
			textBox.Height = DefaultButtonWidth;
			Canvas.SetLeft(textBox, Canvas.GetLeft(canvasKeyboard));
			Canvas.SetTop(textBox, Canvas.GetTop(canvasKeyboard) - Gap - textBox.Height);
			CanvasMain.Children.Add(textBox);
			textBox.Text = "+7 (960) 181-18-73";

			SetLabelsContent(
				Properties.Resources.StringPageCallbackTitleTextBox,
				Properties.Resources.StringPageCallbackSubtitleTextBox);
			
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
			buttonNext.Click += ButtonNoOrNext_Click;
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

		private void ButtonNoOrNext_Click(object sender, RoutedEventArgs e) {
			PageClinicRate pageClinicRate = new PageClinicRate();
			NavigationService.Navigate(pageClinicRate);
		}
	}
}
