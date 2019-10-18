using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey.Items {
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
}
