using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class TicketPage : Page
    {
        public TicketPage()
        {
            this.InitializeComponent();
        }

        private void createTicketButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.MyselectionFrame.Navigate(typeof(CreateTicketPage));
            Frame.Navigate(typeof(CreateTicketPage));
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {

        }

        private void ticketLookUpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.MyselectionFrame.Navigate(typeof(TicketLookUpPage));
            Frame.Navigate(typeof(TicketLookUpPage));
        }
    }
}
