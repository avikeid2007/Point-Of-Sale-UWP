using POS.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

using Windows.Security.Credentials;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class homepage : Page
    {
        public ObservableCollection<Employee> Employees;
        public ObservableCollection<Ticket> OpenTickets;
        public ObservableCollection<Item> QuickViewTicket;
        public ObservableCollection<Item> ticketToSend;
        public ObservableCollection<Notes> QuickTicketNotes;
        public ObservableCollection<Payments> QuickPayments;
        public ObservableCollection<Tax> TaxList;
        bool ticketSideAdded = false;
        long lastPuchID; // used to check if punch was added
        bool loading = false;
        public string lastTicketID;//used to get the last 50 tickets 
        public int selectedPrintType =0;

        //printer

        public Ticket printTicket;
        


        DispatcherTimer Timer = new DispatcherTimer();
        public homepage()
        {
            this.InitializeComponent();
            Employees = employeeManager.GetEmployee();
            OpenTickets = ticketManager.GetTicket();
            QuickViewTicket = itemManager.GetItem();
            ticketToSend = itemManager.GetItem();
            QuickTicketNotes = noteManager.GetNote();
            QuickPayments = payManager.GetPay();
            TaxList = taxManager.GetTax();

            DataContext = this;
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)//update times
        {
            timeIn.Text = DateTime.Now.ToString("h:mm:ss tt");
            timeOut.Text = DateTime.Now.ToString("h:mm:ss tt");
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Timer.Stop();
        }
        private async void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush = new SolidColorBrush(Colors.LightGray);
                brush.Opacity = 0.6;

                punchSideBackDrop.Background = brush;
                Brush brush2 = new SolidColorBrush(Colors.White);
                brush2.Opacity = 0.7;
                ticketSide.Background = brush2;
                rightTicketColor.Background = brush2;
                Brush brush3 = new SolidColorBrush(Colors.LightGray);
                //ticketHeaderColor.Background = brush;
                brush3.Opacity = 0.6;
                quickViewColor.Background = brush3;
                punchOutColor.Background = brush3;
                punchInColor.Background = brush3;
            }

            lastTicketID = Ticket.getTicketID();
            
            Employee.refreshingEmployeeList(Employees);
            Tax.refreshingTaxRates(TaxList);
            await refreshingOpenTickets();
            lastPuchID = TimeStamp.getLastPunchID();
        }

        //Punch In button
        private void punchInButton_Click(object sender, RoutedEventArgs e)
        {
            passcodeIn.Password = "";
            punchOutStatus.Text = "";
            punchInPopup.IsOpen = true;
        }
        private async void punchInPopupButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeComboBoxIn.SelectedIndex != -1)
            {

                var selectedEmployee = (Employee)employeeComboBoxIn.SelectedItem;
                var vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> credentialsList = vault.FindAllByUserName(selectedEmployee.employeeID);
                Windows.Security.Credentials.PasswordCredential credential = credentialsList[0];
                credential.RetrievePassword();//add pass to credential
                if (passcodeIn.Password == credential.Password)
                {

                    try
                    {
                        string employee = selectedEmployee.employeeID;
                        string time1 = DateTime.Now.ToString("HH:mm:ss");//24 hour time
                        string time = time1.Substring(0, 2) + time1.Substring(3, 2) + time1.Substring(6, 2); //removing the : in the time

                        string date = DateTime.Today.ToString("d");
                        string date1 = DateTime.Today.ToString("MM/dd/yyyy"); //used for dialog box



                        date = timeFunctions.formattingDate(date);


                        long punchID = lastPuchID + 1;
                        lastPuchID = punchID;

                        SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");


                        bool test = false;
                        while (test == false)
                        {
                            string sSQL = @"INSERT INTO [Punches] 
([employeeID],[dateTime],[type]) 
VALUES 
('" + employee + "','" + date + time + "','" + "1" + "');";// instert into data base

                            dbConnection.Prepare(sSQL).Step();

                            sSQL = @"SELECT [punchID] FROM Punches ORDER BY punchID DESC LIMIT -1";
                            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);// checks the last entery
                            while (dbState.Step() == SQLiteResult.ROW)
                            {

                                var sID = (long)dbState["punchID"];
                                if (sID == punchID)
                                {
                                    test = true;
                                }
                                break;
                            }

                            dbState.Dispose();


                        }
                        punchInPopup.IsOpen = false;
                        //confirmation popup
                        ContentDialog deleteFileDialog = new ContentDialog
                        {
                            Title = "Punch In Confirmation",
                            Content = "Time stamp for " + selectedEmployee.fullname + " created at " + time1 + " " + date1,
                            //PrimaryButtonText = "Delete";
                            CloseButtonText = "Close"
                        };
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        if (localSettings.Values["theme"].ToString() == "light")
                        {
                            deleteFileDialog.RequestedTheme = ElementTheme.Light;
                        }
                        ContentDialogResult result = await deleteFileDialog.ShowAsync();
                        dbConnection.Dispose();

                    }
                    catch { }
                }//end if passcodes dont equal
                else
                {

                    punchInStatus.Text = "Passwords are not a match.";
                    if (passcodeIn.Password == "")
                    {
                        punchInStatus.Text = "Password cannot be blank.";
                    }

                }
            }//end if != -1

        }


        //Punch Out button
        private void punchOutButton_Click(object sender, RoutedEventArgs e)
        {
            passcodeOut.Password = "";
            punchOutStatus.Text = "";
            punchOutPopup.IsOpen = true;
        }
        private async void punchOutPopupButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeComboBoxOut.SelectedIndex != -1)
            {
               
                var selectedEmployee = (Employee)employeeComboBoxOut.SelectedItem;
                var vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> credentialsList = vault.FindAllByUserName(selectedEmployee.employeeID);
                Windows.Security.Credentials.PasswordCredential credential = credentialsList[0];
                credential.RetrievePassword();//add pass to credential
                if (passcodeOut.Password == credential.Password)
                {


                    try
                    {
                        string employee = selectedEmployee.employeeID;
                        string time1 = DateTime.Now.ToString("HH:mm:ss");//24 hour time
                        string time = time1.Substring(0, 2) + time1.Substring(3, 2) + time1.Substring(6, 2); //removing the : in the time

                        string date = DateTime.Today.ToString("d");
                        string date1 = DateTime.Today.ToString("MM/dd/yyyy");

                        date = timeFunctions.formattingDate(date);


                        long punchID = lastPuchID + 1;
                        lastPuchID = punchID;

                        SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
                        bool test = false;
                        while (test == false)
                        {
                            string sSQL = @"INSERT INTO [Punches] 
([employeeID],[dateTime],[type]) 
VALUES 
('" + employee + "','" + date + time + "','" + "2" + "');";// instert into data base

                            dbConnection.Prepare(sSQL).Step();
                            sSQL = @"SELECT [punchID] FROM Punches ORDER BY punchID DESC LIMIT -1";
                            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);// checks the last entery
                            while (dbState.Step() == SQLiteResult.ROW)
                            {

                                var sID = (long)dbState["punchID"];
                                if (sID == punchID)
                                {
                                    test = true;
                                }
                                break;

                            }
                            dbState.Dispose();
                        }
                        punchOutPopup.IsOpen = false;

                        //confirmation popup
                        ContentDialog deleteFileDialog = new ContentDialog
                        {
                            Title = "Punch Out Confirmation",
                            Content = "Time stamp for " + selectedEmployee.fullname + " created at " + time1 + " " + date1,
                            //PrimaryButtonText = "Delete";
                            CloseButtonText = "Close"
                        };
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        if (localSettings.Values["theme"].ToString() == "light")
                        {
                            deleteFileDialog.RequestedTheme = ElementTheme.Light;
                        }
                        ContentDialogResult result = await deleteFileDialog.ShowAsync();
                        dbConnection.Dispose();


                        // Delete the file if the user clicked the primary button.
                        /// Otherwise, do nothing.
                        if (result == ContentDialogResult.Primary)
                        {
                            // Delete the file.
                        }
                        else
                        {
                            // The user clicked the CLoseButton, pressed ESC, Gamepad B, or the system back button.
                            // Do nothing.
                        }


                    }
                    catch { }
                }//end if passcodes dont equal
                else
                {

                    punchOutStatus.Text = "Passwords are not a match.";
                    if(passcodeOut.Password == "")
                    {
                        punchOutStatus.Text = "Password cannot be blank.";
                    }

                }
            }//end if != -1
            else
            {
                punchOutStatus.Text = "Select your name.";
            }

        }


        public Task<List<Ticket>> refreshingOpenTickets()
        {
            TaskCompletionSource<List<Ticket>> tcs = new TaskCompletionSource<List<Ticket>>();
            Task.Run(async () =>
            {
                List<Ticket> ticketList = new List<Ticket>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    OpenTickets.Clear();
                });
                SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                int indexMainMenu;

                try
                {
                    indexMainMenu = Convert.ToInt16(localSettings.Values["mainMenuList"].ToString());
                }
                catch
                {
                    indexMainMenu = 50;
                }

                string today = DateTime.Today.ToString("yyyyMMdd");
                string sSQL = null;
                int last50ID;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch (indexMainMenu)
                    {


                        case 1://Ready Date Not Passed
                            sSQL = @"SELECT * FROM Tickets  WHERE readyDate >='" + today + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Upcoming Ready Date";
                            break;

                        case 2://Tickets created today
                            sSQL = @"SELECT * FROM Tickets WHERE inputDate ='" + today + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Today's Tickets";
                            break;
                        case 3: //Tickets in the Last 7 Days

                            DateTime day7 = DateTime.Today.AddDays(-7);

                            string daySeven = day7.ToString("yyyyMMdd");

                            sSQL = @"SELECT * FROM Tickets WHERE inputDate >='" + daySeven + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Tickets from Last 7 Days";
                            break;
                        case 4: //Last 50 Tickets

                            //find ticket id of 50 tickets away
                            last50ID = int.Parse(lastTicketID, System.Globalization.NumberStyles.HexNumber) - 25;

                            sSQL = @"SELECT * FROM Tickets WHERE ticketID > '" + last50ID + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Previous Tickets";

                            break;

                        case 5: // Last 100 Tickets
                                //find ticket id of 100 tickets away
                            int last100ID = int.Parse(lastTicketID, System.Globalization.NumberStyles.HexNumber) - 50;
                            sSQL = @"SELECT * FROM Tickets WHERE ticketID > '" + last100ID + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Previous Tickets";
                            break;

                        default:

                            last50ID = int.Parse(lastTicketID, System.Globalization.NumberStyles.HexNumber) - 25;

                            sSQL = @"SELECT * FROM Tickets WHERE ticketID > '" + last50ID + "' ORDER BY ticketID DESC LIMIT -1";
                            ticketListTitle.Text = "Previous Tickets";
                            break;


                    }

                });

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
                    string sTaxID = dbState["taxID"] as string;
                    string sDiscount = dbState["discounts"] as string;
                    string sMods = dbState["modifiers"] as string;
                    string sQuan = dbState["quantities"] as string;
                    string sReadyDate = dbState["readyDate"] as string;
                    string sInDate = dbState["inputDate"] as string;
                    string sInTime = dbState["inputTime"] as string;
                    string sNotes = dbState["notes"] as string;

                    double sChangeAmount = (double)dbState["change"];

                    string sReadyTime = "";
                    if (sReadyDate.Length != 0)
                    {
                        if (sReadyDate.Length == 4)// if just time
                        {
                            if (Convert.ToInt32(sReadyDate.Substring(0, 2)) > 12)
                            {
                                sReadyTime = Convert.ToInt32(sReadyDate.Substring(0, 2))-12 + ":" +  sReadyDate.Substring(2, 2) + " PM";
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

                    if(sInDate.Length != 0)
                    {
                          sInDate = sInDate.Substring(4, 2) + "/" + sInDate.Substring(6, 2) + "/" + sInDate.Substring(0, 4);   
                    }
                    
                    



                    SQLiteConnection dbConnection2 = new SQLiteConnection("Customers.db");
                    sSQL = @"SELECT [home],[cell],[work],[first],[last] FROM Customers WHERE custID ='" + sCustID + "'";
                    ISQLiteStatement dbState2 = dbConnection2.Prepare(sSQL);
                    string sHome = null;
                    string sFirst = null;
                    string sLast = null;
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
                        
                    dbConnection2.Dispose();
                    dbState2.Dispose();

                    string full = "";
                    if (sFirst != null)
                    {
                        full = sFirst + " " + sLast;
                    }
                    //Load into observable collection
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       OpenTickets.Add(new Ticket { ticketID = sID, customerID = sCustID, total = sTotal, notes = sNotes, inputTime = sInTime, readyDate = sReadyDate, readyTime = sReadyTime, items = sItems, taxID = sTaxID, quantities = sQuan, prices = sPrices, inputDate = sInDate, custName = full, discounts = sDiscount, modifiers = sMods, custNumber = sHome, changeAmount=Convert.ToString(sChangeAmount) });
                   });
                }
                dbConnection.Dispose();
                dbState.Dispose();

                tcs.SetResult(ticketList);
            });
            return tcs.Task;
        }
        private void totalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                if(ticketSideAdded == false)
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["TicketExpandListViewDataTemplate"];
                }
                else
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["TicketExpandListViewDataTemplate2"];
                }
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TicketIDListViewDataTemplate"];
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
            
            progress2.IsActive = false;
            QuickView.updateDataTemplate(quickViewListView, this);
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

        

        //Email Invoice
        private void emailInvoice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            Ticket selectedTicket = button.DataContext as Ticket;

            Invoice.createEmail(selectedTicket, QuickViewTicket, ticketToSend, frame, TaxList, progress1, progressTint1);

        }


        




        //Printing
        private void printTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            printTicket = button.DataContext as Ticket;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            
        }

        private void confirmConfirm_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //brings up print UI
            frame.Visibility = Visibility.Visible;

            //progress1.IsActive = true;
            //progress1.Visibility = Visibility.Visible;
            //progressTint1.Visibility = Visibility.Visible;
            if(selectedPrintType == 1 || selectedPrintType == 2)
            {
                printTicketControl.printTicket(printTicket);
            }
            else if (selectedPrintType == 3)
            {
                printInvoiceControl.printInvoice(printTicket, TaxList, progress1, progressTint1);
            }
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

 
        private void outsideGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(outsideGrid.ActualWidth < 925)
            {
                if (ticketSideAdded == false)
                {
                    height0.Height = new GridLength(300);
                    height1.Height = new GridLength(300);
                    outsideGrid.Children.Remove(ticketSide);
                    Grid.SetColumnSpan(punchSideScroll, 2);
                    Grid.SetColumnSpan(punchSideBackDrop, 2);
                    punchSide.Children.Add(ticketSide);
                    Grid.SetRow(ticketSide, 2);
                    Grid.SetColumnSpan(ticketSide, 10);
                    Grid.SetColumn(ticketSide, 0);
                    ticketSideWidth.MinWidth = 0;
                    mostLeft.Width = new GridLength(0);
                    mostRight.Width = new GridLength(0);
                    punchSideScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    if(ticketListView.SelectedIndex != -1)
                    {
                        ListViewItem lvi = (ticketListView).ContainerFromIndex(ticketListView.SelectedIndex) as ListViewItem;
                        lvi.ContentTemplate = (DataTemplate)this.Resources["TicketExpandListViewDataTemplate2"];
                    }
                    Grid.SetColumnSpan(punchInPopup, 2);
                    Grid.SetColumnSpan(punchOutPopup, 2);

                }
                ticketSideAdded = true;
            }
            else
            {
                if (ticketSideAdded == true)
                {
                    height0.Height = new GridLength(1, GridUnitType.Star);
                    height1.Height = new GridLength(1, GridUnitType.Star);
                    punchSide.Children.Remove(ticketSide);
                    outsideGrid.Children.Add(ticketSide);
                    Grid.SetColumnSpan(punchSideScroll, 1);
                    Grid.SetColumnSpan(punchSideBackDrop, 1);
                    ticketSideAdded = false;
                    Grid.SetColumnSpan(ticketSide, 1);
                    Grid.SetColumn(ticketSide, 2);
                    ticketSideWidth.MinWidth = 400;
                    punchSideScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    mostLeft.Width = new GridLength(1, GridUnitType.Star);
                    mostRight.Width = new GridLength(1, GridUnitType.Star);
                    if (ticketListView.SelectedIndex != -1)
                    {
                        ListViewItem lvi = (ticketListView).ContainerFromIndex(ticketListView.SelectedIndex) as ListViewItem;
                        lvi.ContentTemplate = (DataTemplate)this.Resources["TicketExpandListViewDataTemplate"];
                    }
                    Grid.SetColumnSpan(punchInPopup, 1);
                    Grid.SetColumnSpan(punchOutPopup, 1);


                }
            }
        }

        //edit ticket
        private void editTicket_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            Ticket selectedTicket = button.DataContext as Ticket;
            MainPage.MyselectionFrame.Navigate(typeof(CreateTicketPage), selectedTicket);
            Frame.Navigate(typeof(CreateTicketPage),selectedTicket);
        }

       



    }
}

