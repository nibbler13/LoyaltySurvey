using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LoyaltySurvey.Pages.Helpers;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageThanks.xaml
	/// </summary>
	public partial class PageThanks : PageTemplate {
		public PageThanks(ItemSurveyResult surveyResult) {
			InitializeComponent();

			this.SurveyResult = surveyResult ?? throw new ArgumentNullException(nameof(surveyResult));

			Rect rect = CreateFirstOrLastPageControls(
				Properties.Resources.StringPageThanksTitleLeftTop,
				Properties.Resources.StringPageThanksTitleLeftBottom,
				Properties.Resources.StringPageThanksTitleLeftRight,
				Properties.Resources.StringPageThanksSubtitle,
				false);

			Image imageThanks = PageControlsFactory.CreateImage(
				Properties.Resources.BackgroundThanks,
				rect.Width,
				rect.Height,
				rect.Location.X,
				rect.Location.Y,
				CanvasMain,
				false);

			if (IsDebug) {
				Label surveyLabel = PageControlsFactory.CreateLabel(surveyResult.ToString(), Colors.White, Colors.DarkGray, FontFamilySub, FontSizeMain / 2,
					FontWeights.Normal, AvailableWidth / 2, AvailableHeight / 2, StartX, StartY, CanvasMain);
				(surveyLabel.Content as TextBlock).TextAlignment = TextAlignment.Left;
				surveyLabel.VerticalContentAlignment = VerticalAlignment.Top;
				surveyLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
			}

			DisablePageAutoCloseTimerResetByClick();
			PreviewMouseDown += PageThanks_PreviewMouseDown;
		}

		private void PageThanks_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			AutoCloseAllPages(true);
		}
	}
}
