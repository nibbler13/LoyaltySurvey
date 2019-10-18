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
using LoyaltySurvey.Pages.Helpers;

namespace LoyaltySurvey.Pages {
	/// <summary>
	/// Логика взаимодействия для PageSelectSurvey.xaml
	/// </summary>
	public partial class PageSelectSurvey : PageTemplate {
		public PageSelectSurvey(PageDepartmentSelect pageDepartmentSelect) {
			InitializeComponent();
			HideButtonBack();

			SetLabelsContent(
				Properties.Resources.StringPageSelectSurveyTitle,
				Properties.Resources.StringPageSelectSurveySubtitle);

			SystemLogging.ToLog("Отображение страницы с выбором типа опроса");

			double buttonWidth = AvailableWidth / 3;
			double buttonHeight = AvailableHeight / 2;
			double startX = StartX + (AvailableWidth - buttonWidth * 2 - Gap) / 2;
			double startY = StartY + (AvailableHeight - buttonHeight) / 2;

			Button buttonDoctorRate = PageControlsFactory.CreateButtonWithImageAndText(
				Properties.Resources.StringPageSelectSurveyDoctorsRate,
				buttonWidth,
				buttonHeight,
				PageControlsFactory.ElementType.SurveySelect,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.Doctors,
				startY,
				startY,
				CanvasMain);

			startX += buttonWidth + Gap;
			
			Button buttonRegistryRate = PageControlsFactory.CreateButtonWithImageAndText(
				Properties.Resources.StringPageSelectSurveyRegistryRate,
				buttonWidth,
				buttonHeight,
				PageControlsFactory.ElementType.SurveySelect,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.PicRegistry,
				startX,
				startY,
				CanvasMain);

			buttonDoctorRate.Click += (s, e) => { NavigationService.Navigate(pageDepartmentSelect); };
			buttonRegistryRate.Click += (s, e) => { NavigationService.Navigate(new PageRegistryRate()); };
		}
	}
}
