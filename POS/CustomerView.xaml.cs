using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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

namespace POS.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerView : Page
    {
        public CustomerView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            base.OnNavigatedTo(e);

            var rightClickCust = (Customer)e.Parameter;
            first.Text = rightClickCust.first + " " + rightClickCust.last ;
            spouce.Text = rightClickCust.spouse;
            home.Text = rightClickCust.home;
            work.Text = rightClickCust.work;
            cell.Text = rightClickCust.cell;
            email.Text = rightClickCust.email;
            add1.Text = rightClickCust.address1;
            add2.Text = rightClickCust.adrress2;
            if(rightClickCust.adrress2 == "")
            {
                addressStack.Children.Remove(add2);
            }
            city.Text = rightClickCust.city;
            if(rightClickCust.state != "")
            {
                state.Text = rightClickCust.state + ", " + rightClickCust.zip;
            }
            else
            {
                state.Text = rightClickCust.zip;
            }


            
        }

        private  void Page_Loaded(object sender, RoutedEventArgs e)
        {   SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                

                mySolidColorBrush.Color = Color.FromArgb(250,255, 255, 255);
                

            }
            else
            {
                mySolidColorBrush.Color = Color.FromArgb(250, 59, 59, 59);
            }
            rectangleBackground.Fill = mySolidColorBrush;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            
        }
    }
}
