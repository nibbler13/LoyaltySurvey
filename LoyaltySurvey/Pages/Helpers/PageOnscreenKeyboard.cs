using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LoyaltySurvey.Pages.Helpers {
	public class PageOnscreenKeyboard {
		/// <summary>
		/// virtual keyboard fields
		private readonly TextBox textBoxInput = null;
		private enum ShiftKeyStatus { Unpressed, Pressed, Capslock };
		private ShiftKeyStatus currentShiftKeyStatus = ShiftKeyStatus.Unpressed;
		private TimeSpan previousShiftKeyPress = new TimeSpan();
		private readonly double availableWidth;
		private readonly double availableHeight;
		private readonly double startX;
		private readonly double startY;
		private readonly double gap;
		private readonly double fontSize;
		private Button buttonShift;
		private Button buttonEnter;
		public enum KeyboardType { Full, Alphabet, Number }
		private readonly KeyboardType keyboardType;
		/// </summary>

		public PageOnscreenKeyboard(
			TextBox textBoxInput,
			double availableWidth,
			double availableHeight,
			double startX,
			double startY,
			double gap,
			double fontSize,
			KeyboardType keyboardType) {
			this.textBoxInput = textBoxInput;
			this.availableWidth = availableWidth;
			this.availableHeight = availableHeight;
			this.startX = startX;
			this.startY = startY;
			this.gap = gap;
			this.fontSize = fontSize;
			this.keyboardType = keyboardType;
		}

		public void SetEnterButtonClick(RoutedEventHandler eventHandler) {
			RemoveClickEvent(buttonEnter);
			buttonEnter.Click += eventHandler;
		}

		private void RemoveClickEvent(Button b) {
			b.Click -= ButtonKeyEnter_Click;
		}

		public Canvas CreateOnscreenKeyboard() {
			List<List<string>> keys = new List<List<string>>() {
				new List<string>() {
					"(", ")", "'", "", "й", "ц", "у", "к", "е", "н", "г", "ш", "щ", "з", "х", "", "7", "8", "9"
				},
				new List<string>() {
					 "-", "+", "=", "", "ф", "ы", "в", "а", "п", "р", "о", "л", "д", "ж", "э", "", "4", "5", "6"
				},
				new List<string>() {
					":", ";", "!", "", "shift", "я", "ч", "с", "м", "и", "т", "ь", "б", "ю", "backspace", "", "1", "2", "3"
				},
				new List<string>() {
					",", ".", "?", "", "ё", "ъ", "Пробел", "Ввод", "", "", "0", ""
				}
			};

			int optionalKeysCount = 4;
			if (keyboardType == KeyboardType.Alphabet || keyboardType == KeyboardType.Number)
				for (int i = 0; i < keys.Count; i++) {
					if (keyboardType == KeyboardType.Alphabet) {
						keys[i].RemoveRange(keys[i].Count - optionalKeysCount, optionalKeysCount);
						keys[i].RemoveRange(0, optionalKeysCount);
					} else if (keyboardType == KeyboardType.Number) {
						keys[i].RemoveRange(0, keys[i].Count - optionalKeysCount + 1);
					}
				}

			double keyboardSizeCoefficient = 0.4;

			if (keyboardType == KeyboardType.Number)
				keys[3][2] = "backspace";

			int keysInLine = keys[0].Count;
			int keysLines = keys.Count;

			double keyboardHeight = (int)(availableHeight * keyboardSizeCoefficient);

			double distanceBetween = gap / 3;

			double buttonHeight = (keyboardHeight - distanceBetween * (keysLines - 1)) / keysLines;
			double buttonWidth = buttonHeight;

			double keyboardWidth = buttonWidth * keysInLine + distanceBetween * (keysInLine - 1);

			double leftCornerShadow = 4;
			double rightCornerShadow = 8;

			double keyboardX = startX + (availableWidth - keyboardWidth) / 2;
			double keyboardY = startY + availableHeight - keyboardHeight;
			double keyCurrentX = leftCornerShadow;
			double keyCurrentY = leftCornerShadow;

			Canvas canvasKeyboard = new Canvas {
				Width = keyboardWidth + leftCornerShadow + rightCornerShadow,
				Height = keyboardHeight + leftCornerShadow + rightCornerShadow
			};

			if (Properties.Settings.Default.IsDebug)
				canvasKeyboard.Background = new SolidColorBrush(Colors.Yellow);

			currentShiftKeyStatus = ShiftKeyStatus.Unpressed;
			
			foreach (List<string> keysLine in keys) {
				foreach (string keyName in keysLine) {
					if (string.IsNullOrEmpty(keyName)) {
						keyCurrentX += buttonWidth + distanceBetween;
						continue;
					}

					double fontScale = 1.0;

					if (keyName.Equals("Пробел") || keyName.Equals("Ввод"))
						fontScale = 0.7;

					Button buttonKey = PageControlsFactory.CreateButtonWithTextOnly(
						keyName,
						buttonWidth,
						buttonHeight,
						new System.Windows.Media.FontFamily(Properties.Settings.Default.FontSub.Name),
						fontSize * fontScale,
						FontWeights.Normal,
						keyCurrentX,
						keyCurrentY,
						canvasKeyboard);

					System.Drawing.Image imageToButton = null;
					string tag = "";

					switch (keyName) {
						case "shift":
							tag = "shift";
							imageToButton = Properties.Resources.ButtonShiftUnpressed;
							buttonKey.Click += ButtonKeyShift_Click;
							buttonShift = buttonKey;
							break;
						case "backspace":
							tag = "backspace";
							imageToButton = Properties.Resources.ButtonBackspace;
							buttonKey.Click += ButtonKeyBackspace_Click;
							break;
						case "Пробел":
							tag = "space";
							buttonKey.Width = buttonWidth * 7 + distanceBetween * 6;
							keyCurrentX += buttonKey.Width - buttonWidth;
							buttonKey.Click += ButtonKeySpace_Click;
							break;
						case "Ввод":
							tag = "enter";
							buttonKey.Width = buttonWidth * 2 + distanceBetween;
							keyCurrentX += buttonKey.Width - buttonWidth;
							buttonKey.Click += ButtonKeyEnter_Click;
							buttonEnter = buttonKey;
							break;
						case "clear":
							tag = "clear";
							imageToButton = Properties.Resources.ButtonClear;
							buttonKey.Click += ButtonClear_Click;
							break;
						default:
							break;
					}

					if (string.IsNullOrEmpty(tag)) { 
						buttonKey.Tag = null;
						buttonKey.Click += ButtonKey_Click;
					} else
						buttonKey.Tag = tag;

					if (keyName.Equals("shift") ||
						keyName.Equals("backspace") ||
						keyName.Equals("Ввод") ||
						keyName.Equals("clear"))
						buttonKey.Background = new SolidColorBrush(Properties.Settings.Default.ColorDisabled);

					if (imageToButton != null) {
						Image image = PageControlsFactory.CreateImage((System.Drawing.Bitmap)imageToButton);
						buttonKey.Content = image;
					}

					keyCurrentX += buttonWidth + distanceBetween;
				}

				keyCurrentX = leftCornerShadow;
				keyCurrentY += buttonHeight + distanceBetween;
			}

			return canvasKeyboard;
		}

		private void ButtonClear_Click(object sender, EventArgs e) {
			textBoxInput.Clear();
		}

		private void ButtonKeyEnter_Click(object sender, EventArgs e) {
			SendKeyToTextBox(Environment.NewLine);
		}

		private void ButtonKeySpace_Click(object sender, EventArgs e) {
			SendKeyToTextBox(" ");
		}

		private void ButtonKeyBackspace_Click(object sender, EventArgs e) {
			string text = textBoxInput.Text;
			if (text.Length == 0)
				return;

			textBoxInput.Text = text.Substring(0, text.Length - 1);
		}

		private void ButtonKeyShift_Click(object sender, EventArgs e) {
			UpdateShiftKey();
		}

		private void ButtonKey_Click(object sender, RoutedEventArgs e) {
			string code = ((sender as Button).Content as TextBlock).Text;
			textBoxInput.AppendText(code);

			if (currentShiftKeyStatus == ShiftKeyStatus.Pressed)
				UpdateShiftKey(true);
		}



		private void UpdateShiftKey(bool ignoreDoubleClick = false) {
			Color color;
			System.Drawing.Image image;

			bool isDoubleClick = (new TimeSpan(DateTime.Now.Ticks) - previousShiftKeyPress).TotalSeconds < 0.5 ? true : false;
			if (ignoreDoubleClick)
				isDoubleClick = false;

			if (isDoubleClick) {
				currentShiftKeyStatus = ShiftKeyStatus.Capslock;
				color = Properties.Settings.Default.ColorButtonBackground;
				image = Properties.Resources.ButtonCapslock;
			} else if (currentShiftKeyStatus == ShiftKeyStatus.Unpressed) {
				currentShiftKeyStatus = ShiftKeyStatus.Pressed;
				color = Properties.Settings.Default.ColorButtonBackground;
				image = Properties.Resources.ButtonShiftPressed;
			} else {
				currentShiftKeyStatus = ShiftKeyStatus.Unpressed;
				color = Properties.Settings.Default.ColorDisabled;
				image = Properties.Resources.ButtonShiftUnpressed;
			}

			buttonShift.Background = new SolidColorBrush(color);
			buttonShift.Content = PageControlsFactory.CreateImage((System.Drawing.Bitmap)image);
			ChangeKeyboardCapitalizeStatus(buttonShift);

			previousShiftKeyPress = new TimeSpan(DateTime.Now.Ticks);
		}

		private void ChangeKeyboardCapitalizeStatus(Button buttonKey) {
			Canvas keyboardCanvas = buttonKey.Parent as Canvas;

			foreach (Control control in keyboardCanvas.Children) {
				if (control.Tag != null)
					continue;

				TextBlock textBlock = (control as Button).Content as TextBlock;
				textBlock.Text = currentShiftKeyStatus == ShiftKeyStatus.Unpressed ?
					textBlock.Text.ToLower() : textBlock.Text.ToUpper();
			}
		}

		private void SendKeyToTextBox(string code) {
			if (textBoxInput == null)
				return;

			textBoxInput.AppendText(code);
		}
	}
}
