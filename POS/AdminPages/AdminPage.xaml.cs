using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using SQLitePCL;
using Windows.UI;
using Windows.Security.Credentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
   

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {

        


        public AdminPage()
        {
            this.InitializeComponent();
            

        }

        private  void Page_Loading(FrameworkElement sender, object args)//Page loading
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush2 = new SolidColorBrush(Colors.White);

                brush2.Opacity = 0.8;
                headerColor.Background = brush2;
            }
            


        }

        private void financial_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(FinancialPage));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

        }

        private void ticket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(AdminTicketSettings));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

        }

        private void employee_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(EmployeePage));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        private void items_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(ItemsAddEdit));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        private void mainMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(AdminMainMenu));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        private void Data_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(DataAdminSettings));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        private void Invoice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectionFrame.Navigate(typeof(AdminInvoicePage));
            financial.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            employee.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            items.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            mainMenu.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Data.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            ticket.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            Invoice.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
        }

        
        
    }
}
