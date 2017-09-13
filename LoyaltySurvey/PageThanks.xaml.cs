using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageThanks.xaml
	/// </summary>
	public partial class PageThanks : ClassPageTemplate {
		private DispatcherTimer dispatcherTimer;

		public PageThanks() {
			InitializeComponent();

			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageThanksTitle,
				Properties.Resources.StringPageThanksSubtitle);
			
			Image temp = ControlsFactory.CreateImage(
				Properties.Resources.thanks_2,
				AvailableWidth,
				AvailableHeight,
				StartX,
				StartY,
				CanvasMain,
				false);

			dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Start();
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e) {
			dispatcherTimer.Stop();
			CloseAllFormsExceptMain();
		}
	}
}
