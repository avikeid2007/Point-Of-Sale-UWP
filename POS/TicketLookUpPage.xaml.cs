using POS.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Popups;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketLookUpPage : Page
    {
        public ObservableCollection<Ticket> TicketResult;
        public ObservableCollection<Ticket> TicketResultPrint;
        public ObservableCollection<Customer> FilteredCustomer;
        public ObservableCollection<Notes> QuickTicketNotes;
        public ObservableCollection<Item> QuickViewTicket;
        public ObservableCollection<Payments> QuickPayments;
        public ObservableCollection<Tax> TaxList;
        public Customer rightClickCust;
        public string selectedCustID = null;
        public bool loading = false;
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        public PrintTaskOptions printingOptions;
        public PrintPageDescription printPageDescription;
        public int listNumber = 0;
        public int selectedPrintType = 0;
        public Ticket printTicket;


        public TicketLookUpPage()
        {
            FilteredCustomer = customerManager.GetCustomer();
            TicketResult = ticketManager.GetTicket();
            TicketResultPrint = ticketManager.GetTicket();
            QuickViewTicket = itemManager.GetItem();
            QuickTicketNotes = noteManager.GetNote();
            QuickPayments = payManager.GetPay();
            TaxList = taxManager.GetTax();

            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush2 = new SolidColorBrush(Colors.White);
                brush2.Opacity = 0.8;
                titleBar.Background = brush2;

                Brush brush = new SolidColorBrush(Colors.LightGray);
                brush.Opacity = 0.6;
                quickViewColor.Background = brush;
                searchPopColor.Background = brush;
                resultBackColor.Background = brush;

            }

            searchCriteriaPopUp.IsOpen = true;
            Tax.refreshingTaxRates(TaxList);
        }


        //Select Print Radio Button
        private void selectCustCopy_Checked(object sender, RoutedEventArgs e)
        {
            selectedPrintType = 1;
        }

        private void selectStoreCopy_Checked(object sender, RoutedEventArgs e)
        {
            selectedPrintType = 2;
        }

        private void selectInvoice_Checked(object sender, RoutedEventArgs e)
        {
            selectedPrintType = 3;
        }


        //Ticket
        public Task<List<Ticket>> refreshingTickets(string custID, string startDate, string endDate, string ticketID, int index)
        {
            TaskCompletionSource<List<Ticket>> tcs = new TaskCompletionSource<List<Ticket>>();
            Task.Run(async () =>
             {
                 List<Ticket> ticketList = new List<Ticket>();
                //List<string> custList = new List<string>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
               {
                     TicketResult.Clear();
                 });
                 SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
                 string sSQL = null;

                 if (index == 0)//TicketSearch
                {
                     sSQL = @"SELECT * FROM Tickets WHERE stringTicketID ='" + ticketID + "' ORDER BY ticketID DESC LIMIT -1";
                 }
                 else if (index == 1 || index == 2)//Name Search
                {
                     sSQL = @"SELECT * FROM Tickets WHERE inputDate >='" + startDate + "' AND inputDate <='" + endDate + "' AND customerID ='" + custID + "' ORDER BY ticketID DESC LIMIT -1";
                 }
                 else
                 {
                     sSQL = @"SELECT * FROM Tickets  WHERE inputDate >='" + startDate + "' AND inputDate <='" + endDate + "' ORDER BY ticketID DESC LIMIT -1";
                 }


                 ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

                 while (dbState.Step() == SQLiteResult.ROW)
                 {
                     string sID = dbState["stringTicketID"] as string;
                     string sCustID = dbState["customerID"] as string;
                     string sTotal = dbState["total"] as string;
                     if (sTotal.Substring(0, 1) == ".")
                     {
                         sTotal = "0" + sTotal;
                     }
                     string sItems = dbState["items"] as string;
                     string sPrices = dbState["prices"] as string;
                     string sInTime = dbState["inputTime"] as string;
                     string sTaxID = dbState["taxID"] as string;
                     string sQuan = dbState["quantities"] as string;
                     string sReadyDate = dbState["readyDate"] as string;
                     string sDiscount = dbState["discounts"] as string;
                     string sNotes = dbState["notes"] as string;
                     string sMods = dbState["modifiers"] as string;
                     string sInDate = dbState["inputDate"] as string;

                     string sReadyTime = "";
                     if (sReadyDate.Length != 0)
                     {
                         if (sReadyDate.Length == 4)// if just time
                        {
                             if (Convert.ToInt32(sReadyDate.Substring(0, 2)) > 12)
                             {
                                 sReadyTime = Convert.ToInt32(sReadyDate.Substring(0, 2)) - 12 + ":" + sReadyDate.Substring(2, 2) + " PM";
                             }
                             else
                             {
                                 sReadyTime = sReadyDate.Substring(0, 2) + ":" + sReadyDate.Substring(2, 2) + " AM";
                             }
                             sReadyDate = "";
                         }
                         else if (sReadyDate.Length == 8)// if just date
                        {
                             sReadyDate = sReadyDate.Substring(4, 2) + "/" + sReadyDate.Substring(6, 2) + "/" + sReadyDate.Substring(0, 4);
                         }
                         else // if date and time
                        {
                             if (Convert.ToInt32(sReadyDate.Substring(8, 2)) > 12)
                             {
                                 sReadyTime = Convert.ToInt32(sReadyDate.Substring(8, 2)) - 12 + ":" + sReadyDate.Substring(10, 2) + " PM";
                             }
                             else
                             {
                                 sReadyTime = sReadyDate.Substring(8, 2) + ":" + sReadyDate.Substring(10, 2) + " AM";
                             }
                             sReadyDate = sReadyDate.Substring(4, 2) + "/" + sReadyDate.Substring(6, 2) + "/" + sReadyDate.Substring(0, 4);

                         }
                     }

                     if (sInDate.Length != 0)
                     {
                         sInDate = sInDate.Substring(4, 2) + "/" + sInDate.Substring(6, 2) + "/" + sInDate.Substring(0, 4);
                     }


                    //custList.Add(sCustID);
                    SQLiteConnection dbConnection2 = new SQLiteConnection("Customers.db");
                     sSQL = @"SELECT * FROM Customers WHERE custID ='" + sCustID + "'";
                     ISQLiteStatement dbState2 = dbConnection2.Prepare(sSQL);
                     string sHome = null;
                     string sFirst = null;
                     string sLast = null;
                     string custFull = null;

                     while (dbState2.Step() == SQLiteResult.ROW)
                     {
                         sHome = dbState2["home"] as string;
                         sFirst = dbState2["first"] as string;
                         sLast = dbState2["last"] as string;
                     }
                     try
                     {
                         sHome = sHome.Substring(0, 3) + "-" + sHome.Substring(3, 3) + "-" + sHome.Substring(6, 4);
                     }
                     catch { }
                     if (sFirst != null)
                     {
                         custFull = sFirst + " " + sLast;
                     }

                     dbState2.Dispose();
                     dbConnection2.Dispose();




                    //Load into observable collection
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                         TicketResult.Add(new Ticket { ticketID = sID, customerID = sCustID, total = sTotal, notes = sNotes, inputTime = sInTime, readyDate = sReadyDate, readyTime = sReadyTime, items = sItems, taxID = sTaxID, quantities = sQuan, prices = sPrices, inputDate = sInDate, custName = custFull, discounts = sDiscount, modifiers = sMods, custNumber = sHome });
                     });
                 }
                 dbState.Dispose();
                 dbConnection.Dispose();

                 tcs.SetResult(ticketList);
             });


            return tcs.Task;

        }

        private void editSearchButton_Tapped(object sender, TappedRoutedEventArgs e)//open popup
        {
            searchCriteriaPopUp.IsOpen = true;
        }

        private async void search_Tapped(object sender, TappedRoutedEventArgs e)//seach inside popup
        {
            searchCriteriaPopUp.IsOpen = false;

            string startDate = startDatePicker.Date.ToString("yyyyMMdd");
            string endDate = endDatePicker.Date.ToString("yyyyMMdd");
            //endDate = Convert.ToString(Convert.ToDouble(endDate) + 1);
            int index = searchType.SelectedIndex;
            await refreshingTickets(selectedCustID, startDate, endDate, searchCritiria.Text, index);
        }

        private void ticketListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TicketExpandListViewDataTemplate"];
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TicketIDListViewDataTemplate"];
            }
        }

        //Customer Search
        private async void searchType_SelectionChanged(object sender, SelectionChangedEventArgs e)//change combobox
        {
            if (searchType.SelectedIndex == 1)
                await Customer.FilterCustomerAsync(FilteredCustomer, 0, searchCritiria.Text.ToUpper());
            else if (searchType.SelectedIndex == 2)
                await Customer.FilterCustomerAsync(FilteredCustomer, 1, searchCritiria.Text.ToUpper());
            else
            {
                FilteredCustomer.Clear();
            }

        }

        private void searchResults_ItemClick(object sender, ItemClickEventArgs e)
        {
            var cust = (Customer)e.ClickedItem;
            selectedCustID = cust.customerID;
        }//clicking on customer

        private async void searchCritiria_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (searchType.SelectedIndex == 1)
                await Customer.FilterCustomerAsync(FilteredCustomer, 0, searchCritiria.Text.ToUpper());
            else if (searchType.SelectedIndex == 2)
                await Customer.FilterCustomerAsync(FilteredCustomer, 1, searchCritiria.Text.ToUpper());
            else
            {
                FilteredCustomer.Clear();
            }

        }

        //Printer stuff
        /*
         * print button -> printTaskRequest -> printTaskSourceRequest -> Pagination -> get preview page
         */
        public int ticketPerPage = 43;
        async private void printButton_Click(object sender, RoutedEventArgs e)
        {


            printMe.Visibility = Visibility.Visible;

            printDoc.Paginate += null;
            printDoc.GetPreviewPage += null;
            printDoc.AddPages += null;
            printMan.PrintTaskRequested += null;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
            printMan.PrintTaskRequested += PrintTaskRequested;

            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            try
            {
                //printMan.PrintTaskRequested += PrintTaskRequested;
            }
            catch
            {


            }
            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;

        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;

        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        private async void Paginate(object sender, PaginateEventArgs e)
        {
            printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            printPageDescription = printingOptions.GetPageDescription(0);

            //get the height and width of the print option
            printMe.Width = printPageDescription.PageSize.Width;
            printMe.MinHeight = printPageDescription.PageSize.Height;
            //test1.MinHeight = printPageDescription.PageSize.Height;
            test2.Width = printPageDescription.PageSize.Width;
            test2.Height = printPageDescription.PageSize.Height;
            printMe.Height = printPageDescription.PageSize.Height;


            //get the amount of items per page due to the page size
            if (printPageDescription.PageSize.Height == 1056)
            {
                ticketPerPage = 35;
            }
            else if (printPageDescription.PageSize.Height > 1056)
            {
                ticketPerPage = 50;
            }
            if (printPageDescription.PageSize.Height < 1056)
            {
                ticketPerPage = 26;
            }
            else if (printPageDescription.PageSize.Height < 750)
            {
                ticketPerPage = 18;
            }



            await Task.Delay(50);

            //get the total page count
            listNumber = 0;
            int stampCount = 0;
            foreach (Ticket stamp in TicketResult)
            {
                stampCount += 1;
                if (stampCount == ticketPerPage)
                {
                    listNumber += 1;
                    stampCount = 0;
                }
            }
            if (stampCount < 0)
            {
                listNumber += 1;
            }
            printDoc.SetPreviewPageCount(listNumber + 1, PreviewPageCountType.Final);
        }

        private async void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {

            await Task.Delay(200);
            // Provide a UIElement as the print preview.
            TicketResultPrint.Clear();
            listNumber = 0;
            int stampCount = 0;
            //adds items to a listview and captures a hidden gird to get preview
            foreach (Ticket stamp in TicketResult)
            {
                TicketResultPrint.Add(stamp);
                stampCount += 1;
                if (stampCount == ticketPerPage)
                {
                    ListViewItem lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[ticketPerPage - 1]) as ListViewItem;

                    while (lvi2 == null)
                    {
                        try
                        {
                            lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[ticketPerPage - 1]) as ListViewItem;
                        }
                        catch { }

                        await Task.Delay(1);
                    }

                    //await Task.Delay(1000);
                    printDoc.SetPreviewPage(listNumber + 1, printMe);
                    listNumber += 1;
                    stampCount = 0;
                    TicketResultPrint.Clear();
                }
            }
            if (stampCount > 0)
            {
                ListViewItem lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[stampCount - 1]) as ListViewItem;

                while (lvi2 == null)
                {
                    lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[stampCount - 1]) as ListViewItem;
                    await Task.Delay(1);
                }
                printDoc.SetPreviewPage(listNumber + 1, printMe);
                listNumber += 1;

            }

        }

        private async void AddPages(object sender, AddPagesEventArgs e)
        {
            await Task.Delay(200);
            // Provide a UIElement as the print preview.
            TicketResultPrint.Clear();
            listNumber = 0;
            int stampCount = 0;

            foreach (Ticket stamp in TicketResult)
            {
                TicketResultPrint.Add(stamp);
                stampCount += 1;
                if (stampCount == ticketPerPage)
                {
                    ListViewItem lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[ticketPerPage - 1]) as ListViewItem;

                    while (lvi2 == null)
                    {
                        lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[ticketPerPage - 1]) as ListViewItem;
                        await Task.Delay(1);
                    }
                    printMe.MinHeight = printPageDescription.PageSize.Height;
                    printMe.Height = printPageDescription.PageSize.Height;
                    //test1.MinHeight = printPageDescription.PageSize.Height;
                    test2.Width = printPageDescription.PageSize.Width;
                    test2.Height = printPageDescription.PageSize.Height;
                    printDoc.AddPage(printMe);
                    listNumber += 1;
                    stampCount = 0;
                    TicketResultPrint.Clear();
                }
            }
            if (stampCount > 0)
            {
                ListViewItem lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[stampCount - 1]) as ListViewItem;

                while (lvi2 == null)
                {
                    lvi2 = (ticketListView2).ContainerFromItem(ticketListView2.Items[stampCount - 1]) as ListViewItem;
                    await Task.Delay(1);
                }
                printDoc.SetPreviewPage(listNumber + 1, printMe);
                printMe.MinHeight = printPageDescription.PageSize.Height;
                printMe.Height = printPageDescription.PageSize.Height;
                test2.Width = printPageDescription.PageSize.Width;
                test2.Height = printPageDescription.PageSize.Height;
                printDoc.AddPage(printMe);
                listNumber += 1;
            }
            printDoc.AddPagesComplete();
        }

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {

            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });


            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
           {
               printMe.Visibility = Visibility.Collapsed;
           });
            if (printDoc == null)
            {
                return;
            }

            printMan.PrintTaskRequested -= PrintTaskRequested;

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (printDoc == null)
            {
                return;
            }

            printMan.PrintTaskRequested -= PrintTaskRequested;
            printDoc.Paginate -= Paginate;
            printDoc.GetPreviewPage -= GetPreviewPage;
            printDoc.AddPages -= AddPages;

        }



        private void printTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            printTicket = button.DataContext as Ticket;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void confirmConfirm_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (selectedPrintType == 1 || selectedPrintType == 2)
            {
                printTicketControl.printTicket(printTicket);
            }
            else if (selectedPrintType == 3)
            {
                printInvoiceControl.printInvoice(printTicket, TaxList, progress2, progressTint2);
            }
        }


        //Quick View
        private async void quickViewOpen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            Ticket selectedTicket = button.DataContext as Ticket;
            progress2.IsActive = true;
            progress2.Visibility = Visibility.Visible;
            progressTint2.Visibility = Visibility.Visible;
            quickViewPopUp.IsOpen = true;
            await QuickView.generateQuickView(selectedTicket, this, QuickViewTicket, QuickTicketNotes, TaxList, QuickPayments, quickViewListView, loading, quickViewName, quickViewPhone, ticketViewTotal, quickViewReadyDate, quickViewTicketID, quickViewInputDate, ticketViewTax, quickReadyDateTitle, quickNoteTitle, quickTermsTitle, quickReadyChangeAmountTitle, quickViewChangeAmount);
            QuickView.updateDataTemplate(quickViewListView, this);
            progress2.IsActive = false;
            quickViewListView.UpdateLayout();
            progress2.Visibility = Visibility.Collapsed;
            progressTint2.Visibility = Visibility.Collapsed;

        }
        private void quickViewListView_LayoutUpdated(object sender, object e)
        {
            if (loading == true)
            {
                try
                {
                    QuickView.updateDataTemplate(quickViewListView, this);
                }

                catch { }
            }
        }

        private void editTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            Ticket selectedTicket = button.DataContext as Ticket;
            MainPage.MyselectionFrame.Navigate(typeof(CreateTicketPage), selectedTicket);
            Frame.Navigate(typeof(CreateTicketPage), selectedTicket);
        }


    }
}
