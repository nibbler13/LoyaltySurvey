﻿using System;
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
using LoyaltySurvey.Pages.Helpers;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageError.xaml
	/// </summary>
	public partial class PageError : PageTemplate {
		public PageError() {
			InitializeComponent();

			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageErrorTitle,
				Properties.Resources.StringPageErrorSubtitle);

			Label labelMessage = PageControlsFactory.CreateLabel(
				Properties.Resources.StringPageErrorMessage,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight * 1.5,
				StartX,
				StartY + AvailableHeight - DefaultButtonHeight * 1.5,
				CanvasMain);

			PageControlsFactory.CreateImage(
				Properties.Resources.BackgroundError,
				AvailableWidth,
				AvailableHeight - Gap - labelMessage.Height,
				StartX,
				StartY,
				CanvasMain,
				false);

			DisablePageAutoCloseTimer();
			DisablePageAutoCloseTimerResetByClick();
		}
	}
}
