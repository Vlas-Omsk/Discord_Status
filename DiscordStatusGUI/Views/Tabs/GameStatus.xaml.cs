﻿using DiscordStatusGUI.Libs.DiscordApi;
using DiscordStatusGUI.ViewModels;
using DiscordStatusGUI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DiscordStatusGUI.Views.Tabs
{
    /// <summary>
    /// Логика взаимодействия для TopPanel.xaml
    /// </summary>
    public partial class GameStatus : UserControl
    {
        public GameStatus()
        {
            InitializeComponent();

            (DataContext as GameStatusViewModel).GameStatusView = this;
        }

        private bool LockedByGame = false;
        public void ProfilesComboBox_IsEnabled(bool value, bool isGame = false)
        {
            if (!LockedByGame || isGame)
            Dispatcher.Invoke(() =>
                ProfilesComboBox.IsEnabled = value);
            if (isGame)
                LockedByGame = true;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth - 510 < 370)
            {
                Grid.SetColumnSpan(Options, 2);
                Grid.SetRow(Options, 2);
                Grid.SetColumn(Preview, 0);
                Grid.SetRowSpan(Preview, 1);
            }
            else
            {
                Grid.SetColumnSpan(Options, 1);
                Grid.SetRow(Options, 1);
                Grid.SetColumn(Preview, 1);
                Grid.SetRowSpan(Preview, 3);
            }
        }

        #region HelpText
        private void HelpText_MouseEnter(object sender, MouseEventArgs e)
        {
            var help = FindName((sender as FrameworkElement).Name + "_HelpText") as FrameworkElement;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacity = new DoubleAnimation();
            opacity.Duration = TimeSpan.FromMilliseconds(120);
            opacity.To = 1;
            storyboard.Children.Add(opacity);

            DoubleAnimation scaleX = new DoubleAnimation();
            scaleX.Duration = TimeSpan.FromMilliseconds(120);
            scaleX.To = 1;
            storyboard.Children.Add(scaleX);

            DoubleAnimation scaleY = new DoubleAnimation();
            scaleY.Duration = TimeSpan.FromMilliseconds(120);
            scaleY.To = 1;
            storyboard.Children.Add(scaleY);

            Storyboard.SetTargetProperty(opacity, new PropertyPath("Opacity"));
            Storyboard.SetTarget(opacity, help);

            Storyboard.SetTargetProperty(scaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(scaleX, help);

            Storyboard.SetTargetProperty(scaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(scaleY, help);

            storyboard.Begin();
        }

        private void HelpText_MouseLeave(object sender, MouseEventArgs e)
        {
            var help = FindName((sender as FrameworkElement).Name + "_HelpText") as FrameworkElement;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation opacity = new DoubleAnimation();
            opacity.Duration = TimeSpan.FromMilliseconds(120);
            opacity.To = 0;
            storyboard.Children.Add(opacity);

            DoubleAnimation scaleX = new DoubleAnimation();
            scaleX.Duration = TimeSpan.FromMilliseconds(120);
            scaleX.To = 0.8;
            storyboard.Children.Add(scaleX);

            DoubleAnimation scaleY = new DoubleAnimation();
            scaleY.Duration = TimeSpan.FromMilliseconds(120);
            scaleY.To = 0.8;
            storyboard.Children.Add(scaleY);

            Storyboard.SetTargetProperty(opacity, new PropertyPath("Opacity"));
            Storyboard.SetTarget(opacity, help);

            Storyboard.SetTargetProperty(scaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(scaleX, help);

            Storyboard.SetTargetProperty(scaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(scaleY, help);

            storyboard.Begin();
        }
        #endregion HelpText


        public IEnumerable<FieldInfo> ActivityFields = typeof(Activity).GetRuntimeFields();
        private void Field_TextChanged(object sender, TextChangedEventArgs e)
        {
            var field = e.OriginalSource as TextBox;
            var activity_value = ActivityFields.Where(x => x.Name.Contains($"<{field.Name}>")).Single().GetValue(Static.CurrentActivity.SavedState)?.ToString();
            if ((field.Text == "" ? null : field.Text) != activity_value)
            {
                IsChanged = true;
            }
            
            if (ActivityFields.All(element => {
                    if (!(DataContext as GameStatusViewModel).Options.Contains(element.Name.Replace(">k__BackingField", "").Replace("<", "")))
                        return true;
                    var f = element.GetValue(Static.CurrentActivity.SavedState);
                    var s = element.GetValue(Static.CurrentActivity);
                    return (f?.ToString() ?? "") == (s?.ToString() ?? "");//(f?.ToString() == "" ? null : f) == (s == "" ? null : s);
                }))
            {
                IsChanged = false;
            }
        }

        public bool IsChanged
        {
            set
            {
                if (value)
                {
                    ProfilesComboBox_IsEnabled(false);
                    if (SaveChangesBox?.Visibility == Visibility.Hidden)
                        SaveChangesBox?.Show();
                }
                else
                {
                    ProfilesComboBox_IsEnabled(true);
                    if (SaveChangesBox?.Visibility == Visibility.Visible)
                        SaveChangesBox?.Hide();
                }
            }
        }
    }
}