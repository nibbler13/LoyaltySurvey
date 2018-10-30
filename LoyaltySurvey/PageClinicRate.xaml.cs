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
	public partial class PageClinicRate : PageTemplate {
		public PageClinicRate(ItemSurveyResult surveyResult) {
			InitializeComponent();

			this._surveyResult = surveyResult;

			HideLogo();
			//HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageClinicRateTitle,
				Properties.Resources.StringPageClinicRateSubtitle);
			
			double labelWidth = DefaultButtonWidth * 4;
			double labelHeight = DefaultButtonHeight / 2;
			double currentX = StartX;
			double currentY = StartY + AvailableHeight;

			//List<string> labels = new List<string>() {
			//	Properties.Resources.StringPageClinicRateNotAtAll,
			//	Properties.Resources.StringPageClinicRateDontKnow,
			//	Properties.Resources.StringPageClinicRateMostLikely
			//};

			//for (int i = 0; i < labels.Count; i++) {
			//	HorizontalAlignment horizontalAlignment;
			//	if (i == 0) {
			//		horizontalAlignment = HorizontalAlignment.Left;
			//		currentX = StartX;
			//	} else if (i == 1) {
			//		horizontalAlignment = HorizontalAlignment.Center;
			//		currentX = StartX + AvailableWidth / 2 - labelWidth / 2;
			//	} else {
			//		horizontalAlignment = HorizontalAlignment.Right;
			//		currentX = StartX + AvailableWidth - labelWidth;
			//	}

			//	Label label = PageControlsFactory.CreateLabel(
			//		labels[i],
			//		Colors.Transparent,
			//		Properties.Settings.Default.ColorLabelForeground,
			//		FontFamilySub,
			//		FontSizeMain * 0.7,
			//		FontWeights.Normal,
			//		labelWidth,
			//		labelHeight,
			//		currentX,
			//		currentY,
			//		CanvasMain);

			//	label.HorizontalContentAlignment = horizontalAlignment;
			//}

			int maxRate = 11;
			double rateSide = (AvailableWidth - (Gap * (maxRate - 1))) / maxRate;
			currentX = StartX;
			currentY -= rateSide;

			for (int i = 0; i < maxRate; i++) {
				Button button = PageControlsFactory.CreateButtonWithTextOnly(
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
					image = PageControlsFactory.CreateImage(Properties.Resources.ClinicRate0);
				if (i == 5)
					image = PageControlsFactory.CreateImage(Properties.Resources.ClinicRate5);
				if (i == 10)
					image = PageControlsFactory.CreateImage(Properties.Resources.ClinicRate10);

				if (image != null)
					button.Content = image;

				currentX += rateSide + Gap;
			}

			PageControlsFactory.CreateImage(
				Properties.Resources.BackgroundClinicRecommend,
				AvailableWidth,
				currentY - StartY - Gap,
				StartX,
				StartY,
				CanvasMain,
				false);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			string tag = (sender as Button).Tag.ToString();
			SystemLogging.ToLog("Выбрана оценка: " + tag);
			_surveyResult.ClinicRecommendMark = tag;

			PageThanks pageThanks = new PageThanks(_surveyResult);
			NavigationService.Navigate(pageThanks);
		}
	}
}
