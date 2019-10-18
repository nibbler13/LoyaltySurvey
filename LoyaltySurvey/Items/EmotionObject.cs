using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey.Items {
	public class EmotionObject {
		public EmotionFaceRectangle FaceRectangle { get; set; }
		public EmotionScore Scores { get; set; }

		public override string ToString() {
			return "FaceRectangle: " + FaceRectangle.ToString() + Environment.NewLine +
				"Scores: " + Scores;
		}
	}
}
