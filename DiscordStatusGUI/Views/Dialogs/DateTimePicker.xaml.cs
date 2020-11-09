﻿using DiscordStatusGUI.Extensions;
using DiscordStatusGUI.ViewModels.Dialogs;
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

namespace DiscordStatusGUI.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для TopPanel.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        public DateTimePicker()
        {
            InitializeComponent();

            Static.OnMouseButtonClick += Static_OnMouseButtonClick;
            Static.OnMouseMove += Static_OnMouseMove;
        }

        private DateTimePickerViewModel DateTimePickerViewModel => DataContext as DateTimePickerViewModel;
        private bool IsHourArrowCaptured = false, IsMinuteArrowCaptured = false, IsSecondArrowCaptured = false;

        private void Static_OnMouseButtonClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                IsMinuteArrowCaptured = false;
                IsHourArrowCaptured = false;
                IsSecondArrowCaptured = false;
            }
        }

        private double GetAngle(Point p1, Point center, Point p2)
        {
            p1.X -= center.X; p1.Y -= center.Y;
            p2.X -= center.X; p2.Y -= center.Y;
            var cos = Math.Round((p1.X * p2.X + p1.Y * p2.Y) / (Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y) * Math.Sqrt(p2.X * p2.X + p2.Y * p2.Y)), 9);

            return Math.Acos(cos) * 180 / Math.PI;
        }

        private const int HourAngle = 360 / 12;
        private const int MinuteAngle = 360 / 60;
        private const int HourAngle2 = HourAngle / 2;
        private const int MinuteAngle2 = MinuteAngle / 2;
        private void Static_OnMouseMove(int x, int y)
        {
            if (IsSecondArrowCaptured || IsMinuteArrowCaptured || IsHourArrowCaptured)
            {
                var w2 = ClockBody.ActualHeight / 2;
                var center = ClockBody.PointToScreen(new Point(w2, w2));
                var p1 = ClockBody.PointToScreen(new Point(w2, w2 - 1));
                var mul = x < center.X ? -1 : 1;
                var angle = mul * (GetAngle(p1, center, new Point(x, y)) + (x < center.X ? -360 : 0));

                if (IsMinuteArrowCaptured)
                    DateTimePickerViewModel.SelectedMinute = (int)((angle + MinuteAngle2) / MinuteAngle);
                if (IsHourArrowCaptured)
                    if (DateTimePickerViewModel.SelectedHour > 11)
                        DateTimePickerViewModel.SelectedHour = (int)((angle + HourAngle2) / HourAngle) + 12;
                    else
                        DateTimePickerViewModel.SelectedHour = (int)((angle + HourAngle2) / HourAngle);
                if (IsSecondArrowCaptured)
                    DateTimePickerViewModel.SelectedSecond = (int)((angle + MinuteAngle2) / MinuteAngle);
            }
        }

        private void MinuteArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMinuteArrowCaptured = true;
        }

        private void SecondArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsSecondArrowCaptured = true;
        }

        private void HourArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsHourArrowCaptured = true;
        }
    }
}