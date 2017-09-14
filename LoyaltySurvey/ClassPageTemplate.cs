using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LoyaltySurvey {
	public partial class ClassPageTemplate : Page {
		protected double ScreenWidth { get; private set; }
		protected double ScreenHeight { get; private set; }
		protected double Gap { get; private set; }
		protected double HeaderHeight { get; private set; }
		protected double FontSizeMain { get; private set; }
		protected double StartX { get; private set; }
		protected double StartY { get; private set; }
		protected double AvailableWidth { get; private set; }
		protected double AvailableHeight { get; private set; }
		protected Canvas CanvasMain { get; private set; }

		protected FontFamily FontFamilyMain { get; private set; }
		protected FontFamily FontFamilySub { get; private set; }
		protected double DefaultButtonWidth { get; private set; }
		protected double DefaultButtonHeight { get; private set; }
		protected bool IsDebug { get; private set; }

		protected ScrollViewer ScrollViewer { get; private set; }
		protected WrapPanel CanvasForElements { get; private set; }
		protected double ElementWidth { get; private set; }
		protected double ElementHeight { get; private set; }

		private Label labelTitle;
		private Label labelSubtitle;
		private Image imageLogo;
		private Image imageBottomLineSolid;
		private Image imageBottomLineColors;
		private Button buttonBack;

		private Button buttonScrollUp;
		private Button buttonScrollDown;
		private Button buttonScrollLeft;
		private Button buttonScrollRight;
		
		private Image imageQuestion;
		private Label labelQuestion;
		private Button buttonNoQuestion;
		private Button buttonYesQuestion;

		protected double leftCornerShadow = 0;
		protected double rightCornerShadow = 5;
		private double elementsInLine = 0;
		private double elementsLineCount = 0;



		public ClassPageTemplate() {
			ScreenWidth = SystemParameters.PrimaryScreenWidth;
			ScreenHeight = SystemParameters.PrimaryScreenHeight;
			Gap = ScreenHeight * 0.03;
			FontSizeMain = ScreenHeight * 0.033;
			HeaderHeight = FontSizeMain * 3;
			DefaultButtonHeight = FontSizeMain * 2;
			DefaultButtonWidth = DefaultButtonHeight;

			FontFamilyMain = new FontFamily(Properties.Settings.Default.FontMain.Name);
			FontFamilySub = new FontFamily(Properties.Settings.Default.FontSub.Name);
			IsDebug = Properties.Settings.Default.IsDebug;

			CanvasMain = new Canvas();
			CanvasMain.Width = ScreenWidth;
			CanvasMain.Height = ScreenHeight;
			Content = CanvasMain;

			CreateMainControls();

			StartX = Gap;
			StartY = (double)(labelTitle.Height + Gap);
			AvailableWidth = ScreenWidth - Gap * 2;
			AvailableHeight = (double)(Canvas.GetTop(labelSubtitle) - HeaderHeight - Gap * 2);

			Background = Brushes.White;
		}



		protected void HideLogo() {
			imageLogo.Visibility = Visibility.Hidden;
		}

		protected void HideButtonBack() {
			buttonBack.Visibility = Visibility.Hidden;
		}

		protected void SetLabelsContent(string title, string subtitle) {
			labelTitle.Content = ControlsFactory.CreateTextBlock(
				title, 
				FontFamilyMain, 
				FontSizeMain, 
				FontWeights.Bold);

			labelSubtitle.Content = ControlsFactory.CreateTextBlock(
				subtitle, 
				FontFamilySub, 
				FontSizeMain, 
				FontWeights.Normal);
		}

		protected void SetLabelSubtitleText(string text) {
			labelSubtitle.Content = ControlsFactory.CreateTextBlock(
				text, 
				FontFamilySub, 
				FontSizeMain, 
				FontWeights.Normal);
		}

		protected void SetElementsWidthAndHeight(double width, double height) {
			ElementWidth = width;
			ElementHeight = height;
		}



		private void CreateMainControls() {
			double logoWidth = Properties.Resources.ButterflyClear.Width;
			double logoHeight = Properties.Resources.ButterflyClear.Height;
			double logoScale = logoWidth / logoHeight;
			logoHeight = ScreenHeight * 0.15f;
			logoWidth = logoHeight * logoScale;

			double colorLineWidth = Properties.Resources.BottomLineContinuesClear.Width;
			double colorLineHeight = Properties.Resources.BottomLineContinuesClear.Height;
			double colorLineScale = colorLineWidth / colorLineHeight;
			colorLineHeight = logoHeight * 0.058f;
			colorLineWidth = colorLineHeight * colorLineScale;

			imageBottomLineColors = ControlsFactory.CreateImage(
				Properties.Resources.BottomLineContinuesClear,
				colorLineWidth,
				colorLineHeight,
				ScreenWidth - colorLineWidth,
				ScreenHeight - colorLineHeight,
				CanvasMain,
				false);

			imageBottomLineSolid = ControlsFactory.CreateImage(
				Properties.Resources.BottomLineTemplate,
				ScreenWidth - colorLineWidth,
				colorLineHeight,
				0,
				ScreenHeight - colorLineHeight,
				CanvasMain,
				false);
			imageBottomLineSolid.Stretch = Stretch.Fill;
			imageBottomLineSolid.HorizontalAlignment = HorizontalAlignment.Left;

			imageLogo = ControlsFactory.CreateImage(
				Properties.Resources.ButterflyClear,
				logoWidth,
				logoHeight,
				ScreenWidth - Gap - logoWidth,
				ScreenHeight - Gap - colorLineHeight - logoHeight,
				CanvasMain,
				false);
			Canvas.SetZIndex(imageLogo, 99);
			
			labelTitle = ControlsFactory.CreateLabel(
				"Title" + Environment.NewLine + "Title",
				Properties.Settings.Default.ColorHeaderBackground,
				Properties.Settings.Default.ColorHeaderForeground,
				FontFamilyMain,
				FontSizeMain,
				FontWeights.DemiBold,
				ScreenWidth,
				HeaderHeight,
				0,
				0,
				CanvasMain);

			labelSubtitle = ControlsFactory.CreateLabel(
				"Subtitle",
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				ScreenWidth,
				DefaultButtonHeight,
				0,
				ScreenHeight - DefaultButtonHeight - colorLineHeight - Gap,
				CanvasMain);

			buttonBack = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.BackButton2,
				DefaultButtonWidth,
				DefaultButtonHeight,
				Gap,
				Canvas.GetTop(labelSubtitle),
				CanvasMain);
			buttonBack.Click += ButtonBack_Click;
		}

		protected void CreateRootPanel(double elementsInLine, double elementsLineCount, double totalElements, 
			Orientation orientation = Orientation.Vertical, double width = 0, double height = 0, double left = 0, double top = 0) {
			//Console.WriteLine("CreateRootPanel");
			this.elementsInLine = elementsInLine;
			this.elementsLineCount = elementsLineCount;

			ElementWidth = (AvailableWidth - Gap * (elementsInLine - 1)) / elementsInLine;
			ElementHeight = (AvailableHeight - DefaultButtonHeight - Gap - Gap * (elementsLineCount - 1)) / elementsLineCount;

			VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;

			if (totalElements > elementsInLine * elementsLineCount)
				CreateUpDownButtons();
			else {
				verticalAlignment = VerticalAlignment.Center;
				horizontalAlignment = HorizontalAlignment.Center;
			}

			if (width == 0 & height == 0) {
				width = AvailableWidth + leftCornerShadow + rightCornerShadow;
				height = AvailableHeight + leftCornerShadow + rightCornerShadow - DefaultButtonHeight - Gap;
				left = StartX - leftCornerShadow;
				top = StartY - leftCornerShadow + DefaultButtonWidth + Gap;
			}

			if (totalElements > elementsInLine && totalElements < (elementsInLine * elementsLineCount) - 1) {
				double half = Math.Ceiling(totalElements / 2);
				width = ElementWidth * half + Gap * (half - 1) + leftCornerShadow + rightCornerShadow;
				left = StartX + AvailableWidth / 2 - width / 2 - leftCornerShadow;
				this.elementsInLine = half;
			}

			ScrollViewer = new ScrollViewer();
			ScrollViewer.Width = width;
			ScrollViewer.Height = height;
			CanvasMain.Children.Add(ScrollViewer);
			Canvas.SetLeft(ScrollViewer, left);
			Canvas.SetTop(ScrollViewer, top);

			ScrollViewer.PanningMode = PanningMode.Both;
			ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

			CanvasForElements = new WrapPanel();
			CanvasForElements.VerticalAlignment = verticalAlignment;
			CanvasForElements.HorizontalAlignment = horizontalAlignment;
			ScrollViewer.Content = CanvasForElements;

			if (IsDebug)
				ScrollViewer.Background = new SolidColorBrush(Colors.Yellow);

			if (orientation == Orientation.Horizontal) {
				CreateLeftRightButtons();
				CanvasForElements.Orientation = Orientation.Horizontal;
				CanvasForElements.VerticalAlignment = VerticalAlignment.Top;
				ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
			} else {
				ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			}
		}

		private void CreateLeftRightButtons() {
			buttonScrollLeft = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonLeft,
				DefaultButtonHeight,
				DefaultButtonHeight,
				StartX,
				Canvas.GetTop(ScrollViewer) + ScrollViewer.Height + Gap,
				CanvasMain);
			buttonScrollLeft.Click += ButtonScrollLeft_Click;
			buttonScrollLeft.Visibility = Visibility.Hidden;
			buttonScrollLeft.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);

			buttonScrollRight = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.ButtonRight,
				DefaultButtonHeight,
				DefaultButtonHeight,
				StartX + AvailableWidth - DefaultButtonHeight,
				Canvas.GetTop(buttonScrollLeft),
				CanvasMain);
			buttonScrollRight.Click += ButtonScrollRight_Click;
			buttonScrollRight.Visibility = Visibility.Hidden;
			buttonScrollRight.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);
		}

		private void CreateUpDownButtons() {
			buttonScrollUp = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.UpButton,
				DefaultButtonHeight,
				DefaultButtonHeight,
				StartX + AvailableWidth - DefaultButtonHeight,
				StartY,
				CanvasMain);
			buttonScrollUp.Click += ButtonScrollUp_Click;
			buttonScrollUp.Visibility = Visibility.Hidden;
			buttonScrollUp.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);

			buttonScrollDown = ControlsFactory.CreateButtonWithImageOnly(
				Properties.Resources.DownButton,
				DefaultButtonHeight,
				DefaultButtonHeight,
				StartX + AvailableWidth - DefaultButtonHeight,
				Canvas.GetTop(buttonBack),
				CanvasMain);
			buttonScrollDown.Click += ButtonScrollDown_Click;
			buttonScrollDown.Background = new SolidColorBrush(Properties.Settings.Default.ColorScrollButton);
		}

		protected void CreateQuestionControlsAnd(string question, System.Drawing.Bitmap image, 
			RoutedEventHandler buttonNoHandler, RoutedEventHandler buttonYesHandler) {
			double buttonWidth = DefaultButtonWidth * 3;

			buttonNoQuestion = ControlsFactory.CreateButtonWithImageAndText(
				"Нет",
				buttonWidth,
				DefaultButtonHeight,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonClose,
				StartX + AvailableWidth / 2 - Gap * 2 - buttonWidth,
				StartY + AvailableHeight - DefaultButtonHeight,
				CanvasMain);

			buttonYesQuestion = ControlsFactory.CreateButtonWithImageAndText(
				"Да",
				buttonNoQuestion.Width,
				buttonNoQuestion.Height,
				ControlsFactory.ElementType.Custom,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				Properties.Resources.ButtonOk,
				Canvas.GetLeft(buttonNoQuestion) + buttonNoQuestion.Width + Gap * 4,
				Canvas.GetTop(buttonNoQuestion),
				CanvasMain);
			buttonYesQuestion.Background = new SolidColorBrush(Properties.Settings.Default.ColorHeaderBackground);
			buttonYesQuestion.Foreground = new SolidColorBrush(Properties.Settings.Default.ColorHeaderForeground);

			buttonNoQuestion.Click += buttonNoHandler;
			buttonYesQuestion.Click += buttonYesHandler;
			buttonYesQuestion.Click += ButtonYesQuestion_Click;

			labelQuestion = ControlsFactory.CreateLabel(
				question,
				Colors.Transparent,
				Properties.Settings.Default.ColorLabelForeground,
				FontFamilySub,
				FontSizeMain,
				FontWeights.Normal,
				AvailableWidth,
				DefaultButtonHeight,
				StartX,
				Canvas.GetTop(buttonNoQuestion) - Gap - DefaultButtonHeight,
				CanvasMain);

			imageQuestion = ControlsFactory.CreateImage(
				image,
				AvailableWidth,
				Canvas.GetTop(labelQuestion) - StartY - Gap,
				StartX,
				StartY,
				CanvasMain,
				false);
		}

		private void ButtonYesQuestion_Click(object sender, RoutedEventArgs e) {
			HideLogo();
			
			List<Control> controlsToRemove = new List<Control>() {
				labelQuestion,
				buttonNoQuestion,
				buttonYesQuestion,
			};

			CanvasMain.Children.Remove(imageQuestion);

			foreach (Control control in controlsToRemove)
				CanvasMain.Children.Remove(control);
		}

		private void ButtonScrollRight_Click(object sender, RoutedEventArgs e) {
			double newOffset = ScrollViewer.HorizontalOffset + ScrollViewer.Width + Gap - leftCornerShadow - rightCornerShadow;
			ScrollHorizontalWithAnimation(newOffset);
			buttonScrollLeft.Visibility = Visibility.Visible;
		}

		private void ButtonScrollLeft_Click(object sender, RoutedEventArgs e) {
			double newOffset = ScrollViewer.HorizontalOffset - ScrollViewer.Width - Gap + leftCornerShadow + rightCornerShadow;
			ScrollHorizontalWithAnimation(newOffset);
			buttonScrollRight.Visibility = Visibility.Visible;
		}

		private void ScrollHorizontalWithAnimation(double to) {
			DoubleAnimation horizontalAnimation = new DoubleAnimation();
			horizontalAnimation.From = ScrollViewer.HorizontalOffset;
			horizontalAnimation.To = to;
			horizontalAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));

			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(horizontalAnimation);
			Storyboard.SetTarget(horizontalAnimation, ScrollViewer);
			Storyboard.SetTargetProperty(horizontalAnimation, new PropertyPath(ScrollAnimationBehavior.HorizontalOffsetProperty));
			storyboard.Begin();
		}



		private void ButtonScrollDown_Click(object sender, RoutedEventArgs e) {
			double newOffset = ScrollViewer.VerticalOffset + ScrollViewer.Height + Gap - leftCornerShadow - rightCornerShadow;
			ScrollVerticalWithAnimation(newOffset);
			buttonScrollUp.Visibility = Visibility.Visible;
		}

		private void ButtonScrollUp_Click(object sender, RoutedEventArgs e) {
			double newOffset = ScrollViewer.VerticalOffset - ScrollViewer.Height - Gap + leftCornerShadow + rightCornerShadow;
			ScrollVerticalWithAnimation(newOffset);
			buttonScrollDown.Visibility = Visibility.Visible;
		}

		private void ScrollVerticalWithAnimation(double to) {
			DoubleAnimation verticalAnimation = new DoubleAnimation();
			verticalAnimation.From = ScrollViewer.VerticalOffset;
			verticalAnimation.To = to;
			verticalAnimation.Duration = new Duration(new TimeSpan(0, 0, 1));

			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(verticalAnimation);
			Storyboard.SetTarget(verticalAnimation, ScrollViewer);
			Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty));
			storyboard.Begin();
		}




		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			if (buttonScrollDown != null || buttonScrollUp != null) {
				Console.WriteLine("e.VerticalOffset: " + e.VerticalOffset + " ScrollViewer.ScrollableHeight: " + ScrollViewer.ScrollableHeight);
				if (ScrollViewer.ScrollableHeight == 0) {
					buttonScrollUp.Visibility = Visibility.Hidden;
					buttonScrollDown.Visibility = Visibility.Hidden;
					return;
				}

				if (e.VerticalOffset >= ScrollViewer.ScrollableHeight) {
					buttonScrollDown.Visibility = Visibility.Hidden;
					buttonScrollUp.Visibility = Visibility.Visible;
				} else if (e.VerticalOffset <= 0) {
					buttonScrollUp.Visibility = Visibility.Hidden;
					buttonScrollDown.Visibility = Visibility.Visible;
				}
			}

			if (buttonScrollLeft != null || buttonScrollRight != null) {
				Console.WriteLine("e.HorizontalOffset: " + e.HorizontalOffset + " ScrollViewer.ScrollableWidth: " + ScrollViewer.ScrollableWidth);
				if ((int)ScrollViewer.ScrollableWidth == 0) {
					buttonScrollLeft.Visibility = Visibility.Hidden;
					buttonScrollRight.Visibility = Visibility.Hidden;
					return;
				}

				if (e.HorizontalOffset >= ScrollViewer.ScrollableWidth) {
					buttonScrollRight.Visibility = Visibility.Hidden;
					buttonScrollLeft.Visibility = Visibility.Visible;
				} else if (e.HorizontalOffset <= 0) {
					buttonScrollLeft.Visibility = Visibility.Hidden;
					buttonScrollRight.Visibility = Visibility.Visible;
				}
			}
		}



		private void ButtonBack_Click(object sender, RoutedEventArgs e) {
			NavigationService.GoBack();
		}



		protected void FillPanelWithElements(List<string> elements, ControlsFactory.ElementType type, RoutedEventHandler eventHandler) {
			double elementsCreated = 0;
			double totalElementsCreated = 0;
			double linesCreated = 0;
			double totalLines = Math.Ceiling(elements.Count / elementsInLine);

			double currentX = leftCornerShadow;
			double currentY = leftCornerShadow;

			bool isLastLineCentered = false;
			elements.Sort();
			foreach (string element in elements) {
				if (string.IsNullOrEmpty(element))
					continue;

				Button innerButton = ControlsFactory.CreateButtonWithImageAndText(
					element, 
					ElementWidth, 
					ElementHeight,
					type,
					FontFamilySub,
					FontSizeMain,
					FontWeights.Normal);

				double bottomMargin = Gap;
				double rightMargin = Gap;
				double leftMargin = 0;

				if (elementsCreated == elementsInLine - 1)
					rightMargin = 0;

				if (linesCreated == totalLines - 1) {
					bottomMargin = rightCornerShadow;

					if (totalLines > 1 && !isLastLineCentered) {
						double lastElements = elements.Count - totalElementsCreated;
						double lastElementsWidth = lastElements * ElementWidth + Gap * (lastElements - 1);
						leftMargin = (ScrollViewer.Width - lastElementsWidth ) / 2;

						isLastLineCentered = true;
					}
				}


				innerButton.Margin = new Thickness(leftMargin, 0, rightMargin, bottomMargin);

				CanvasForElements.Children.Add(innerButton);
				innerButton.Click += eventHandler;
				elementsCreated++;
				totalElementsCreated++;

				if (elementsCreated >= elementsInLine) {
					elementsCreated = 0;
					linesCreated++;
					currentX = leftCornerShadow;
					currentY += ElementHeight + Gap;
				}
			}
		}

		protected void CloseAllFormsExceptMain() {
			Console.WriteLine("CloseAllFormsExceptMain");

			while (NavigationService.CanGoBack)
				NavigationService.GoBack();

			//NavigationService.GoBack();
		}
	}
}
