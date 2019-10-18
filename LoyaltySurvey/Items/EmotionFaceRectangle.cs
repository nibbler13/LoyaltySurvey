using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey.Items {
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
}
