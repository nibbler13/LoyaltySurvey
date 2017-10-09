using System;

namespace LoyaltySurvey {
	public class Doctor {
		private string _name;
		public string Name {
			get {
				string clearedName = _name.Replace("  ", " ").TrimStart(' ').TrimEnd(' ');
				if (clearedName.Contains("("))
					clearedName = clearedName.Substring(0, clearedName.IndexOf('('));

				string[] parts = clearedName.Split(' ');

				if (parts.Length < 3)
					return _name;

				return parts[0] + " " + parts[1] + " " + parts[2];
			}
			private set {
				_name = value;
			}
		}

		public string Position { get; private set; }
		public string Department { get; private set; }
		public string DeptCode { get; private set; }
		public string Code { get; private set; }
		
		public Doctor(string name, string position, string department, string code, string deptCode) {
			Name = name;
			Position = position;
			Department = department;
			Code = code;
			DeptCode = deptCode;
		}
	}
}
