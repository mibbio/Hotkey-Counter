using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System;
using System.Resources;

namespace KeyCounter
// andrewG85
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private AppSettings settings = new AppSettings();
        private static ResourceManager resManager = Properties.Resources.ResourceManager;

        public MainWindow() {
            InitializeComponent();
            InitializeCounterCollection();
            editFlyout.IsOpenChanged += counterCollection.ToggleEditMode;

            settingsUI.DataContext = settings;
            overlayWindow.DataContext = settings;
        }

        CounterCollection counterCollection;
        private void InitializeCounterCollection() {
            counterCollection = new CounterCollection();
            overlayWindow.counterItems.ItemsSource = counterCollection;
            counterListBox.ItemsSource = counterCollection;
        }

        private void BtnAddCounter_Click(object sender, RoutedEventArgs e) {
            CounterData cd = new CounterData(Guid.NewGuid(), resManager.GetString("TXT_NEW_COUNTER"), 0);
            counterCollection.Add(cd);
            editFlyout.DataContext = cd;
            editFlyout.IsOpen = true;
        }

        private void CounterListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is ListBox lb && lb.SelectedIndex >= 0) {
                editFlyout.DataContext = lb.SelectedItem;
                editFlyout.IsOpen = true;
            }
        }

        private void BtnEditDone_Click(object sender, RoutedEventArgs e) {
            editFlyout.DataContext = null;
            editFlyout.IsOpen = false;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            e.Handled = true;

            TextBox t = (TextBox)sender;
            CounterData cd = (CounterData)t.DataContext;

            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin) {
                return;
            }

            if (Keyboard.Modifiers != (ModifierKeys.None | ModifierKeys.Windows)) {
                cd.Modifiers = Keyboard.Modifiers;
            }

            cd.Key = key;
            cd.UpdateHotkey();
        }

        OverlayWindow overlayWindow = new OverlayWindow();
        private void BtnToggleOverlay_Click(object sender, RoutedEventArgs e) {
            if (overlayWindow.IsVisible) {
                overlayWindow.Hide();
                ((Button)sender).Content = resManager.GetString("TXT_BUTTON_OVERLAY_SHOW");
            }
            else {
                overlayWindow.Show();
                ((Button)sender).Content = resManager.GetString("TXT_BUTTON_OVERLAY_HIDE");
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e) {
            counterCollection.Dispose();
            App.DbConnection.Close();
        }

        private void MetroWindow_Activated(object sender, EventArgs e) {
            overlayWindow.Owner = GetWindow(this);
        }

        private void BtnDeleteCounter_Click(object sender, RoutedEventArgs e) {
            CounterData cd = (CounterData)((Button)sender).DataContext;
            counterCollection.Remove(cd);
        }

        private void BtnResetCounter_Click(object sender, RoutedEventArgs e) {
            CounterData cd = (CounterData)((Button)sender).DataContext;
            cd.Reset();
        }
    }
}
