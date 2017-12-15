using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace LoyaltySurvey {
	public class EmotionObject {
		public class EmotionScore {
			public double Anger { get; set; }
			public double Contempt { get; set; }
			public double Disgust { get; set; }
			public double Fear { get; set; }
			public double Happiness { get; set; }
			public double Neutral { get; set; }
			public double Sadness { get; set; }
			public double Surprise { get; set; }

			public override string ToString() {
				return "Anger: " + string.Format("{0:P3}", Anger) + Environment.NewLine +
					"Contempt: " + string.Format("{0:P3}", Contempt) + Environment.NewLine +
					"Disgust: " + string.Format("{0:P3}", Disgust) + Environment.NewLine +
					"Fear: " + string.Format("{0:P3}", Fear) + Environment.NewLine +
					"Happiness: " + string.Format("{0:P3}", Happiness) + Environment.NewLine +
					"Neutral: " + string.Format("{0:P3}", Neutral) + Environment.NewLine +
					"Sadness: " + string.Format("{0:P3}", Sadness) + Environment.NewLine +
					"Surprise: " + string.Format("{0:P3}", Surprise);
			}
		}

		public class EmotionFaceRectangle {
			public int Height { get; set; }
			public int Left { get; set; }
			public int Top { get; set; }
			public int Width { get; set; }

			public override string ToString() {
				return "Height: " + Height + Environment.NewLine +
					"Left: " + Left + Environment.NewLine +
					"Top: " + Top + Environment.NewLine +
					"Width: " + Width;
			}
		}

		public EmotionFaceRectangle FaceRectangle { get; set; }
		public EmotionScore Scores { get; set; }

		public override string ToString() {
			return "FaceRectangle: " + FaceRectangle.ToString() + Environment.NewLine +
				"Scores: " + Scores;
		}
	}

	class SystemMsEmotioni {
		public static async Task<List<EmotionObject>> GetEmotions(string fileName) {
			SystemLogging.LogMessageToFile("MsEmotionApi.GetEmotions: " + fileName);

			List<EmotionObject> emotionObjects = new List<EmotionObject>();

			string emotionApiUrl = Properties.Settings.Default.EmotionApiUrl;
			string emotionApiKey = Properties.Settings.Default.EmotionApiKey;

			if (string.IsNullOrEmpty(emotionApiUrl) ||
				string.IsNullOrEmpty(emotionApiKey)) {
				SystemLogging.LogMessageToFile("В настройках не заданы параметры emotionApiUrl или emotionApiKey, пропуск");
				return emotionObjects;
			}

			if (string.IsNullOrEmpty(fileName)) {
				SystemLogging.LogMessageToFile("fileName is empty");
				return emotionObjects;
			}

			try {
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", emotionApiKey);
				HttpResponseMessage response;
				string responseContent;

				byte[] byteData = GetImageAsByteArray(fileName);

				using (ByteArrayContent content = new ByteArrayContent(byteData)) {
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
					response = await client.PostAsync(emotionApiUrl, content);
					responseContent = response.Content.ReadAsStringAsync().Result;

					if (!response.IsSuccessStatusCode) { 
						SystemLogging.LogMessageToFile("MsEmotionApi - не удалось получить ответ: " +
							"response.StatusCode: " + response.StatusCode + Environment.NewLine +
							"responseContent: " + responseContent);
						return emotionObjects;
					}
				}

				emotionObjects = JsonConvert.DeserializeObject<List<EmotionObject>>(responseContent);

				if (emotionObjects.Count == 0)
					SystemLogging.LogMessageToFile("Не удалось распознать лица людей");

				foreach (EmotionObject emotionObject in emotionObjects)
					SystemLogging.LogMessageToFile(emotionObject.ToString() + Environment.NewLine);
			} catch (Exception e) {
				SystemLogging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
			}

			return emotionObjects;
		}

		private static byte[] GetImageAsByteArray(string fileName) {
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			return binaryReader.ReadBytes((int)fileStream.Length);
		}
	}
}
