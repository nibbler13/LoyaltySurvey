using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorSelect.xaml
	/// </summary>
	public partial class PageDoctorSelect : ClassPageTemplate {
		private List<Doctor> doctors;

		public PageDoctorSelect(List<Doctor> doctors, string depName) {
			InitializeComponent();

			HideLogo();

			string title = Properties.Resources.StringPageDoctorSelectTitle;
			if (title.Contains("*"))
				title = title.Replace("*", depName);

			SetLabelsContent(
				title,
				Properties.Resources.StringPageDoctorSelectSubtitle);

			Label labelDep = ControlsFactory.CreateLabel(
				"Отделение: " + depName,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonWidth,
				StartX,
				StartY,
				CanvasMain);

			this.doctors = doctors;
			
			CreateRootPanel(
				Properties.Settings.Default.PageDoctorSelectElementsInLine,
				Properties.Settings.Default.PageDoctorSelectElementsLinesCount,
				doctors.Count);

			List<string> keys = new List<string>();
			foreach (Doctor doctor in doctors)
				keys.Add(doctor.Name);

			FillPanelWithElements(keys, ControlsFactory.ElementType.Doctor, PanelDoctor_Click);
		}

		private void PanelDoctor_Click(object sender, RoutedEventArgs e) {
			string docname = (sender as Control).Tag.ToString();
			Doctor selectedDoctor = new Doctor("", "", "", "");

			foreach (Doctor doctor in doctors) {
				if (doctor.Name.Equals(docname)) {
					selectedDoctor = doctor;
					break;
				}
			}

			PageDoctorRate pageDoctorRate = new PageDoctorRate(selectedDoctor);
			NavigationService.Navigate(pageDoctorRate);
		}
	}
}
