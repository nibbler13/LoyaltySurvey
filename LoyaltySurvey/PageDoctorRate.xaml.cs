using DirectShowLib;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorRate.xaml
	/// </summary>
	public partial class PageDoctorRate : PageTemplate {
		private ItemDoctor doctor;

		public PageDoctorRate(ItemDoctor doctor) {
			InitializeComponent();

			this.doctor = doctor;

			string docInfo = doctor.Name;
			if (!string.IsNullOrEmpty(doctor.Position))
				docInfo += ", " + doctor.Position;

			HideLogo();

			SetLabelsContent(
				Properties.Resources.StringPageDoctorRateTitle,
				Properties.Resources.StringPageDoctorRateSubtitle);

			List<string> rates = new List<string>() { "1", "2", "3", "4", "5" };

			double elementsInLine = rates.Count;
			double elementWidth = (AvailableWidth * 0.66 - Gap * (elementsInLine - 1)) / elementsInLine;
			double elementHeight = AvailableHeight * 0.35;

			double currentX = StartX + (AvailableWidth * 0.33) / 2;
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

			Label labelDocInfo = PageControlsFactory.CreateLabel(
				docInfo,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight,
				StartX,
				currentY - Gap - DefaultButtonHeight,
				CanvasMain);

			double imageSide = AvailableHeight - elementHeight - Gap * 2 - labelDocInfo.Height;
			Image docPhoto = PageControlsFactory.CreateImage(
				(System.Drawing.Bitmap)PageControlsFactory.GetImageForDoctor(doctor.Code),
				imageSide,
				imageSide,
				StartX + AvailableWidth / 2 - imageSide / 2,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonRate_Click(object sender, EventArgs e) {
			string tag = (sender as Control).Tag.ToString();
			SystemLogging.LogMessageToFile("Выбрана оценка: " + tag);
			_surveyResult = new ItemSurveyResult(DateTime.Now, doctor.Code, doctor.Name, tag, doctor.Department, doctor.DeptCode);
			Page page;

			SystemWebCam webCam = new SystemWebCam(_surveyResult);
			if (Properties.Settings.Default.WebCamWriteAll)
				webCam.CaptureImageFromWebCamAndSave();
			else if ((tag.Equals("1") || tag.Equals("2")) &&
				Properties.Settings.Default.WebCamWriteOnlyNegative)
				webCam.CaptureImageFromWebCamAndSave();
			else
				_surveyResult.PhotoLink = "Don't need";

			if (tag.Equals("3") ||
				tag.Equals("4") ||
				tag.Equals("5")) {
				_surveyResult.Comment = "Don't need";
				_surveyResult.PhoneNumber = "Don't need";

				if (((MainWindow)Application.Current.MainWindow).previousRatesDcodes.Count > 0) {
					_surveyResult.ClinicRecommendMark = "Don't need";
					page = new PageThanks(_surveyResult);
				} else
					page = new PageClinicRate(_surveyResult);
			} else {
				page = new PageComment(_surveyResult);
			}

			NavigationService.Navigate(page);
		}
	}
}
