using DirectShowLib;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey.Utilities {
	public class WebCam {
		private readonly ItemSurveyResult surveyResult;

		public WebCam(ItemSurveyResult surveyResult) {
			this.surveyResult = surveyResult;
		}

		public void CaptureImageFromWebCamAndSave() {
			string photoSavePath = Properties.Settings.Default.PathWebCamSaveTo;

			if (!Properties.Settings.Default.IsDebug) {
				DsDevice[] dsDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
				if (dsDevices.Length == 0) {
					surveyResult.PhotoLink = "Camera isn't installed";
					Logging.ToLog("CaptureImageFromWebCamAndSave: There is no video input device available");
					return;
				}

				if (string.IsNullOrEmpty(photoSavePath))
					photoSavePath = Directory.GetCurrentDirectory() + "\\Photos\\";

				if (!photoSavePath.EndsWith("\\"))
					photoSavePath += "\\";

				if (!Directory.Exists(photoSavePath)) {
					try {
						Directory.CreateDirectory(photoSavePath);
					} catch (Exception e) {
						Logging.ToLog("CaptureImageFromWebCamAndSave exception: " + e.Message +
							Environment.NewLine + e.StackTrace);
						return;
					}
				}

				string fileName = "photo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
				photoSavePath += fileName;
			} else
				photoSavePath = @"\\MSSU-INFOMON-3\Photos\photo_20171203_160411.jpg";

			surveyResult.PhotoLink = photoSavePath;

#pragma warning disable CA2000 // Dispose objects before losing scope
			BackgroundWorker backgroundWorker = new BackgroundWorker();
#pragma warning restore CA2000 // Dispose objects before losing scope
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
			backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			(sender as BackgroundWorker).Dispose();
		}

		private bool IsEmpty(System.Drawing.Bitmap image) {
			var data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
				System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
			var bytes = new byte[data.Height * data.Stride];
			Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
			image.UnlockBits(data);
			return bytes.All(x => x == 0);
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				if (!Properties.Settings.Default.IsDebug) {
					string savePath = surveyResult.PhotoLink;

					Logging.ToLog("Получение изображения с веб-камеры и сохранение в файл: " + savePath);

					VideoCapture videoCapture = new VideoCapture();
					System.Drawing.Bitmap bitmap = null;

					//wait for camera to be active and get non blank screen
					for (int i = 0; i < 5; i++) {
						bitmap = videoCapture.QueryFrame().Bitmap;

						if (IsEmpty(bitmap))
							continue;

						bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					}
					
					bitmap.Dispose();
					videoCapture.Dispose();
				}

				List<Items.EmotionObject> emotionObjects = MsEmotioni.GetEmotions(surveyResult.PhotoLink).Result;
				if (emotionObjects.Count == 0)
					return;

				surveyResult.EmotionObject = emotionObjects[0];

				if (!surveyResult.IsInsertedToDb)
					return;

				string updateQuery = Properties.Settings.Default.SqlUpdateEmotion;

				Dictionary<string, object> parameters = new Dictionary<string, object>() {
					{ "@em_anger", surveyResult.EmotionObject.Scores.Anger },
					{ "@em_contempt", surveyResult.EmotionObject.Scores.Contempt },
					{ "@em_disgust", surveyResult.EmotionObject.Scores.Disgust },
					{ "@em_fear", surveyResult.EmotionObject.Scores.Fear },
					{ "@em_happiness", surveyResult.EmotionObject.Scores.Happiness },
					{ "@em_neutral", surveyResult.EmotionObject.Scores.Neutral },
					{ "@em_sadness", surveyResult.EmotionObject.Scores.Sadness },
					{ "@em_surprice", surveyResult.EmotionObject.Scores.Surprise},
					{ "@photoPath", surveyResult.PhotoLink }
				};

				using (ClientFirebird fBClient = new ClientFirebird(
					Properties.Settings.Default.MisInfoclinicaDbAddress,
					Properties.Settings.Default.MisInfoclinicaDbName,
					Properties.Settings.Default.MisInfoclinicaDbUser,
					Properties.Settings.Default.MisInfoclinicaDbPassword)) {
					bool result = fBClient.ExecuteUpdateQuery(updateQuery, parameters);
					Logging.ToLog("Результат записи оценок эмоций: " + result);
				}
			} catch (Exception exception) {
				Logging.ToLog("BackgroundWorker_DoWork exception: " + exception.Message +
					Environment.NewLine + exception.StackTrace);
			}
		}
	}
}
