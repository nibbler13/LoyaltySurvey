using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using LoyaltySurvey.Utilities;

namespace LoyaltySurvey.Pages.Helpers {
#pragma warning disable CA1060 // Move pinvokes to native methods class
	public static class PageControlsFactory {
#pragma warning restore CA1060 // Move pinvokes to native methods class
		public enum ElementType { Department, Doctor, Rate, Search, SurveySelect, Custom };

		public static TextBox CreateTextBox(FontFamily fontFamily, double fontSize, bool isContentCentered = true) {
			TextBox textBox = new TextBox {
				FontFamily = fontFamily,
				FontSize = fontSize,
				FontWeight = FontWeights.Normal,
				IsReadOnly = true,
				Focusable = false
			};

			if (isContentCentered) {
				textBox.VerticalContentAlignment = VerticalAlignment.Center;
				textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
			}

			return textBox;
		}
		
		public static Button CreateButton(double width, double height, double left = -1, double top = -1, Panel panel = null) {
			Button button = new Button {
				Width = width,
				Height = height,
				Background = new SolidColorBrush(Properties.Settings.Default.ColorButtonBackground),
				Foreground = new SolidColorBrush(Properties.Settings.Default.ColorLabelForeground),
				BorderThickness = new Thickness(0)
			};
			AddDropShadow(button);

			if (left != -1 && top != -1) {
				Canvas.SetLeft(button, left);
				Canvas.SetTop(button, top);
			}

			button.Style = Application.Current.MainWindow.FindResource("RoundCorner") as Style;

			if (panel != null)
				panel.Children.Add(button);

			return button;
		}

		//public static void RoundButton() {
		//	ControlTemplate circleButtonTemplate = new ControlTemplate(typeof(Button));

		//	// Create the circle
		//	FrameworkElementFactory circle = new FrameworkElementFactory(typeof(Ellipse));
		//	circle.SetValue(Ellipse.FillProperty, Brushes.LightGreen);
		//	circle.SetValue(Ellipse.StrokeProperty, Brushes.Black);
		//	circle.SetValue(Ellipse.StrokeThicknessProperty, 1.0);

		//	// Create the ContentPresenter to show the Button.Content
		//	FrameworkElementFactory presenter = new FrameworkElementFactory(typeof(ContentPresenter));
		//	presenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(Button.ContentProperty));
		//	presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
		//	presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

		//	// Create the Grid to hold both of the elements
		//	FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
		//	grid.AppendChild(circle);
		//	grid.AppendChild(presenter);

		//	// Set the Grid as the ControlTemplate.VisualTree
		//	circleButtonTemplate.VisualTree = grid;

		//	// Set the ControlTemplate as the Button.Template
		//	CircleButton.Template = circleButtonTemplate;
		//}

		public static Button CreateButtonWithImageOnly(System.Drawing.Bitmap bitmap, double width, double height,
			double left = -1, double top = -1, Panel panel = null) {
			Button button = CreateButton(width, height, left, top, panel);
			Image image = CreateImage(bitmap);
			button.Content = image;
			return button;
		}

		public static Button CreateButtonWithTextOnly(string text, double width, double height, FontFamily fontFamily,
			double fontSize, FontWeight fontWeight, double left = -1, double top = -1, Panel panel = null) {
			Button button = CreateButton(width, height, left, top, panel);
			TextBlock textBlock = CreateTextBlock(text, fontFamily, fontSize, fontWeight);
			button.Content = textBlock;
			button.Tag = text;
			return button;
		}



