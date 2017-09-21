using DirectShowLib;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public class WebCam {
		public static void CaptureImageFromWebCamAndSave(ref SurveyResult surveyResult) {
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

		private static void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			(sender as BackgroundWorker).Dispose();
		}

		private static void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				string savePath = (string)e.Argument;

				LoggingSystem.LogMessageToFile("Получение изображения с веб-камеры и сохранение в файл: " + savePath);

				VideoCapture videoCapture = new VideoCapture();
				System.Drawing.Bitmap bitmap = videoCapture.QueryFrame().Bitmap;
				bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
				bitmap.Dispose();
				videoCapture.Dispose();
			} catch (Exception exception) {
				LoggingSystem.LogMessageToFile("BackgroundWorker_DoWork exception: " + exception.Message +
					Environment.NewLine + exception.StackTrace);
			}

		}
	}
}
