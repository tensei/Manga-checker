﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace MangaChecker.Windows {
	/// <summary>
	///     Interaktionslogik für MangaViewer.xaml
	/// </summary>
	public partial class MangaViewer {
		private static Timer _loopTimer;

		private int _direction;

		public MangaViewer() {
			InitializeComponent();
			//loop timer
			_loopTimer = new Timer {
				Interval = 10,
				Enabled = false
			};
			// interval in milliseconds
			_loopTimer.Elapsed += loopTimerEvent;
			_loopTimer.AutoReset = true;
		}

		public string link { get; set; }

		private void loopTimerEvent(object source, ElapsedEventArgs e) {
			Application.Current.Dispatcher.BeginInvoke(new Action(() => {
				var x = scviewer.VerticalOffset;
				scviewer.ScrollToVerticalOffset(x + _direction);
				x = scviewer.VerticalOffset;
			}));
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) DragMove();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e) {
			scviewer.ScrollToTop();
		}


		private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				_loopTimer.Enabled = true;
				_direction = (int)SliderScrollSpeed.Value;
			}
			if (e.ChangedButton != MouseButton.Right) return;
			_loopTimer.Enabled = true;
			_direction = -(int)SliderScrollSpeed.Value;
		}

		private void img_MouseUp(object sender, MouseButtonEventArgs e) {
			_loopTimer.Enabled = false;
		}

		private void Canvas_MouseLeave(object sender, MouseEventArgs e) {
			_loopTimer.Enabled = false;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e) {
			try {
				images.Items.Clear();
			} catch (Exception) {
				//lul
			}
			Close();
		}
	}
}