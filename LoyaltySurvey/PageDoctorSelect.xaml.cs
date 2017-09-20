﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorSelect.xaml
	/// </summary>
	public partial class PageDoctorSelect : ClassPageTemplate {
		private List<Doctor> doctors;
		private WrapPanel wrapPanel;

		public PageDoctorSelect(List<Doctor> doctors, string depName) {
			InitializeComponent();

			HideLogo();

			SetLabelsContent(
				Properties.Resources.StringPageDoctorSelectTitle,
				Properties.Resources.StringPageDoctorSelectSubtitle);

			wrapPanel = new WrapPanel();

			Image imageDepartment = ControlsFactory.CreateImage((System.Drawing.Bitmap)ControlsFactory.GetImageForDepartment(depName));
			wrapPanel.Children.Add(imageDepartment);

			Label labelDep = ControlsFactory.CreateLabel(
				ControlsFactory.FirstCharToUpper(depName),
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				-1,
				-1,
				-1,
				-1,
				wrapPanel);
			
			wrapPanel.Height = DefaultButtonHeight;
			wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
			wrapPanel.VerticalAlignment = VerticalAlignment.Center;

			if (IsDebug)
				wrapPanel.Background = new SolidColorBrush(Colors.Aqua);

			Loaded += PageDoctorSelect_Loaded;
			Canvas.SetTop(wrapPanel, StartY);
			CanvasMain.Children.Add(wrapPanel);

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

		private void PageDoctorSelect_Loaded(object sender, RoutedEventArgs e) {
			Canvas.SetLeft(wrapPanel, StartX + AvailableWidth / 2 - wrapPanel.ActualWidth / 2);
		}

		private void PanelDoctor_Click(object sender, RoutedEventArgs e) {
			string docname = (sender as Control).Tag.ToString();
			LoggingSystem.LogMessageToFile("Выбран доктор: " + docname);
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
