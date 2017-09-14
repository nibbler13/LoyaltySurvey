using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageClinicRate.xaml
	/// </summary>
	public partial class PageClinicRate : ClassPageTemplate {
		public PageClinicRate() {
			InitializeComponent();

			HideLogo();
			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageClinicRateTitle,
				Properties.Resources.StringPageClinicRateSubtitle);




			double labelWidth = DefaultButtonWidth * 4;
			double labelHeight = DefaultButtonHeight / 2;
			double currentX = StartX;
			double currentY = StartY + AvailableHeight;

			List<string> labels = new List<string>() {
				Properties.Resources.StringPageClinicRateNotAtAll,
				//Properties.Resources.StringPageClinicRateDontKnow,
				Properties.Resources.StringPageClinicRateMostLikely
			};

			for (int i = 0; i < labels.Count; i++) {
				HorizontalAlignment horizontalAlignment;
				if (i == 0) {
					horizontalAlignment = HorizontalAlignment.Left;
					currentX = StartX;
				//} else if (i == 1) {
				//	horizontalAlignment = HorizontalAlignment.Center;
				//	currentX = StartX + AvailableWidth / 2 - labelWidth / 2;
				} else {
					horizontalAlignment = HorizontalAlignment.Right;
					currentX = StartX + AvailableWidth - labelWidth;
				}

				Label label = ControlsFactory.CreateLabel(
					labels[i],
					Colors.Transparent,
					Properties.Settings.Default.ColorLabelForeground,
					FontFamilySub,
					FontSizeMain * 0.7,
					FontWeights.Normal,
					labelWidth,
					labelHeight,
					currentX,
					currentY,
					CanvasMain);

				label.HorizontalContentAlignment = horizontalAlignment;
				//label.VerticalContentAlignment = VerticalAlignment.Top;
			}




			int maxRate = 11;
			double rateSide = (AvailableWidth - (Gap * (maxRate - 1))) / maxRate;
			currentX = StartX;
			currentY -= rateSide;

			for (int i = 0; i < maxRate; i++) {
				Button button = ControlsFactory.CreateButtonWithTextOnly(
					i.ToString(),
					rateSide,
					rateSide,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal,
					currentX,
					currentY,
					CanvasMain);
				button.Tag = i;
				button.Click += Button_Click;

				Image image = null;
				if (i == 0)
					image = ControlsFactory.CreateImage(Properties.Resources.icon_dislike);
				if (i == 5)
					image = ControlsFactory.CreateImage(Properties.Resources.icon_dont_know);
				if (i == 10)
					image = ControlsFactory.CreateImage(Properties.Resources.icon_like);

				if (image != null)
					button.Content = image;

				//Brush brush = null;
				//if (i < 4) {
				//	brush = new SolidColorBrush(Colors.LightSkyBlue);
				//} else if (i > 6) {
				//	brush = new SolidColorBrush(Colors.LightSeaGreen);
				//}

				//if (brush != null)
				//	button.Background = brush;

				currentX += rateSide + Gap;
			}






			double labelQuestionHeight = DefaultButtonHeight * 2;
			currentY -= Gap + labelQuestionHeight;
			Label labelQuestion = ControlsFactory.CreateLabel(
				Properties.Resources.StringPageClinicRateQuestion,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				labelQuestionHeight,
				StartX,
				currentY,
				CanvasMain);
			//labelQuestion.VerticalContentAlignment = VerticalAlignment.Bottom;




			currentY -= Gap;
			double lastHeight = StartY + AvailableHeight - Canvas.GetTop(labelQuestion) - labelQuestion.Height - Gap;
			ControlsFactory.CreateImage(
				Properties.Resources.Recommend,
				AvailableWidth,
				currentY - StartY,
				StartX,
				StartY,
				CanvasMain,
				false);
			
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			PageThanks pageThanks = new PageThanks();
			NavigationService.Navigate(pageThanks);
		}
	}
}
