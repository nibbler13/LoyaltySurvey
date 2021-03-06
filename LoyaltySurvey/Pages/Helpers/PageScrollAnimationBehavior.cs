﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LoyaltySurvey.Pages.Helpers {
	public static class PageScrollAnimationBehavior {
		#region Private ScrollViewer for ListBox

		private static ScrollViewer _listBoxScroller = new ScrollViewer();

		#endregion

		#region VerticalOffset Property

		public static DependencyProperty VerticalOffsetProperty { get; set; } =
			DependencyProperty.RegisterAttached("VerticalOffset",
												typeof(double),
												typeof(PageScrollAnimationBehavior),
												new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

		public static void SetVerticalOffset(FrameworkElement target, double value) {
			if (target == null)
				return;

			target.SetValue(VerticalOffsetProperty, value);
		}

		public static double GetVerticalOffset(FrameworkElement target) {
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			return (double)target.GetValue(VerticalOffsetProperty);
		}

		#endregion
		
		#region HorizontalOffset Property

		public static DependencyProperty HorizontalOffsetProperty { get; set; } =
			DependencyProperty.RegisterAttached("HorizontalOffset",
												typeof(double),
												typeof(PageScrollAnimationBehavior),
												new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

		public static void SetHorizontalOffset(FrameworkElement target, double value) {
			if (target == null)
				return;

			target.SetValue(HorizontalOffsetProperty, value);
		}

#pragma warning disable IDE1006 // Naming Styles
		public static double cw(FrameworkElement target) {
#pragma warning restore IDE1006 // Naming Styles
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			return (double)target.GetValue(HorizontalOffsetProperty);
		}

		#endregion
		
		#region OnHorizontalOffset Changed

		private static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			if (target is ScrollViewer scrollViewer) 
				scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
		}

		#endregion
		
		#region TimeDuration Property

		public static DependencyProperty TimeDurationProperty { get; set; } =
			DependencyProperty.RegisterAttached("TimeDuration",
												typeof(TimeSpan),
												typeof(PageScrollAnimationBehavior),
												new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

		public static void SetTimeDuration(FrameworkElement target, TimeSpan value) {
			if (target == null)
				return;

			target.SetValue(TimeDurationProperty, value);
		}

		public static TimeSpan GetTimeDuration(FrameworkElement target) {
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			return (TimeSpan)target.GetValue(TimeDurationProperty);
		}

		#endregion

		#region PointsToScroll Property

		public static DependencyProperty PointsToScrollProperty { get; set; } =
			DependencyProperty.RegisterAttached("PointsToScroll",
												typeof(double),
												typeof(PageScrollAnimationBehavior),
												new PropertyMetadata(0.0));

		public static void SetPointsToScroll(FrameworkElement target, double value) {
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			target.SetValue(PointsToScrollProperty, value);
		}

		public static double GetPointsToScroll(FrameworkElement target) {
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			return (double)target.GetValue(PointsToScrollProperty);
		}

		#endregion

		#region OnVerticalOffset Changed

		private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			if (target is ScrollViewer scrollViewer)
				scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
		}

		#endregion

		#region IsEnabled Property

		public static DependencyProperty IsEnabledProperty { get; set; } =
												DependencyProperty.RegisterAttached("IsEnabled",
												typeof(bool),
												typeof(PageScrollAnimationBehavior),
												new UIPropertyMetadata(false, OnIsEnabledChanged));

		public static void SetIsEnabled(FrameworkElement target, bool value) {
			if (target == null)
				return;

			target.SetValue(IsEnabledProperty, value);
		}

		public static bool GetIsEnabled(FrameworkElement target) {
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			return (bool)target.GetValue(IsEnabledProperty);
		}

		#endregion

		#region OnIsEnabledChanged Changed

		private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var target = sender;

			if (target != null && target is ScrollViewer) {
				ScrollViewer scroller = target as ScrollViewer;
				scroller.Loaded += new RoutedEventHandler(ScrollerLoaded);
			}

			if (target != null && target is ListBox) {
				ListBox listbox = target as ListBox;
				listbox.Loaded += new RoutedEventHandler(ListboxLoaded);
			}
		}

		#endregion

		#region AnimateScroll Helper

		private static void AnimateScroll(ScrollViewer scrollViewer, double ToValue) {
			DoubleAnimation verticalAnimation = new DoubleAnimation {
				From = scrollViewer.VerticalOffset,
				To = ToValue,
				Duration = new Duration(GetTimeDuration(scrollViewer))
			};

			Storyboard storyboard = new Storyboard();

			storyboard.Children.Add(verticalAnimation);
			Storyboard.SetTarget(verticalAnimation, scrollViewer);
			Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(PageScrollAnimationBehavior.VerticalOffsetProperty));
			storyboard.Begin();
		}

		#endregion

		#region NormalizeScrollPos Helper

		private static double NormalizeScrollPos(ScrollViewer scroll, double scrollChange, Orientation o) {
			double returnValue = scrollChange;

			if (scrollChange < 0) {
				returnValue = 0;
			}

			if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight) {
				returnValue = scroll.ScrollableHeight;
			} else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth) {
				returnValue = scroll.ScrollableWidth;
			}

			return returnValue;
		}

		#endregion

		#region UpdateScrollPosition Helper

		private static void UpdateScrollPosition(object sender) {
			if (sender is ListBox listbox) {
				double scrollTo = 0;

				for (int i = 0; i < (listbox.SelectedIndex); i++) 
					if (listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) is ListBoxItem tempItem) 
						scrollTo += tempItem.ActualHeight;

				AnimateScroll(_listBoxScroller, scrollTo);
			}
		}

		#endregion

		#region SetEventHandlersForScrollViewer Helper

		private static void SetEventHandlersForScrollViewer(ScrollViewer scroller) {
			scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
			scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
		}

		#endregion

		#region scrollerLoaded Event Handler

		private static void ScrollerLoaded(object sender, RoutedEventArgs e) {
			ScrollViewer scroller = sender as ScrollViewer;

			SetEventHandlersForScrollViewer(scroller);
		}

		#endregion

		#region listboxLoaded Event Handler

		private static void ListboxLoaded(object sender, RoutedEventArgs e) {
			ListBox listbox = sender as ListBox;

			_listBoxScroller = FindVisualChildHelper.GetFirstChildOfType<ScrollViewer>(listbox);
			SetEventHandlersForScrollViewer(_listBoxScroller);

			SetTimeDuration(_listBoxScroller, new TimeSpan(0, 0, 0, 0, 200));
			SetPointsToScroll(_listBoxScroller, 16.0);

			listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
			listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
			listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
		}

		#endregion

		#region ScrollViewerPreviewMouseWheel Event Handler

		private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			double mouseWheelChange = (double)e.Delta;
			ScrollViewer scroller = (ScrollViewer)sender;
			double newVOffset = GetVerticalOffset(scroller) - (mouseWheelChange / 3);

			if (newVOffset < 0) {
				AnimateScroll(scroller, 0);
			} else if (newVOffset > scroller.ScrollableHeight) {
				AnimateScroll(scroller, scroller.ScrollableHeight);
			} else {
				AnimateScroll(scroller, newVOffset);
			}

			e.Handled = true;
		}

		#endregion

		#region ScrollViewerPreviewKeyDown Handler

		private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e) {
			ScrollViewer scroller = (ScrollViewer)sender;

			Key keyPressed = e.Key;
			double newVerticalPos = GetVerticalOffset(scroller);
			bool isKeyHandled = false;

			if (keyPressed == Key.Down) {
				newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
				isKeyHandled = true;
			} else if (keyPressed == Key.PageDown) {
				newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
				isKeyHandled = true;
			} else if (keyPressed == Key.Up) {
				newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
				isKeyHandled = true;
			} else if (keyPressed == Key.PageUp) {
				newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
				isKeyHandled = true;
			}

			if (newVerticalPos != GetVerticalOffset(scroller)) {
				AnimateScroll(scroller, newVerticalPos);
			}

			e.Handled = isKeyHandled;
		}

		#endregion

		#region ListBox Event Handlers

		private static void ListBoxLayoutUpdated(object sender, EventArgs e) {
			UpdateScrollPosition(sender);
		}

		private static void ListBoxLoaded(object sender, RoutedEventArgs e) {
			UpdateScrollPosition(sender);
		}

		private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdateScrollPosition(sender);
		}

		#endregion
	}

	public static class FindVisualChildHelper {
		public static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject {
			if (dependencyObject == null) {
				return null;
			}

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++) {
				var child = VisualTreeHelper.GetChild(dependencyObject, i);

				var result = (child as T) ?? GetFirstChildOfType<T>(child);

				if (result != null) {
					return result;
				}
			}

			return null;
		}
	}
}
