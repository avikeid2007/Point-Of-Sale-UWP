using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            this.InitializeComponent();
        }

        private void reportsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                selectionFrame.Navigate(typeof(ReportsDashboard));
        }

  
        private void Page_Loading(FrameworkElement sender, object args)
        {
            selectionFrame.Navigate(typeof(ReportsDashboard));
            dash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            time.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush = new SolidColorBrush(Colors.White);
                brush.Opacity = 0.8;
                titleBar.Background = brush;
            }
        }

        private void dash_Click(object sender, RoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(ReportsDashboard));
            dash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            time.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

        }

        private void time_Click(object sender, RoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(TimeStampPage));
            time.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            dash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);


        }
    }
}
