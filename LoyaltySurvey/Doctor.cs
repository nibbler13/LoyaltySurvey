using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltySurvey {
	public class Doctor {
		public string Name { get; private set; }
		public string Position { get; private set; }
		public string Department { get; private set; }
		public string Id { get; private set; }
		
		public Doctor(string name, string position, string department, string id) {
			Name = name;
			Position = position;
			Department = department;
			Id = id;
		}
	}
}
