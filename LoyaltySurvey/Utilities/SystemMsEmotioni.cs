using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using LoyaltySurvey.Items;

namespace LoyaltySurvey {
	class SystemMsEmotioni {
		public static async Task<List<EmotionObject>> GetEmotions(string fileName) {
			SystemLogging.ToLog("MsEmotionApi.GetEmotions: " + fileName);

			List<EmotionObject> emotionObjects = new List<EmotionObject>();

			string emotionApiUrl = Properties.Settings.Default.EmotionApiUrl;
			string emotionApiKey = Properties.Settings.Default.EmotionApiKey;

			if (string.IsNullOrEmpty(emotionApiUrl) ||
				string.IsNullOrEmpty(emotionApiKey)) {
				SystemLogging.ToLog("В настройках не заданы параметры emotionApiUrl или emotionApiKey, пропуск");
				return emotionObjects;
			}

			if (string.IsNullOrEmpty(fileName)) {
				SystemLogging.ToLog("fileName is empty");
				return emotionObjects;
			}

			try {
				using (HttpClient client = new HttpClient()) {
					client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", emotionApiKey);
					string responseContent;

					byte[] byteData = GetImageAsByteArray(fileName);

					using (ByteArrayContent content = new ByteArrayContent(byteData)) {
						content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
						using (HttpResponseMessage response = await client.PostAsync(new Uri(emotionApiUrl), content).ConfigureAwait(false)) {

							responseContent = response.Content.ReadAsStringAsync().Result;

							if (!response.IsSuccessStatusCode) {
								SystemLogging.ToLog("MsEmotionApi - не удалось получить ответ: " +
									"response.StatusCode: " + response.StatusCode + Environment.NewLine +
									"responseContent: " + responseContent);
								return emotionObjects;
							}

							emotionObjects = JsonConvert.DeserializeObject<List<EmotionObject>>(responseContent);
						}
					}

					if (emotionObjects.Count == 0)
						SystemLogging.ToLog("Не удалось распознать лица людей");

					foreach (EmotionObject emotionObject in emotionObjects)
						SystemLogging.ToLog(emotionObject.ToString() + Environment.NewLine);
				}
			} catch (Exception e) {
				SystemLogging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			return emotionObjects;
		}

		private static byte[] GetImageAsByteArray(string fileName) {
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
				return binaryReader.ReadBytes((int)fileStream.Length);
		}
	}
}