		public static Button CreateButtonWithImageAndText(string str, double width, double height, ElementType type,
			FontFamily fontFamily, double fontSize, FontWeight fontWeight, System.Drawing.Image imageInside = null, 
			double left = -1, double top = -1, Panel panel = null, string dcode = "") {
			if (str == null)
				throw new ArgumentNullException(nameof(str));

			string normalizedStr = str;
			Orientation orientation = Orientation.Vertical;
			double maxSizeCoefficient = 1.0;
			double fontCoefficient = 0.7;
			VerticalAlignment verticalAlignment = VerticalAlignment.Center;
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
			TextAlignment textAlignment = TextAlignment.Center;

			if (type == ElementType.Department) {
				imageInside = GetImageForDepartment(str);
				normalizedStr = str.Replace("- ", "-").Replace(" -", "-").Replace("-", " - ");
				normalizedStr = FirstCharToUpper(normalizedStr);
				maxSizeCoefficient = 0.5;
			} else if (type == ElementType.Doctor) {
				string[] splitted = str.Split('|');
				imageInside = GetImageForDoctor(splitted[0]);
				Regex regex = new Regex(Regex.Escape(" "));
				normalizedStr = regex.Replace(splitted[1], Environment.NewLine, 2);
				maxSizeCoefficient = 0.7;
			} else if (type == ElementType.Rate) {
				imageInside = GetImageForRate(str);
				normalizedStr = GetNameForRate(str);
				maxSizeCoefficient = 0.8;
			} else if (type == ElementType.Search) {
				imageInside = GetImageForDoctor(dcode);
				normalizedStr = str.Replace(" ", Environment.NewLine);
				maxSizeCoefficient = 0.65;
			} else if (type == ElementType.SurveySelect) {
				maxSizeCoefficient = 0.8;
				fontCoefficient = 1.0;
			}

			Grid grid = new Grid {
				Width = width - 5,
				Height = height - 5
			};

			Button button = CreateButton(width, height, left, top, panel);
			Image image = CreateImage((System.Drawing.Bitmap)imageInside, -1, -1, -1, -1, null, true, 10);
			TextBlock textBlock = CreateTextBlock(normalizedStr, fontFamily, fontSize * fontCoefficient, fontWeight);
			
			if (type == ElementType.Department) {
				orientation = Orientation.Horizontal;
				horizontalAlignment = HorizontalAlignment.Left;
				textAlignment = TextAlignment.Left;

				textBlock.Text = string.Empty;
				textBlock.Inlines.Add(new Bold(new Run(normalizedStr.Substring(0, 1)) { FontSize = fontSize * fontCoefficient * 1.5 }));
				textBlock.Inlines.Add(normalizedStr.Substring(1, normalizedStr.Length - 1));
			}

			if (type == ElementType.Custom)
				orientation = Orientation.Horizontal;

			if (type == ElementType.Search)
				orientation = Orientation.Horizontal;

			if (type == ElementType.Doctor)
				image.Margin = new Thickness(0);

			if (orientation == Orientation.Horizontal &&
				horizontalAlignment == HorizontalAlignment.Center) {
				image.Margin = new Thickness(10, 10, 0, 10);
				textBlock.Margin = new Thickness(0, 0, 10, 0);
			}

			if (orientation == Orientation.Horizontal) {
				ColumnDefinition col0 = new ColumnDefinition {
					Width = new GridLength(height)
				};

				ColumnDefinition col1 = new ColumnDefinition {
					Width = new GridLength(width - height)
				};

				grid.ColumnDefinitions.Add(col0);
				grid.ColumnDefinitions.Add(col1);

				Grid.SetColumn(image, 0);
				Grid.SetColumn(textBlock, 1);
			} else {
				RowDefinition row0 = new RowDefinition {
					Height = new GridLength(height * maxSizeCoefficient)
				};

				RowDefinition row1 = new RowDefinition {
					Height = new GridLength(height - row0.Height.Value)
				};

				grid.RowDefinitions.Add(row0);
				grid.RowDefinitions.Add(row1);

				Grid.SetRow(image, 0);
				Grid.SetRow(textBlock, 1);

				Border border = new Border {
					Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
					CornerRadius = new CornerRadius(0, 0, 12, 12), // 12 - same as in the style
					Margin = new Thickness(0, 0, 0, 5) // 5 - the shadow offset
				};
				textBlock.Margin = new Thickness(0, 0, 0, 5);
				Grid.SetRow(border, 1);
				grid.Children.Add(border);
			}

			textBlock.VerticalAlignment = verticalAlignment;
			textBlock.HorizontalAlignment = horizontalAlignment;
			textBlock.TextAlignment = textAlignment;

			grid.Children.Add(image);
			grid.Children.Add(textBlock);

			button.Content = grid;
			button.Tag = str;

			return button;
		}

		public static TextBlock CreateTextBlock(string text, FontFamily fontFamily, double fontSize, FontWeight fontWeight,
			Color? colorForeground = null, FontStretch? fontStretch = null) {
			TextBlock textBlock = new TextBlock {
				Text = text,
				TextAlignment = TextAlignment.Center,
				FontFamily = fontFamily,
				FontSize = fontSize,
				FontWeight = fontWeight,
				TextWrapping = TextWrapping.Wrap,
				TextTrimming = TextTrimming.CharacterEllipsis
			};

			if (colorForeground != null)
				textBlock.Foreground = new SolidColorBrush((Color)colorForeground);

			if (fontStretch != null)
				textBlock.FontStretch = (FontStretch)fontStretch;

			if (Properties.Settings.Default.IsDebug)
				textBlock.Background = new SolidColorBrush(Colors.Yellow);

			return textBlock;
		}

		public static Image CreateImage(System.Drawing.Bitmap bitmap, double width = -1, double height = -1,
			double left = -1, double top = -1, Panel panel = null, bool withMargin = true, double margin = 5.0) {
			Image image = new Image {
				Source = ImageSourceForBitmap(bitmap)
			};
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);

			if (withMargin)
				image.Margin = new Thickness(margin, margin, margin, margin);

			if (width != -1 && height != -1) {
				image.Width = width;
				image.Height = height;
			}

			if (left != -1 && top != -1) {
				Canvas.SetLeft(image, left);
				Canvas.SetTop(image, top);
			}

