using POS.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class printInvoicexaml : Page
    {
        public ObservableCollection<Payments> InvoicePayments;
        public printInvoicexaml()
        {
            this.InitializeComponent();
            InvoicePayments = payManager.GetPay();
        }

        private async void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            pageCount.Text = ApplicationData.Current.LocalSettings.Values["pageCount"].ToString();
            if (localSettings.Values["ShowHeader"].ToString() == "true")
            {

                StorageFolder folder = ApplicationData.Current.LocalFolder;//used to get logo

                ticketID.Text = localSettings.Values["ticketID"].ToString();
                //Input Date
                if (localSettings.Values["showInputDate"].ToString() == "true")
                {
                        inputDate.Text = localSettings.Values["ticketInputDate"].ToString();
                        ApplicationData.Current.LocalSettings.Values["ticketInputDate"] = "";
                }
                else
                {
                    inputDate.Visibility = Visibility.Collapsed;
                }

                
                //Input Time
                if (localSettings.Values["showInputTime"].ToString() == "true")
                {
                    inputTime.Text = timeFunctions.time24to12(localSettings.Values["ticketInputTime"].ToString());
                    ApplicationData.Current.LocalSettings.Values["ticketInputTime"] = "";
                }
                else
                {
                    inputTime.Visibility = Visibility.Collapsed;
                }
                //Hide the header if both time and date are hidden
                if(inputTime.Visibility == Visibility.Collapsed && inputDate.Visibility == Visibility.Collapsed )
                {
                    inputDateHeader.Visibility = Visibility.Collapsed;
                }
                //ready Date
                if(localSettings.Values["showReadyDate"].ToString() == "true")
                {
                    if (localSettings.Values["ticketReadyDate"] as string == "" || localSettings.Values["ticketReadyDate"] as string == null)
                    {
                        readyDate.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        readyDate.Text = localSettings.Values["ticketReadyDate"].ToString();
                    }
                }
                else
                {
                    readyDate.Visibility = Visibility.Collapsed;
                }
                //ready Time
                if (localSettings.Values["showReadyTime"].ToString() == "true")
                {
                    if (localSettings.Values["ticketReadyTime"] as string == "" || localSettings.Values["ticketReadyTime"] as string == null)
                    {
                        readyTime.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        readyTime.Text = localSettings.Values["ticketReadyTime"].ToString();
                    }
                }
                else
                {
                    readyTime.Visibility = Visibility.Collapsed;
                }
                if(readyTime.Visibility == Visibility.Collapsed && readyDate.Visibility == Visibility.Collapsed)
                {
                    readyDateHeader.Visibility = Visibility.Collapsed;
                }

                //move ready date and time if  input date is hidden
                if(inputDateHeader.Visibility == Visibility.Collapsed)
                {
                    Grid.SetColumn(readyDateStackPanel, 1);
                    Grid.SetColumn(readyDateHeader, 0);
                }


                SolidColorBrush black = new SolidColorBrush(Colors.Black);

                //recreate business data
                businessName.Text = localSettings.Values["businessName"].ToString();

                busPhoneNumber.Text = localSettings.Values["busPhone"].ToString();
                if (localSettings.Values["busPhone"].ToString() == "" )
                {
                    phoneBusHeader.Visibility = Visibility.Collapsed;
                    busPhoneNumber.Visibility = Visibility.Collapsed;
                }


                busFax.Text = localSettings.Values["busFax"].ToString();
                if(localSettings.Values["busFax"].ToString() == "")
                {
                    faxBusHeader.Visibility = Visibility.Collapsed;
                    busFax.Visibility = Visibility.Collapsed;
                }

                busEmail.Text = localSettings.Values["busEmail"].ToString();
                if (localSettings.Values["busEmail"].ToString() == "")
                {
                    emailBusHeader.Visibility = Visibility.Collapsed;
                    busEmail.Visibility = Visibility.Collapsed;
                }


                busWeb.Text = localSettings.Values["busWeb"].ToString();
                if (localSettings.Values["busWeb"].ToString() == "")
                {
                    websiteBusHeader.Visibility = Visibility.Collapsed;
                    busWeb.Visibility = Visibility.Collapsed;
                }


                string bussAdd1;
                bussAdd1 = localSettings.Values["busAddress1"].ToString();
                if (bussAdd1 != "" && bussAdd1 != " ")
                {
                    TextBlock add1 = new TextBlock();
                    add1.FontSize = 30;
                    add1.Foreground = black;
                    add1.Text = bussAdd1;
                    busAddress.Children.Add(add1);
                }

                string bussAdd2;
                bussAdd2 = localSettings.Values["busAddress2"].ToString();
                if (bussAdd2 != "" && bussAdd2 != " ")
                {
                    TextBlock add2 = new TextBlock();
                    add2.FontSize = 30;
                    add2.Foreground = black;
                    add2.Text = bussAdd2;
                    busAddress.Children.Add(add2);
                }


                string addres3 = "";
                string busCity = "";
                string busState = "";
                string busZip = "";

                busCity = localSettings.Values["busCity"].ToString();
                busState = localSettings.Values["busState"].ToString();
                busZip = localSettings.Values["busZip"].ToString();

                if (busCity != "" )
                {
                    addres3 = busCity + ", " + busState + " " + busZip;
                }
                else
                {
                    addres3 = busState + " " + busZip;
                }
                if (addres3 != "" && addres3 != " ")
                {
                    TextBlock add3 = new TextBlock();
                    add3.FontSize = 30;
                    add3.Foreground = black;
                    add3.Text = addres3;
                    busAddress.Children.Add(add3);
                }
                if(busAddress.Children.Count() == 0)
                {
                    addressBusHeader.Visibility = Visibility.Collapsed;
                }


                //get logo
                try
                {
                    if (ApplicationData.Current.LocalSettings.Values["showLogo"].ToString() == "true")
                    {
                        StorageFile newFile = await folder.GetFileAsync("logo.png");
                        var bitmapImage = new BitmapImage();

                        bitmapImage.SetSource(await newFile.OpenAsync(FileAccessMode.Read));

                        ImageViewer.Source = bitmapImage;
                    }
                }
                catch{}




                //get cust email
                if (localSettings.Values["showCustEmail"].ToString() == "true")
                {
                    string custEmail1 = ApplicationData.Current.LocalSettings.Values["ticketCustomerEmail"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketCustomerEmail"] = "";

                    if (custEmail1 != "" && custEmail1 != null)
                    {
                        custEmail.Text = custEmail1;
                    }
                    else
                    {
                        custEmailHeader.Visibility = Visibility.Collapsed;
                        custEmail.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    custEmailHeader.Visibility = Visibility.Collapsed;
                    custEmail.Visibility = Visibility.Collapsed;
                }

                //get cust phone
                if (localSettings.Values["showCustPhone"].ToString() == "true")
                {
                    string custHome = ApplicationData.Current.LocalSettings.Values["ticketCustomerHome"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketCustomerHome"] = "";

                    string custCell = ApplicationData.Current.LocalSettings.Values["ticketCustomerCell"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketCustomerCell"] = "";

                    string custWork = ApplicationData.Current.LocalSettings.Values["ticketCustomerWork"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketCustomerWork"] = "";

                    if (custHome != "" && custHome != null)
                    {
                        TextBlock home = new TextBlock();
                        home.FontSize = 30;
                        home.Foreground = black;
                        home.Text = custHome;
                        phoneStack.Children.Add(home);

                    }
                    if (custCell != "" && custCell != null)
                    {
                        TextBlock cell = new TextBlock();
                        cell.Foreground = black;
                        cell.FontSize = 30;
                        cell.Text = custCell;
                        phoneStack.Children.Add(cell);

                    }
                    if (custWork != "" && custWork != null)
                    {
                        TextBlock work = new TextBlock();
                        work.Foreground = black;
                        work.FontSize = 30;
                        work.Text = custWork;
                        phoneStack.Children.Add(work);

                    }
                    if (phoneStack.Children.Count == 0)
                    {
                        phoneStack.Visibility = Visibility.Collapsed;
                        custPhoneHeader.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    custPhoneHeader.Visibility = Visibility.Collapsed;
                    phoneStack.Visibility = Visibility.Collapsed;
                }
                //get cust name
                if(localSettings.Values["showCustName"].ToString() == "true")
                {
                    string custName1 = ApplicationData.Current.LocalSettings.Values["ticketCustomerName"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketCustomerName"] = "";
                    
                    if(custName1 != "" && custName1 != null)
                    {
                        custName.Text = custName1;
                    }
                    else
                    {
                        custNameHeader.Visibility = Visibility.Collapsed;
                        custName.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    custNameHeader.Visibility = Visibility.Collapsed;
                    custName.Visibility = Visibility.Collapsed;
                }
                //get cust Company
                if(localSettings.Values["showCustCompany"].ToString() == "true")
                {
                    string custCompany1 = localSettings.Values["ticketCustomerCompany"] as string;
                    if (custCompany1 != "" && custCompany1 != null)
                    {
                        custCompany.Text = localSettings.Values["ticketCustomerCompany"].ToString();
                    }
                    else
                    {
                        custCompany.Visibility = Visibility.Collapsed;
                        custCompanyHeader.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    custCompanyHeader.Visibility = Visibility.Collapsed;
                    custCompany.Visibility = Visibility.Collapsed;
                }

                if(localSettings.Values["showBusHeader"].ToString() == "false")
                {
                    websiteBusHeader.Visibility = Visibility.Collapsed;
                    phoneBusHeader.Visibility = Visibility.Collapsed;
                    faxBusHeader.Visibility = Visibility.Collapsed;
                    addressBusHeader.Visibility = Visibility.Collapsed;
                    emailBusHeader.Visibility = Visibility.Collapsed;

                    allBusHeader.MinWidth = 0;
                    allBusHeader.Width = new GridLength(0);
                }

                if (localSettings.Values["showCustHeader"].ToString() == "false")
                {
                    custEmailHeader.Visibility = Visibility.Collapsed;
                    custNameHeader.Visibility = Visibility.Collapsed;
                    custPhoneHeader.Visibility = Visibility.Collapsed;
                    custCompanyHeader.Visibility = Visibility.Collapsed;

                    allCustHeader.MinWidth = 0;
                    allCustHeader.Width = new GridLength(0);

                }

                //mmove ready and input time
                if (custEmail.Visibility == Visibility.Collapsed && phoneStack.Visibility == Visibility.Collapsed && custName.Visibility == Visibility.Collapsed && custCompany.Visibility == Visibility.Collapsed)
                {
                    gridPage.Children.Remove(invoiceInfo);
                    customerInfo.Children.Add(invoiceInfo);

                    invoiceInfo.ColumnDefinitions.RemoveAt(2);
                    invoiceInfo.ColumnDefinitions.RemoveAt(3);


                    Grid.SetColumn(invoiceInfo, 0);
                    Grid.SetColumnSpan(invoiceInfo,4);
                    allCustHeader.Width = new GridLength(0);

                    Grid.SetRow(readyDateHeader, 2);
                    Grid.SetRow(readyDateStackPanel, 2);
                    Grid.SetColumn(readyDateHeader, 0);
                    Grid.SetColumn(readyDateStackPanel, 1);
                    readyDateHeader.HorizontalAlignment = HorizontalAlignment.Left;
                }


            }
            else //if show header
            {
                invoiceInfo.Visibility = Visibility.Collapsed;
                header.Visibility = Visibility.Collapsed;
            }


            if (localSettings.Values["ShowPaymentTotal"].ToString() == "true")
            {
                try
                {
                    ticketTax.Text = "$" + ApplicationData.Current.LocalSettings.Values["ticketTax"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketTax"] = "";
                }
                catch { ticketTax.Text = "$0.00"; }
                try
                {
                    ticketTotal.Text = "$" + ApplicationData.Current.LocalSettings.Values["ticketTotal"] as string;
                    ApplicationData.Current.LocalSettings.Values["ticketTotal"] = "";
                }
                catch { ticketTotal.Text = "$0.00"; }

                if(localSettings.Values["ShowPayments"].ToString() == "true")
                {
                      Payments.getPaymentsLow(localSettings.Values["ticketID"].ToString(), InvoicePayments);
                }
                else
                {
                    paymentTitle.Visibility = Visibility.Collapsed;
                }


                if (InvoicePayments.Count == 0)
                {
                    paymentTitle.Visibility = Visibility.Collapsed;
                }
                
            }
            else
            {
                payments.Visibility = Visibility.Collapsed;
                paymentTitle.Visibility = Visibility.Collapsed;
            }
            if (localSettings.Values["showAmountDue"].ToString() == "false")
            {
                ticketAmountDue.Visibility = Visibility.Collapsed;
                amountDueHeader.Visibility = Visibility.Collapsed;
            }
            else if (Convert.ToDouble(ApplicationData.Current.LocalSettings.Values["ticketAmountDue"] as string) <= 0)//amount due
            {
                ticketAmountDue.Visibility = Visibility.Visible;
                amountDueHeader.Visibility = Visibility.Visible;
                amountDueHeader.Text = "Amount Due:";
                ticketAmountDue.Text = "$"+ (ApplicationData.Current.LocalSettings.Values["ticketAmountDue"] as string).Replace("-","") ;
            }

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["loadingInvoive"] = "true";
            var ticketItems = e.Parameter as ObservableCollection<Item>;
            invoiceList.ItemsSource = ticketItems;

            int k = 0;

            foreach (var item in invoiceList.Items)
            {
                //var item = (Item)lvi2;

                ListViewItem lvi = invoiceList.ContainerFromItem(item) as ListViewItem;
                
                while (lvi == null)
                {
                    await Task.Delay(25);
                    lvi = invoiceList.ContainerFromItem(item) as ListViewItem;
                }
                //list

                //lvi.ContentTemplate = (DataTemplate)this.Resources["ItemSelectedDiscountDataTemplate"];
                if (ticketItems[k].minQuantity == "0" && ticketItems[k].discount != "No Discount")
                {
                    if(ticketItems[k].gray == false)
                    {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModDiscountDataTemplate"];//mod discount
                    }
                    else
                    {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModDiscountBackgroundDataTemplate"];//mod discount

                    }
                }
                else if (ticketItems.ElementAt(k).minQuantity == "0" && ticketItems.ElementAt(k).discount == "No Discount")
                {
                    if (ticketItems[k].gray == false)
                    {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModDataTemplate"];//mod
                    }
                    else
                    {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModBackgroundDataTemplate"];//mod

                    }
                }
                else if (ticketItems[k].minQuantity != "0" && ticketItems[k].discount != "No Discount")
                {
                        if (ticketItems[k].gray == false)
                        {
                            lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketDiscountDataTemplate"];//dicount
                        }
                        else
                        {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketDiscountBackgroundDataTemplate"];//dicount
                        }
                }
                else
                {
                            if (ticketItems[k].gray == false)
                            {
                                lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketDataTemplate"];//regular
                            }
                            else
                            {
                        lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketBackgroundDataTemplate"];//regular

                    }
                }

                k++;
            }

            
           
        

            ApplicationData.Current.LocalSettings.Values["loadingInvoive"] = "false";

        }

    }
    }
