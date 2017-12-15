using System;

namespace LoyaltySurvey {
	public class ItemSurveyResult {
		public DateTime SurveyDateTime { get; private set; }
		public string DCode { get; private set; }
		public string DocName { get; private set; }
		public string DocDepartment { get; private set; }
		public string DocDeptCode { get; private set; }
		public string DocRate { get; set; }
		public string PhotoLink { get; set; }
		public string Comment { get; set; }
		public string PhoneNumber { get; set; }
		public string ClinicRecommendMark { get; set; }
		public EmotionObject EmotionObject { get; set; }
		public bool IsInsertedToDb { get; set; }

		public ItemSurveyResult(DateTime dateTime, string dCode, string docName, 
			string docRate, string docDepartment, string docDeptCode) {
			SurveyDateTime = dateTime;
			DCode = dCode;
			DocName = docName;
			DocRate = docRate;
			DocDepartment = docDepartment;
			DocDeptCode = docDeptCode;
			IsInsertedToDb = false;
		}

		public override string ToString() {
			return base.ToString() + Environment.NewLine +
				"SurveyDateTime: " + SurveyDateTime.ToString() + Environment.NewLine +
				"DCode: " + DCode + Environment.NewLine +
				"DocName: " + DocName + Environment.NewLine +
				"DocDepartment: " + DocDepartment + Environment.NewLine +
				"DocDeptCode: " + DocDeptCode + Environment.NewLine +
				"DocRate: " + DocRate + Environment.NewLine +
				"PhotoLink: " + PhotoLink + Environment.NewLine +
				"Comment: " + Comment + Environment.NewLine +
				"PhoneNumber: " + PhoneNumber + Environment.NewLine +
				"ClinicRecommendMark: " + ClinicRecommendMark + Environment.NewLine +
				"EmotionObject: " + (EmotionObject == null ? "null" : EmotionObject.ToString());
		}
	}
}
