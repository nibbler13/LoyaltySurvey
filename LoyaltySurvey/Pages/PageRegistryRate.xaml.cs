﻿using System;
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
using LoyaltySurvey.Pages.Helpers;
using LoyaltySurvey.Utilities;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageRegistryRate.xaml
	/// </summary>
	public partial class PageRegistryRate : PageTemplate {
		public PageRegistryRate() {
			InitializeComponent();

			HideLogo();

			SetLabelsContent(
				Properties.Resources.StringPageRegistryRateTitle,
				Properties.Resources.StringPageRegistryRateSubitle);

			List<string> rates = new List<string>() { "2", "3", "4" };

			double elementsInLine = 5;// rates.Count;
			double elementWidth = (AvailableWidth * 0.66 - Gap * (elementsInLine - 1)) / elementsInLine;
			double elementHeight = AvailableHeight * 0.35;

			double currentX = StartX + (AvailableWidth * 0.33) / 2 + elementWidth + Gap;
			double currentY = StartY + AvailableHeight - elementHeight;

			foreach (string rate in rates) {
				Button buttonRate = PageControlsFactory.CreateButtonWithImageAndText(
					rate,
					elementWidth,
					elementHeight,
					PageControlsFactory.ElementType.Rate,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal,
					null,
					currentX,
					currentY,
					CanvasMain);

				buttonRate.Click += ButtonRate_Click;
				currentX += elementWidth + Gap;
			}

			double imageSide = AvailableHeight - elementHeight - Gap * 2;
			Image docPhoto = PageControlsFactory.CreateImage(
				Properties.Resources.PicRegistry,
				imageSide,
				imageSide,
				StartX + AvailableWidth / 2 - imageSide / 2,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonRate_Click(object sender, EventArgs e) {
			string tag = (sender as Control).Tag.ToString();
			string dCode = "0";
			string depNum = (Properties.Settings.Default.ClinicRestrictions1AdultOnly2ChildOnly - 1).ToString();
			
			Logging.ToLog("Выбрана оценка: " + tag);
			SurveyResult = new ItemSurveyResult(ItemSurveyResult.Type.Registry, DateTime.Now, dCode, "Регистратура", tag, string.Empty, depNum);
			Page page;

			WebCam webCam = new WebCam(SurveyResult);
			if (Properties.Settings.Default.WebCamWriteAll)
				webCam.CaptureImageFromWebCamAndSave();
			else if ((tag.Equals("1") || tag.Equals("2")) &&
				Properties.Settings.Default.WebCamWriteOnlyNegative)
				webCam.CaptureImageFromWebCamAndSave();
			else
				SurveyResult.PhotoLink = "Don't need";

			if (tag.Equals("3") ||
				tag.Equals("4") ||
				tag.Equals("5")) {
				SurveyResult.Comment = "Don't need";
				SurveyResult.PhoneNumber = "Don't need";

				if (((MainWindow)Application.Current.MainWindow).PreviousRatesDcodes.Count > 0) {
					SurveyResult.ClinicRecommendMark = "Don't need";
					page = new PageThanks(SurveyResult);
				} else
					page = new PageClinicRate(SurveyResult);
			} else {
				page = new PageComment(SurveyResult);
			}

			NavigationService.Navigate(page);
		}
	}
}
