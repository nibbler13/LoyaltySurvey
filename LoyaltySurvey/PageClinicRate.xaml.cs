using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

			double height = DefaultButtonHeight * 2;
			Label labelQuestion = ControlsFactory.CreateLabel(
				Properties.Resources.StringPageClinicRateQuestion,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				height,
				StartX,
				StartY + AvailableHeight / 2 - height / 2,
				CanvasMain);
			labelQuestion.VerticalContentAlignment = VerticalAlignment.Bottom;

			ControlsFactory.CreateImage(
				Properties.Resources.Recommend,
				AvailableWidth,
				Canvas.GetTop(labelQuestion) - StartY - Gap,
				StartX,
				StartY,
				CanvasMain,
				false);

			int maxRate = 11;
			double rateSide = (AvailableWidth - (Gap * (maxRate - 1))) / maxRate;
			double lastHeight = StartY + AvailableHeight - Canvas.GetTop(labelQuestion) - labelQuestion.Height - Gap;
			currentX = StartX;
			currentY = StartY + AvailableHeight - lastHeight / 2 - rateSide / 2;

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

				currentX += rateSide + Gap;
			}

			currentX = StartX;
			currentY += rateSide + Gap / 2;

			List<string> labels = new List<string>() {
				Properties.Resources.StringPageClinicRateNotAtAll,
				Properties.Resources.StringPageClinicRateDontKnow,
				Properties.Resources.StringPageClinicRateMostLikely
			};

			double labelWidth = rateSide * 3;
			double labelHeight = rateSide * 3;
			for (int i = 0; i < labels.Count; i++) {
				HorizontalAlignment horizontalAlignment;
				if (i == 0) {
					horizontalAlignment = HorizontalAlignment.Left;
					currentX = StartX;
				} else if (i == 1) {
					horizontalAlignment = HorizontalAlignment.Center;
					currentX = StartX + AvailableWidth / 2 - labelWidth / 2;
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
				label.VerticalContentAlignment = VerticalAlignment.Top;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			PageThanks pageThanks = new PageThanks();
			NavigationService.Navigate(pageThanks);
		}
	}
}
