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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminTicketSettings : Page
    {
        public AdminTicketSettings()
        {
            this.InitializeComponent();
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //recreate business data
            try
            {
                businessName.Text = localSettings.Values["businessName"].ToString();
            }
            catch {  }
            try
            {
                busPhoneNumber.Text = localSettings.Values["busPhone"].ToString();
            }
            catch { }
            try
            {
                busFaxNumber.Text = localSettings.Values["busFax"].ToString();
            }
            catch { }
            try
            {
                busWeb.Text = localSettings.Values["busWeb"].ToString();
            }
            catch{}
            try
            {
                busEmail.Text = localSettings.Values["busEmail"].ToString();
            }
            catch { }
            try
            {
                busAddress1.Text = localSettings.Values["busAddress1"].ToString();
            }
            catch { }        
            try { 
                busAddress2.Text = localSettings.Values["busAddress2"].ToString();
            }
            catch { }
            try
            {
                busCity.Text = localSettings.Values["busCity"].ToString();
            }
            catch { }
            try
            {
                busState.Text = localSettings.Values["busState"].ToString();
            }
            catch { }
            try
            {
                busZip.Text = localSettings.Values["busZip"].ToString();
            }
            catch { }
            //recreate list order 
            try
            {

                int businessCust = Convert.ToInt32(localSettings.Values["businessCustTicket"].ToString());
                int barcodeCust = Convert.ToInt32(localSettings.Values["barcodeCustTicket"].ToString());
                int readyDateCust = Convert.ToInt32(localSettings.Values["readyDateCustTicket"].ToString());
                int tipCust = Convert.ToInt32(localSettings.Values["tipCustTicket"].ToString());
                int employeeCust = Convert.ToInt32(localSettings.Values["employeeCustTicket"].ToString());
                int dateCreadtedCust = Convert.ToInt32(localSettings.Values["dateCreatedCustTicket"].ToString());
                int ticketNumCust = Convert.ToInt32(localSettings.Values["ticketNumCustTicket"].ToString());
                int centerCust = Convert.ToInt32(localSettings.Values["centerCustTicket"].ToString());
                int custNameCust = Convert.ToInt32(localSettings.Values["custNameCustTicket"].ToString());
                int tableCust = Convert.ToInt32(localSettings.Values["tableCustTicket"].ToString());
                int logoCust = Convert.ToInt32(localSettings.Values["logoTicket"].ToString());
                int noteCust = Convert.ToInt32(localSettings.Values["notesCustTicket"].ToString());


                string[] list = new string[12];
                list[businessCust] = "businessList";
                list[readyDateCust] = "readyDateList";
                list[tipCust] = "estimatedTipList";
                list[employeeCust] = "employeeList";
                list[dateCreadtedCust] = "dateCreatedList";
                list[ticketNumCust] = "ticketNumList";
                list[centerCust] = "centerTicket";
                list[barcodeCust] = "barcodeList";
                list[custNameCust] = "custNameCust";
                list[tableCust] = "tableList";
                list[logoCust] = "logoCust";
                list[noteCust] = "noteCust";

                for (int i = 0; i < 13; i++)
                {
                    int currentIndex = 1;
                    switch (list[i])
                    {
                        case ("businessList"):
                            currentIndex = custListView.Items.IndexOf(businessList);
                            break;
                        case ("readyDateList"):
                            currentIndex = custListView.Items.IndexOf(readyDateList);
                            break;
                        case ("estimatedTipList"):
                            currentIndex = custListView.Items.IndexOf(estimatedTipList);
                            break;
                        case ("employeeList"):
                            currentIndex = custListView.Items.IndexOf(employNameList);
                            break;
                        case ("dateCreatedList"):
                            currentIndex = custListView.Items.IndexOf(dateCreatedList);
                            break;
                        case ("ticketNumList"):
                            currentIndex = custListView.Items.IndexOf(ticketNumList);
                            break;
                        case ("centerTicket"):
                            currentIndex = custListView.Items.IndexOf(centerTicket);
                            break;
                        case ("barcodeList"):
                            currentIndex = custListView.Items.IndexOf(barcodeList);
                            break;
                        case ("custNameCust"):
                            currentIndex = custListView.Items.IndexOf(custNameList);
                            break;
                        case ("tableList"):
                            currentIndex = custListView.Items.IndexOf(tableList);
                            break;
                        case ("logoCust"):
                            currentIndex = custListView.Items.IndexOf(logoList);
                            break;
                        case("noteCust"):
                            currentIndex = custListView.Items.IndexOf(notesList);
                            break;
                        default:
                            break;
                    }





                    var listItem = (ListViewItem)custListView.Items[currentIndex];

                    custListView.Items.RemoveAt(currentIndex);
                    custListView.Items.Insert(i, listItem);

                }
            }
            catch { };
            //recreate list toggles
            try
            {

                string businessCust = localSettings.Values["businessCustToggled"].ToString();
                string barcodeCust = localSettings.Values["barcodeCustToggled"].ToString();
                string readyDateCust = localSettings.Values["readyCustToggled"].ToString();
                string tipCust = localSettings.Values["tipCustToggled"].ToString();
                string employeeCust = localSettings.Values["employCustToggled"].ToString();
                string dateCreadtedCust = localSettings.Values["dateCreatedCustToggled"].ToString();
                string ticketNumCust = localSettings.Values["tickCustToggled"].ToString();
                string custName = localSettings.Values["custCustToggled"].ToString();
                string tableCust = localSettings.Values["tableCustToggled"].ToString();
                string logoCust = localSettings.Values["logoCustToggled"].ToString();


                if (businessCust == "False")
                {
                    businessCustToggle.IsOn = false;
                }
                 if (barcodeCust == "False")
                {
                    barcodeCustToggle.IsOn = false;
                }
                if (employeeCust == "False")
                {
                    employCustToggle.IsOn = false;
                }
                 if (readyDateCust == "False")
                {
                    readyCustToggle.IsOn = false;
                }
                 if (dateCreadtedCust == "False")
                {
                    dateCreatedCustToggle.IsOn = false;
                }
                if (ticketNumCust == "False")
                {
                    tickCustToggle.IsOn = false;
                }
                if (custName == "False")
                {
                    custCustToggle.IsOn = false;
                }
                if (tipCust == "False")
                {
                    tipCustToggle.IsOn = false;
                }
                if (tableCust == "False")
                {
                    tableCustToggle.IsOn = false;
                }
                if (logoCust == "False")
                {
                    logoCustToggle.IsOn = false;
                }


            }

            catch { }
        }


        private void saveBusiness_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


            localSettings.Values["businessName"] = businessName.Text;
            localSettings.Values["busPhone"] = busPhoneNumber.Text;
            localSettings.Values["busFax"] = busFaxNumber.Text;
            localSettings.Values["busWeb"] = busWeb.Text;
            localSettings.Values["busEmail"] = busEmail.Text;
            localSettings.Values["busAddress1"] = busAddress1.Text;
            localSettings.Values["busAddress2"] = busAddress2.Text;
            localSettings.Values["busCity"] = busCity.Text;
            localSettings.Values["busState"] = busState.Text;
            localSettings.Values["busZip"] = busZip.Text;
        }

        private void saveCustTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["businessCustTicket"] = custListView.Items.IndexOf(businessList).ToString(); 
            localSettings.Values["barcodeCustTicket"] = custListView.Items.IndexOf(barcodeList).ToString();
            localSettings.Values["readyDateCustTicket"] = custListView.Items.IndexOf(readyDateList).ToString();
            localSettings.Values["tipCustTicket"] = custListView.Items.IndexOf(estimatedTipList).ToString();
            localSettings.Values["employeeCustTicket"] = custListView.Items.IndexOf(employNameList).ToString();
            localSettings.Values["dateCreatedCustTicket"] = custListView.Items.IndexOf(dateCreatedList).ToString();
            localSettings.Values["ticketNumCustTicket"] = custListView.Items.IndexOf(ticketNumList).ToString();
            localSettings.Values["centerCustTicket"] = custListView.Items.IndexOf(centerTicket).ToString();
            localSettings.Values["custNameCustTicket"] = custListView.Items.IndexOf(custNameList).ToString();
            localSettings.Values["tableCustTicket"] = custListView.Items.IndexOf(tableList).ToString();
            localSettings.Values["logoTicket"] = custListView.Items.IndexOf(logoList).ToString();
            localSettings.Values["notesCustTicket"] = custListView.Items.IndexOf(notesList).ToString();

            localSettings.Values["barcodeCustToggled"] = barcodeCustToggle.IsOn.ToString();
            localSettings.Values["businessCustToggled"] = businessCustToggle.IsOn.ToString();
            localSettings.Values["tipCustToggled"] = tipCustToggle.IsOn.ToString();
            localSettings.Values["tickCustToggled"] = tickCustToggle.IsOn.ToString();
            localSettings.Values["dateCreatedCustToggled"] = dateCreatedCustToggle.IsOn.ToString();
            localSettings.Values["readyCustToggled"] = readyCustToggle.IsOn.ToString();
            localSettings.Values["employCustToggled"] = employCustToggle.IsOn.ToString();
            localSettings.Values["custCustToggled"] = custCustToggle.IsOn.ToString();
            localSettings.Values["tableCustToggled"] = tableCustToggle.IsOn.ToString();
            localSettings.Values["logoCustToggled"] = logoCustToggle.IsOn.ToString();
            localSettings.Values["notesCustToggle"] = notesCustToggle.IsOn.ToString();

            


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values["useQuickAdd"].ToString() == "false")
                {
                    quickAddToggle.IsOn = false;
                }
                else
                {
                    quickAddToggle.IsOn = true;
                }
            }
            catch { quickAddToggle.IsOn = false; }

        }

        private void ticketTemplateOpen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (var listItem in custListView.Items)
            {
                var ticketComponent = (ListViewItem)listItem;
                string name = ticketComponent.Name;

                if (name == "businessList")
                {
                    ticketContentStackPanel.Children.Remove(businessInfo);
                    ticketContentStackPanel.Children.Add(businessInfo);
                    if (businessCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(businessInfo);
                    }
                }
                else if (name == "barcodeList")
                {
                    ticketContentStackPanel.Children.Remove(Barcode);
                    ticketContentStackPanel.Children.Add(Barcode);
                    if (barcodeCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(Barcode);
                    }
                }

                else if (name == "readyDateList")
                {
                    ticketContentStackPanel.Children.Remove(readyDateContent);
                    ticketContentStackPanel.Children.Add(readyDateContent);
                    if (readyCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(readyDateContent);
                    }
                }

                else if (name == "estimatedTipList")
                {
                    ticketContentStackPanel.Children.Remove(tipContent);
                    ticketContentStackPanel.Children.Add(tipContent);
                    if (tipCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(tipContent);
                    }
                }
                else if (name == "employNameList")
                {
                    ticketContentStackPanel.Children.Remove(employeeContent);
                    ticketContentStackPanel.Children.Add(employeeContent);
                    if (employCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(employeeContent);
                    }
                }
                else if (name == "dateCreatedList")
                {
                    ticketContentStackPanel.Children.Remove(inputDateContent);
                    ticketContentStackPanel.Children.Add(inputDateContent);
                    if (dateCreatedCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(inputDateContent);
                    }
                }
                else if (name == "ticketNumList")
                {
                    ticketContentStackPanel.Children.Remove(ticketNumContent);
                    ticketContentStackPanel.Children.Add(ticketNumContent);
                    if (tickCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(ticketNumContent);
                    }
                }
                else if (name == "custNameList")
                {
                    ticketContentStackPanel.Children.Remove(custInfo);
                    ticketContentStackPanel.Children.Add(custInfo);
                    if (custCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(custInfo);
                    }
                }
                else if (name == "centerTicket")
                {
                    ticketContentStackPanel.Children.Remove(saleItems);
                    ticketContentStackPanel.Children.Add(saleItems);

                }
                else if (name == "tableList")
                {
                    ticketContentStackPanel.Children.Remove(tableNum);
                    ticketContentStackPanel.Children.Add(tableNum);
                    if (tableCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(tableNum);
                    }
                }
                else if (name == "logoList")
                {
                    ticketContentStackPanel.Children.Remove(logo);
                    ticketContentStackPanel.Children.Add(logo);
                    if (logoCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(logo);
                    }
                }
                else if (name == "notesList")
                {
                    ticketContentStackPanel.Children.Remove(notesContent);
                    ticketContentStackPanel.Children.Add(notesContent);
                    if (notesCustToggle.IsOn == false)
                    {
                        ticketContentStackPanel.Children.Remove(notesContent);
                    }
                }



            }


            ticketPreview.IsOpen = true;
        }

        private void quickAddToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (quickAddToggle.IsOn == false)
            {
                localSettings.Values["useQuickAdd"] = "false";
            }
            else
            {
                localSettings.Values["useQuickAdd"] = "true";
            }
        }

        private void insight_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