			if (panel != null)
				panel.Children.Add(image);

			return image;
		}

		public static Label CreateLabel(string text, Color background, Color foreground, FontFamily fontFamily, double fontSize, FontWeight fontWeight,
				double width = -1, double height = -1, double left = -1, double top = -1, Panel panel = null) {
			Label label = new Label {
				Content = CreateTextBlock(text, fontFamily, fontSize, fontWeight),
				Background = new SolidColorBrush(background),
				Foreground = new SolidColorBrush(foreground),
				HorizontalContentAlignment = HorizontalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center
			};

			if (width != -1 && height != -1) {
				label.Width = width;
				label.Height = height;
			}

			if (left != -1 && top != -1) {
				Canvas.SetLeft(label, left);
				Canvas.SetTop(label, top);
			}

			if (panel != null)
				panel.Children.Add(label);

			if (Properties.Settings.Default.IsDebug && background == Colors.Transparent)
				label.Background = new SolidColorBrush(Colors.YellowGreen);

			return label;
		}


		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteObject([In] IntPtr hObject);
		public static ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp) {
			if (bmp == null)
				throw new ArgumentNullException(nameof(bmp));

			var handle = bmp.GetHbitmap();
			try {
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			} finally {
				DeleteObject(handle);
			}
		}

		public static void AddDropShadow(UIElement element, bool heavyShadow = false) {
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			int depth = 5;
			double opacity = 0.2;
			int blurRadius = 10;

			if (heavyShadow) {
				depth = 8;
				opacity = 0.6;
				blurRadius = 8;
			}

			element.Effect = new DropShadowEffect {
				Color = Colors.Black,
				Direction = 315,
				ShadowDepth = depth,
				Opacity = opacity,
				BlurRadius = blurRadius
			};
		}



		public static System.Drawing.Image GetImageForDoctor(string dcode) {
			try {
				string folderToSearchPhotos = Directory.GetCurrentDirectory() + "\\DoctorsPhotos\\";
				string[] files = Directory.GetFiles(folderToSearchPhotos, "*.jpg");

				string wantedFile = "";
				foreach (string file in files) {
					string fileName = Path.GetFileNameWithoutExtension(file);

					if (!fileName.Contains("@"))
						continue;

					string fileDcode = fileName.Split('@')[1];

					if (fileDcode.Equals(dcode)) {
						wantedFile = file;
						break;
					}
				}

				if (string.IsNullOrEmpty(wantedFile)) {
					Logging.ToLog("Не удалось найти изображение для доктора с кодом: " + dcode);
					return Properties.Resources.DoctorWithoutAPhoto;
				}

				return System.Drawing.Image.FromFile(wantedFile);
			} catch (Exception e) {
				Logging.ToLog("Не удалось открыть файл с изображением: " + e.Message + 
					Environment.NewLine + e.StackTrace);
				return Properties.Resources.DoctorWithoutAPhoto;
			}
		}

		public static System.Drawing.Image GetImageForDepartment(string depname) {
			try {
				string mask = Directory.GetCurrentDirectory() + "\\DepartmentsPhotos\\*.png";
				string wantedFile = mask.Replace("*", depname);

				if (!File.Exists(wantedFile)) {
					Logging.ToLog("Не удалось найти изображение для подразделения: " + depname);
					return Properties.Resources.DepartmentWithoutAPhoto;
				}

				return System.Drawing.Image.FromFile(wantedFile);
			} catch (Exception e) {
				Logging.ToLog("Не удалось открыть файл с изображением: " + e.Message +
					Environment.NewLine + e.StackTrace);
				return Properties.Resources.DepartmentWithoutAPhoto;
			}
		}

		public static System.Drawing.Image GetImageForRate(string rate) {
			Dictionary<string, System.Drawing.Image> rates = new Dictionary<string, System.Drawing.Image>() {
				{ "1", Properties.Resources.DoctorRate1 },
				{ "2", Properties.Resources.DoctorRate2 },
				{ "3", Properties.Resources.DoctorRate3 },
				{ "4", Properties.Resources.DoctorRate4 },
				{ "5", Properties.Resources.DoctorRate5 }
			};

			if (!rates.ContainsKey(rate))
				return rates["3"];

			return rates[rate];
		}



		public static string GetNameForRate(string str) {
			Dictionary<string, string> rates = new Dictionary<string, string>() {
				{ "1", Properties.Resources.StringRateDoctorMark1 },
				{ "2", Properties.Resources.StringRateDoctorMark2 },
				{ "3", Properties.Resources.StringRateDoctorMark3 },
				{ "4", Properties.Resources.StringRateDoctorMark4 },
				{ "5", Properties.Resources.StringRateDoctorMark5 }
			};

			if (!rates.ContainsKey(str))
				return "unkown";

			return rates[str];
		}
		
		public static string FirstCharToUpper(string input) {
			if (String.IsNullOrEmpty(input))
				return input;

			return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
		}
	}
}
