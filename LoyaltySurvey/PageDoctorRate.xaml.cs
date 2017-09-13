﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace LoyaltySurvey {
	/// <summary>
	/// Логика взаимодействия для PageDoctorRate.xaml
	/// </summary>
	public partial class PageDoctorRate : ClassPageTemplate {
		private Doctor doctor;

		public PageDoctorRate(Doctor doctor) {
			InitializeComponent();

			this.doctor = doctor;

			string docInfo = doctor.Name;
			if (!string.IsNullOrEmpty(doctor.Position))
				docInfo += ", " + doctor.Position;

			HideLogo();

			SetLabelsContent(
				Properties.Resources.StringPageDoctorRateTitle,
				Properties.Resources.StringPageDoctorRateSubtitle);

			List<string> rates = new List<string>() { "5", "4", "3", "2", "1" };

			double elementsInLine = rates.Count;
			double elementWidth = (AvailableWidth * 0.66 - Gap * (elementsInLine - 1)) / elementsInLine;
			double elementHeight = AvailableHeight * 0.35;

			currentX = StartX + (AvailableWidth * 0.33) / 2;
			currentY = StartY + AvailableHeight - elementHeight;

			foreach (string rate in rates) {
				Button buttonRate = ControlsFactory.CreateButtonWithImageAndText(
					rate, 
					elementWidth, 
					elementHeight,
					ControlsFactory.ElementType.Rate,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal,
					null,
					currentX,
					currentY,
					CanvasMain);

				buttonRate.Click += ButtonRate_Click;
				currentX += elementWidth + Gap;
			}

			Label labelDocInfo = ControlsFactory.CreateLabel(
				docInfo,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight,
				StartX,
				currentY - Gap - DefaultButtonHeight,
				CanvasMain);

			double imageSide = AvailableHeight - elementHeight - Gap * 2 - labelDocInfo.Height;
			Image docPhoto = ControlsFactory.CreateImage(
				(System.Drawing.Bitmap)ControlsFactory.GetImageForDoctor(doctor.Name),
				imageSide,
				imageSide,
				StartX + AvailableWidth / 2 - imageSide / 2,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonRate_Click(object sender, EventArgs e) {
			Console.WriteLine("PanelRate_Click");
			string tag = (sender as Control).Tag.ToString();
			Console.WriteLine("tag: " + tag);

			if (tag.Equals("1") || tag.Equals("2")) {
				PageComment pageComment = new PageComment();
				NavigationService.Navigate(pageComment);
			} else {
				PageClinicRate pageClinicRate = new PageClinicRate();
				NavigationService.Navigate(pageClinicRate);
			}
		}
	}
}