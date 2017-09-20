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
	public partial class PageDoctorRate : ClassPageTemplate {
		private Doctor doctor;

		public PageDoctorRate(Doctor doctor) {
			InitializeComponent();

			this.doctor = doctor;

			string docInfo = doctor.Name;
			if (!string.IsNullOrEmpty(doctor.Position))
				docInfo += ", " + doctor.Position;

			HideLogo();

			SetLabelsContent(
				Properties.Resources.StringPageDoctorRateTitle,
				Properties.Resources.StringPageDoctorRateSubtitle);

			List<string> rates = new List<string>() { "5", "4", "3", "2", "1" };

			double elementsInLine = rates.Count;
			double elementWidth = (AvailableWidth * 0.66 - Gap * (elementsInLine - 1)) / elementsInLine;
			double elementHeight = AvailableHeight * 0.35;

			double currentX = StartX + (AvailableWidth * 0.33) / 2;
			double currentY = StartY + AvailableHeight - elementHeight;

			foreach (string rate in rates) {
				Button buttonRate = ControlsFactory.CreateButtonWithImageAndText(
					rate, 
					elementWidth, 
					elementHeight,
					ControlsFactory.ElementType.Rate,
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

			Label labelDocInfo = ControlsFactory.CreateLabel(
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
			Image docPhoto = ControlsFactory.CreateImage(
				(System.Drawing.Bitmap)ControlsFactory.GetImageForDoctor(doctor.Name),
				imageSide,
				imageSide,
				StartX + AvailableWidth / 2 - imageSide / 2,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonRate_Click(object sender, EventArgs e) {
			string tag = (sender as Control).Tag.ToString();
			LoggingSystem.LogMessageToFile("Выбрана оценка: " + tag);
			surveyResult = new SurveyResult(DateTime.Now, doctor.Code, doctor.Name, tag);
			Page page;

			if (Properties.Settings.Default.WebCamWriteAll)
				CaptureImageFromWebCamAndSave(ref surveyResult);
			else if ((tag.Equals("1") || tag.Equals("2")) &&
				Properties.Settings.Default.WebCamWriteOnlyNegative)
				CaptureImageFromWebCamAndSave(ref surveyResult);
			else
				surveyResult.SetPhotoLink("don't need");

			if (tag.Equals("3") ||
				tag.Equals("4") ||
				tag.Equals("5")) {
				surveyResult.SetComment("Don't need");
				surveyResult.SetPhoneNumber("Don't need");
				page = new PageClinicRate(surveyResult);
			} else {
				page = new PageComment(surveyResult);
			}

			NavigationService.Navigate(page);
		}

		private void CaptureImageFromWebCamAndSave(ref SurveyResult surveyResult) {
			DsDevice[] dsDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
			if (dsDevices.Length == 0) {
				surveyResult.SetPhotoLink("Camera not installed");
				LoggingSystem.LogMessageToFile("CaptureImageFromWebCamAndSave: There is no video input device available");
				return;
			}

			string photoSavePath = Properties.Settings.Default.WebCamSavePath;

			if (string.IsNullOrEmpty(photoSavePath))
				photoSavePath = Directory.GetCurrentDirectory() + "\\Photos\\";

			if (!photoSavePath.EndsWith("\\"))
				photoSavePath += "\\";

			if (!Directory.Exists(photoSavePath)) {
				try {
					Directory.CreateDirectory(photoSavePath);
				} catch (Exception e) {
					LoggingSystem.LogMessageToFile("CaptureImageFromWebCamAndSave exception: " + e.Message + 
						Environment.NewLine + e.StackTrace);
					return;
				}
			}

			string fileName = "photo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
			photoSavePath += fileName;
			surveyResult.SetPhotoLink(photoSavePath);

			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync(photoSavePath);
			backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			(sender as BackgroundWorker).Dispose();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				string savePath = (string)e.Argument;

				LoggingSystem.LogMessageToFile("Получение изображения с веб-камеры и сохранение в файл: " + savePath);
				VideoCapture videoCapture = new VideoCapture();
				System.Drawing.Bitmap bitmap = videoCapture.QueryFrame().Bitmap;
				bitmap.Save(savePath);
				bitmap.Dispose();
				videoCapture.Dispose();
			} catch (Exception exception) {
				LoggingSystem.LogMessageToFile("BackgroundWorker_DoWork exception: " + exception.Message + 
					Environment.NewLine + exception.StackTrace);
			}

		}
	}
}
