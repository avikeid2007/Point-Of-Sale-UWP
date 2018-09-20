using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class settingsPage : Page
    {
        public settingsPage()
        {
            this.InitializeComponent();
        }

        private void fullscreen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }

        }

        private void trans_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.MyHamMenu.Background = new SolidColorBrush(Color.FromArgb(0xFF, 96, 96, 96));
        }


        //Theme
        private void light_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Brush brush = new SolidColorBrush(Colors.White);
            localSettings.Values["theme"] = "light";
            localSettings.Values["themeSetting"] = "light";
            brush.Opacity = 0.15;
            MainPage.allBack.Background = brush;

            MainPage.MyHamMenu.RequestedTheme = ElementTheme.Light;
            if (localSettings.Values["theme"].ToString() == "dark" && App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else if (localSettings.Values["theme"].ToString() == "light" && App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else
            {
                themeMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void dark_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Brush brush = new SolidColorBrush(Colors.Black);
            localSettings.Values["theme"] = "dark";
            localSettings.Values["themeSetting"] = "dark";
            brush.Opacity = 0.15;
            MainPage.allBack.Background = brush;

            MainPage.MyHamMenu.RequestedTheme = ElementTheme.Dark;
            if (localSettings.Values["theme"].ToString() == "dark" && App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else if (localSettings.Values["theme"].ToString() == "light" && App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else
            {
                themeMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void System_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            var color = uiSettings.GetColorValue(
                                    Windows.UI.ViewManagement.UIColorType.Background
                                   );
            string color2 = color.ToString();

            if  (color2 =="#FF000000")
            {

                MainPage.MyHamMenu.RequestedTheme = ElementTheme.Light;
                Brush brush = new SolidColorBrush(Colors.Black);
                localSettings.Values["theme"] = "dark";
                brush.Opacity = 0.15;
                MainPage.allBack.Background = brush;

                MainPage.MyHamMenu.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                Brush brush = new SolidColorBrush(Colors.White);
                localSettings.Values["theme"] = "light";
                brush.Opacity = 0.15;
                MainPage.allBack.Background = brush;

                MainPage.MyHamMenu.RequestedTheme = ElementTheme.Light;

            }
            if (localSettings.Values["theme"].ToString() == "dark" && App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else if (localSettings.Values["theme"].ToString() == "light" && App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else
            {
                themeMessage.Visibility = Visibility.Collapsed;
            }

            localSettings.Values["themeSetting"] = "System";

        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["themeSetting"].ToString() == "light")
            {
                light.IsChecked = true;
            }
            else if (localSettings.Values["themeSetting"].ToString() == "dark")
            {
                dark.IsChecked = true;
            }
            else
            {
                System.IsChecked = true;
            }


            if (localSettings.Values["theme"].ToString() == "dark" && App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else if (localSettings.Values["theme"].ToString() == "light" && App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                themeMessage.Visibility = Visibility.Visible;
            }
            else
            {
                themeMessage.Visibility = Visibility.Collapsed;
            }
        }
    }
}
