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
    public sealed partial class AdminInvoicePage : Page
    {
        public AdminInvoicePage()
        {
            this.InitializeComponent();
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            try
            {
                subjectBox.Text = localSettings.Values["messageSub"].ToString();
                messageText.Text = localSettings.Values["messageText"].ToString();
                senderName.Text = localSettings.Values["senderName"].ToString();
                receiverCombo.SelectedIndex = Convert.ToInt32(localSettings.Values["greetingIndex"].ToString());
                senderClosingCombo.SelectedIndex = Convert.ToInt32(localSettings.Values["closingIndex"].ToString());
                string closingToggle1 = localSettings.Values["closingEnable"].ToString();
                if (closingToggle1 == "1")
                {
                    closingToggle.IsOn = true;
                }
                else
                {
                    closingToggle.IsOn = false;
                }
            }
            catch { }


            if(localSettings.Values["showLogo"].ToString() == "false")
            {
                showLogo.IsOn = false;
                logo.Visibility = Visibility.Collapsed;
            }
            else
            {
                showLogo.IsOn = true;
            }


            if (localSettings.Values["shadeItems"].ToString() == "false")
            {
                shadeItems.IsOn = false;
                shadeStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                shadeItems.IsOn = true;
            }


            if (localSettings.Values["showInputTime"].ToString() == "false")
            {
                showInputTime.IsOn = false;
                inTime.Visibility = Visibility.Collapsed;
                inDateTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                showInputTime.IsOn = true;
            }


            if (localSettings.Values["showInputDate"].ToString() == "false")
            {
                showInputData.IsOn = false;
                inDate.Visibility = Visibility.Collapsed;
                inDateTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                showInputData.IsOn = true;
            }

            if (localSettings.Values["showReadyDate"].ToString() == "false")
            {
                showReadyData.IsOn = false;
                readyDateTitle.Visibility = Visibility.Collapsed;
                readyDate.Visibility = Visibility.Collapsed;
            }
            else
            {
                showReadyData.IsOn = true;
            }

            try
            {
                if (localSettings.Values["showReadyTime"].ToString() == "false")
                {
                    showReadyTime.IsOn = false;
                    readyTime.Visibility = Visibility.Collapsed;
                    readyDateTitle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    showReadyTime.IsOn = true;
                }
            }
            catch
            {
                showReadyTime.IsOn = true;
            }

            if (localSettings.Values["showCustName"].ToString() == "false")
            {
                showCustomerName.IsOn = false;
                custNameHeader.Visibility = Visibility.Collapsed;
                custName.Visibility = Visibility.Collapsed;
            }
            else
            {
                showCustomerName.IsOn = true;
            }


            if (localSettings.Values["showCustCompany"].ToString() == "false")
            {
                showCustCompany.IsOn = false;
                custCompanyHeader.Visibility = Visibility.Collapsed;
                custCompany.Visibility = Visibility.Collapsed;
            }
            else
            {
                showCustCompany.IsOn = true;
            }


            if (localSettings.Values["showCustPhone"].ToString() == "false")
            {
                showCustPhone.IsOn = false;
                custPhoneHeader1.Visibility = Visibility.Collapsed;
                custPhoneHeader2.Visibility = Visibility.Collapsed;
                custPhoneHeader3.Visibility = Visibility.Collapsed;

                custPhone1.Visibility = Visibility.Collapsed;
                custPhone2.Visibility = Visibility.Collapsed;
                custPhone3.Visibility = Visibility.Collapsed;
            }
            else
            {
                showCustPhone.IsOn = true;
            }
            

            if (localSettings.Values["showCustEmail"].ToString() == "false")
            {
                showCustEmail.IsOn = false;
                custEmailHeader.Visibility = Visibility.Collapsed;
                custEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                showCustEmail.IsOn = true;
            }

            if (localSettings.Values["showBusHeader"].ToString() == "false")
            {
                showBusHeader.IsOn = false;
                businessHeader.Visibility = Visibility.Collapsed;
            }
            else
            {
                showBusHeader.IsOn = true;
            }

            if (localSettings.Values["showCustHeader"].ToString() == "false")
            {
                showCustHeader.IsOn = false;
                customerHeader.Visibility = Visibility.Collapsed;
            }
            else
            {
                showCustHeader.IsOn = true;
            }

            if (localSettings.Values["showPayments"].ToString() == "false")
            {
                showPayments.IsOn = false;
                paymentTitle.Visibility = Visibility.Collapsed;
                paymentList.Visibility = Visibility.Collapsed;
            }
            else
            {
                showPayments.IsOn = true;
            }

            if (localSettings.Values["showAmountDue"].ToString() == "false")
            {
                showAmountDue.IsOn = false;
                amountDueTitle.Visibility = Visibility.Collapsed;
                ticketAmountDue.Visibility = Visibility.Collapsed;
            }
            else
            {
                showAmountDue.IsOn = true;
            }
            //move input ready time and date 
            if (showCustomerName.IsOn == false && showCustCompany.IsOn == false && showCustPhone.IsOn == false && showCustEmail.IsOn == false)
            {
                try
                {
                    innerInvoiceGrid.Children.Remove(ticketTimeStack);
                    custStack.Children.Add(ticketTimeStack);
                    if (showLogo.IsOn == false)
                    {
                        inDateTitle.Margin = new Thickness(25, 0, 0, 0);

                    }
                    else
                    {
                        inDateTitle.Margin = new Thickness(5, 0, 0, 0);
                        readyDateTitle.Margin = new Thickness(7, 0, 0, 0);
                    }
                    ticketTimeStack.Orientation = Orientation.Vertical;
                }
                catch { }
            }
        }

        private void insightEmail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void saveEmail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["messageSub"] = subjectBox.Text;
            localSettings.Values["messageText"] = messageText.Text;
            localSettings.Values["senderName"] = senderName.Text;
            localSettings.Values["greetingIndex"] = Convert.ToString(receiverCombo.SelectedIndex);
            localSettings.Values["closingIndex"] = Convert.ToString(senderClosingCombo.SelectedIndex);
            if (closingToggle.IsOn)
            {
                localSettings.Values["closingEnable"] = "1";
            }
            else
            {
                localSettings.Values["closingEnable"] = "0";
            }

        }

        //Toggle switches
        private void showLogo_Toggled(object sender, RoutedEventArgs e)
        {
            if((sender as ToggleSwitch).IsOn == true)
            {
                logo.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showLogo"] = "true";
            }
            else
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showLogo"] = "false";
                logo.Visibility = Visibility.Collapsed;

            }
        }

        private void shadeItems_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["shadeItems"] = "true";
                shadeStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                shadeStackPanel.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["shadeItems"] = "false";

            }
        }

        private void showBusHeader_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                businessHeader.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showBusHeader"] = "true";
            }
            else
            {
                businessHeader.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showBusHeader"] = "false";
            }
        }


        private void showInputTime_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                inTime.Visibility = Visibility.Visible;
                inDateTitle.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showInputTime"] = "true";

            }
            else
            {
                inTime.Visibility = Visibility.Collapsed;
                if (showInputData.IsOn == false)
                {
                    inDateTitle.Visibility = Visibility.Collapsed;
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["showInputTime"] = "false";
                }

            }
        }

        private void showInputData_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                inDate.Visibility = Visibility.Visible;
                inDateTitle.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showInputDate"] = "true";

            }
            else
            {
                inDate.Visibility = Visibility.Collapsed;
                if(showInputTime.IsOn == false)
                {
                    inDateTitle.Visibility = Visibility.Collapsed;
                }
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showInputDate"] = "false";
            }
        }

        private void showReadyData_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                readyDate.Visibility = Visibility.Visible;
                readyDateTitle.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showReadyDate"] = "true";

            }
            else
            {
                readyDate.Visibility = Visibility.Collapsed;
                if (showReadyTime.IsOn == false)
                {
                    readyDateTitle.Visibility = Visibility.Collapsed;
                    
                }
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showReadyDate"] = "false";

            }
        }

        private void showReadyTime_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                readyTime.Visibility = Visibility.Visible;
                readyDateTitle.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showReadyTime"] = "true";

            }
            else
            {
                readyTime.Visibility = Visibility.Collapsed;
                if (showReadyData.IsOn == false)
                {
                    readyDateTitle.Visibility = Visibility.Collapsed;
                }
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showReadyTime"] = "false";

            }
        }


        //customer toggles
        private void showCustomerName_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                custNameHeader.Visibility = Visibility.Visible;
                custName.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustName"] = "true";
                try
                { 
                    custStack.Children.Remove(ticketTimeStack);
                    innerInvoiceGrid.Children.Add(ticketTimeStack);
                    inDateTitle.Margin = new Thickness(0, 0, 0, 0);
                    readyDateTitle.Margin = new Thickness(17, 0, 0, 0);
                    ticketTimeStack.Orientation = Orientation.Horizontal;
                }
                catch { }
            }
            else
            {
                custNameHeader.Visibility = Visibility.Collapsed;
                custName.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustName"] = "false";
                if(showCustomerName.IsOn ==false && showCustCompany.IsOn == false && showCustPhone.IsOn == false && showCustEmail.IsOn == false)
                {
                    try
                    {
                        innerInvoiceGrid.Children.Remove(ticketTimeStack);
                        custStack.Children.Add(ticketTimeStack);
                        if (showLogo.IsOn == false)
                        {
                            inDateTitle.Margin = new Thickness(25, 0, 0, 0);

                        }
                        else
                        {
                            inDateTitle.Margin = new Thickness(5, 0, 0, 0);
                            readyDateTitle.Margin = new Thickness(7, 0, 0, 0);
                        }
                        ticketTimeStack.Orientation = Orientation.Vertical;
                    }
                    catch { }
                }


            }


        }
        private void showCustCompany_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                custCompanyHeader.Visibility = Visibility.Visible;
                custCompany.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustCompany"] = "true";

                try
                {
                    custStack.Children.Remove(ticketTimeStack);
                    innerInvoiceGrid.Children.Add(ticketTimeStack);
                    inDateTitle.Margin = new Thickness(0, 0, 0, 0);
                    readyDateTitle.Margin = new Thickness(17, 0, 0, 0);
                    ticketTimeStack.Orientation = Orientation.Horizontal;
                }
                catch { }
            }
            else
            {
                custCompanyHeader.Visibility = Visibility.Collapsed;
                custCompany.Visibility = Visibility.Collapsed;

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustCompany"] = "false";

                if (showCustomerName.IsOn == false && showCustCompany.IsOn == false && showCustPhone.IsOn == false && showCustEmail.IsOn == false)
                {
                    try
                    {
                        innerInvoiceGrid.Children.Remove(ticketTimeStack);
                        custStack.Children.Add(ticketTimeStack);
                        if (showLogo.IsOn == false)
                        {
                            inDateTitle.Margin = new Thickness(25, 0, 0, 0);

                        }
                        else
                        {
                            inDateTitle.Margin = new Thickness(5, 0, 0, 0);
                            readyDateTitle.Margin = new Thickness(7, 0, 0, 0);
                        }
                        ticketTimeStack.Orientation = Orientation.Vertical;
                    }
                    catch { }
                }
            }
        }

        private void showCustPhone_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                custPhoneHeader1.Visibility = Visibility.Visible;
                custPhoneHeader2.Visibility = Visibility.Visible;
                custPhoneHeader3.Visibility = Visibility.Visible;

                custPhone1.Visibility = Visibility.Visible;
                custPhone2.Visibility = Visibility.Visible;
                custPhone3.Visibility = Visibility.Visible;

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustPhone"] = "true";

                try
                {
                    custStack.Children.Remove(ticketTimeStack);
                    innerInvoiceGrid.Children.Add(ticketTimeStack);
                    inDateTitle.Margin = new Thickness(0, 0, 0, 0);
                    readyDateTitle.Margin = new Thickness(17, 0, 0, 0);
                    ticketTimeStack.Orientation = Orientation.Horizontal;


                }
                catch { }
            }
            else
            {
                custPhoneHeader1.Visibility = Visibility.Collapsed;
                custPhoneHeader2.Visibility = Visibility.Collapsed;
                custPhoneHeader3.Visibility = Visibility.Collapsed;

                custPhone1.Visibility = Visibility.Collapsed;
                custPhone2.Visibility = Visibility.Collapsed;
                custPhone3.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustPhone"] = "false";

                if (showCustomerName.IsOn == false && showCustCompany.IsOn == false && showCustPhone.IsOn == false && showCustEmail.IsOn == false)
                {
                    try
                    {
                        innerInvoiceGrid.Children.Remove(ticketTimeStack);
                        custStack.Children.Add(ticketTimeStack);
                        if (showLogo.IsOn == false)
                        {
                            inDateTitle.Margin = new Thickness(25, 0, 0, 0);

                        }
                        else
                        {
                            inDateTitle.Margin = new Thickness(5, 0, 0, 0);
                            readyDateTitle.Margin = new Thickness(7, 0, 0, 0);
                        }
                        ticketTimeStack.Orientation = Orientation.Vertical;
                    }
                    catch { }
                }
            }
        }

        private void showCustEmail_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                custEmailHeader.Visibility = Visibility.Visible;
                custEmail.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustEmail"] = "true";

                try
                {
                    custStack.Children.Remove(ticketTimeStack);
                    innerInvoiceGrid.Children.Add(ticketTimeStack);
                    inDateTitle.Margin = new Thickness(0, 0, 0, 0);
                    readyDateTitle.Margin = new Thickness(17, 0, 0, 0);
                    ticketTimeStack.Orientation = Orientation.Horizontal;
                }
                catch { }

            }
            else
            {
                custEmailHeader.Visibility = Visibility.Collapsed;
                custEmail.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustEmail"] = "false";


                if (showCustomerName.IsOn == false && showCustCompany.IsOn == false && showCustPhone.IsOn == false && showCustEmail.IsOn == false)
                {
                    try
                    {
                        innerInvoiceGrid.Children.Remove(ticketTimeStack);
                        custStack.Children.Add(ticketTimeStack);
                        if (showLogo.IsOn == false)
                        {
                            inDateTitle.Margin = new Thickness(25, 0, 0, 0);

                        }
                        else
                        {
                            inDateTitle.Margin = new Thickness(5, 0, 0, 0);
                            readyDateTitle.Margin = new Thickness(7, 0, 0, 0);
                        }
                        ticketTimeStack.Orientation = Orientation.Vertical;
                    }
                    catch { }
                }
            }
        }


        private void showCustHeader_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                customerHeader.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustHeader"] = "true";
            }
            else
            {
                customerHeader.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showCustHeader"] = "false";
            }
        }


        private void showPayments_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                paymentTitle.Visibility = Visibility.Visible;
                paymentList.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showPayments"] = "true";

            }
            else
            {
                paymentTitle.Visibility = Visibility.Collapsed;
                paymentList.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showPayments"] = "false";
            }
        }

        private void showAmountDue_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn == true)
            {
                amountDueTitle.Visibility = Visibility.Visible;
                ticketAmountDue.Visibility = Visibility.Visible;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showAmountDue"] = "true";
            }
            else
            {
                amountDueTitle.Visibility = Visibility.Collapsed;
                ticketAmountDue.Visibility = Visibility.Collapsed;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["showAmountDue"] = "false";

            }
        }
    }
}
