using System;

namespace LoyaltySurvey {
	public class SurveyResult {
		public DateTime SurveyDateTime { get; private set; }
		public string DCode { get; private set; }
		public string DocName { get; private set; }
		public string DocDepartment { get; private set; }
		public string DocDeptCode { get; private set; }
		public string DocRate { get; private set; }
		public string PhotoLink { get; private set; }
		public string Comment { get; private set; }
		public string PhoneNumber { get; private set; }
		public string ClinicRecommendMark { get; private set; }

		public SurveyResult(DateTime dateTime, string dCode, string docName, 
			string docRate, string docDepartment, string docDeptCode) {
			SurveyDateTime = dateTime;
			DCode = dCode;
			DocName = docName;
			DocRate = docRate;
			DocDepartment = docDepartment;
			DocDeptCode = docDeptCode;
		}

		public void SetPhotoLink(string link) {
			PhotoLink = link;
		}

		public void SetComment(string comment) {
			Comment = comment;
		}

		public void SetPhoneNumber(string phoneNumber) {
			PhoneNumber = phoneNumber;
		}

		public void SetClinicRecommendMark(string mark) {
			ClinicRecommendMark = mark;
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
				"ClinicRecommendMark: " + ClinicRecommendMark;
		}
	}
}
