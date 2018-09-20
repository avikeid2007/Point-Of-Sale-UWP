using POS.Models;
using SQLitePCL;
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
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeStampPage : Page
    {
        public ObservableCollection<Employee> EmployeeList;
        public ObservableCollection<TimeStamp> TimeStamps;
        ObservableCollection<TimeStamp> TimeStampPrintList;
        public Employee selectedEmployee;
        public TimeStamp selectedTimestamp;
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        public PrintTaskOptions printingOptions;
        public PrintPageDescription printPageDescription;
        public int listNumber = 0;


        //private PrintHelper printHelper;


        public TimeStampPage()
        {
            EmployeeList = employeeManager.GetEmployee();
            TimeStamps = timeManager.GetTimeStamp();
            TimeStampPrintList = timeManager.GetTimeStamp();
            this.InitializeComponent();
        }

        private async void Page_Loading(FrameworkElement sender, object args)
        {            
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush = new SolidColorBrush(Colors.White);
                brush.Opacity = 0.6;
                headerColor.Background = brush;
                employeeColor.Background = brush;
                Brush brush3 = new SolidColorBrush(Colors.White);
                brush3.Opacity = 0.35;
                grid1.Background = brush3;
                grid2.Background = brush3;
                editPunchColor.Background = brush3;
                addPunchColor.Background = brush3;
                Brush brush2 = new SolidColorBrush(Colors.LightGray);
                brush2.Opacity = 0.2;
                grid3.Background = brush2;
                grid4.Background = brush2;

            }

            startDatePicker.Date = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7);

            await Employee.refreshingEmployeeListAsync(EmployeeList);
        }


        
        public Task<List<Employee>> refreshingTimeStamps(string employeeID, string startDate, string endDate)
        {
            TaskCompletionSource<List<Employee>> tcs = new TaskCompletionSource<List<Employee>>();
            Task.Run(async () =>
            {
                List<Employee> employeeListAsync = new List<Employee>();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
                {
                    TimeStamps.Clear();
                });
                string selectedEmployee = employeeID;
                SQLiteConnection dbConnection2 = new SQLiteConnection("Employees.db");
                string sSQL = @"SELECT * FROM Punches WHERE employeeID =" + selectedEmployee + " AND dateTime <='" + endDate +"999999" + "' AND dateTime >='" + startDate + "000000" + "' ORDER BY dateTime ";
                ISQLiteStatement dbState2 = dbConnection2.Prepare(sSQL);

                for (int i = 0; dbState2.Step() == SQLiteResult.ROW; i++)
                {
                    var sPunchID = (long)dbState2["punchID"];
                    string shiftTime = null;
                    string sType = dbState2["type"] as string;
                    string sDateTime = dbState2["dateTime"] as string;
                    string sTime = sDateTime.Substring(8, 6);
                    string sDate = sDateTime.Substring(0, 8);
                    if (sType == "1")
                    {
                        sType = "In";
                    }
                    else
                    {
                        sType = "Out";
                                //not finished
                        if (i != 0)
                        {
                            int days;
                            //int time;//HHmmss //no :
                            if (TimeStamps[i - 1].type == "In")
                            {
                                //do subtraction and calc time of shift                            

                                days = Convert.ToInt32(sDate) - Convert.ToInt32(timeFunctions.formattingDate(TimeStamps[i - 1].date));

                                shiftTime = timeFunctions.calcTimeSpan(timeFunctions.formattingTimeBack(TimeStamps[i - 1].time), sTime);
                            }
                        }
                    }

                    //convert 24 hour time to 12 hour time
                    sTime = timeFunctions.time24to12(sTime);

                            //add / back to dates
                            sDate = sDate.Substring(4, 2) + "/" + sDate.Substring(6, 2) + "/" + sDate.Substring(0, 4);

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
                            {
                    TimeStamps.Add(new TimeStamp { time = sTime, punchID = sPunchID, type = sType, date = sDate, employeeID = selectedEmployee, shiftTime = shiftTime });
                });

                }
                dbState2.Dispose();
                dbConnection2.Dispose();
                tcs.SetResult(employeeListAsync);
            });

            return tcs.Task;

        }

        private void typeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TypeDataExpandedTemplate"];
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TypeDataTemplate"];
            }


        }



        private void typeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewItem clickedItem = sender as ListViewItem;
        }


        

        private async void employeeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string startDate = startDatePicker.Date.ToString("yyyyMMdd");
            string endDate = endDatePicker.Date.ToString("yyyyMMdd");
            var employeeTapped = (Employee)e.ClickedItem;
            selectedEmployee = employeeTapped;
            await refreshingTimeStamps(employeeTapped.employeeID, startDate, endDate);
            employeeToPrint.Text = employeeTapped.fullname;

        }

        private async void startDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            string startDate = startDatePicker.Date.ToString("yyyyMMdd");
            string endDate = endDatePicker.Date.ToString("yyyyMMdd");
            if (selectedEmployee != null)
            {
                await refreshingTimeStamps(selectedEmployee.employeeID, startDate, endDate);
            }
                
        }

        private async void deltePunch_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Button button = sender as Button;
            TimeStamp selectedTimestamp = button.DataContext as TimeStamp;

            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete timestamp permanently?",
                Content = "If you delete this timestamp, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                deleteFileDialog.RequestedTheme = ElementTheme.Light;
            }
            ContentDialogResult result = await deleteFileDialog.ShowAsync();


            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
                string deleteQuery = "DELETE FROM Punches WHERE punchID =" + selectedTimestamp.punchID + ";";
                dbConnection.Prepare(deleteQuery).Step();
                dbConnection.Dispose();
                string startDate = startDatePicker.Date.ToString("yyyyMMdd");
                string endDate = endDatePicker.Date.ToString("yyyyMMdd");
                await refreshingTimeStamps(selectedEmployee.employeeID, startDate, endDate);
            }
            else
            {
                // The user clicked the CLoseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }

        }

        private void editPunch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
           selectedTimestamp = button.DataContext as TimeStamp;


            var date = DateTime.Parse(selectedTimestamp.date);
            editeDatePicker.Date = date;
            
            Date.Text = selectedTimestamp.date;
            punchtTime.Text = selectedTimestamp.time;
            employeeName.Text = selectedEmployee.fullname;
            if (selectedTimestamp.type == "In")
            {
                punchType.Text = "In";
                editPunchType.SelectedIndex = 0;
            }
            else
            {
                punchType.Text = "Out";
                editPunchType.SelectedIndex = 1;
            }


            editPunchPopUp.IsOpen = true;
        }

        private void addPunch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            addPunchPopup.IsOpen = true;
        }
        private async void saveEditPunch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            

            var time1 = editPunchTime.Time;
            string time = time1.ToString();
            time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2); //removing the : in the time

            var date = editeDatePicker.Date;
            string editDate = date.ToString("d");
            editDate = timeFunctions.formattingDate(editDate);

            string type = "1";
            if (editPunchType.SelectedIndex == 0)
            {
                type = "1";
            }
            else
            {
                type = "0";
            }
            string update = "UPDATE Punches set type='" + type + "',dateTime = '" + editDate + time + "'  WHERE punchID = '" + selectedTimestamp.punchID + "'";
            dbConnection.Prepare(update).Step();
            dbConnection.Dispose();

            string startDate = startDatePicker.Date.ToString("yyyyMMdd");
            string endDate = endDatePicker.Date.ToString("yyyyMMdd");
            await refreshingTimeStamps(selectedEmployee.employeeID, startDate, endDate);


        }

        private async void addTimestamp_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (employeeComboBox.SelectedIndex == -1)
            {
                ContentDialog deleteFileDialog = new ContentDialog
                {
                    Title = "Missing Information",
                    Content = "Please select an employee.",

                    CloseButtonText = "Close"
                };
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values["theme"].ToString() == "light")
                {
                    deleteFileDialog.RequestedTheme = ElementTheme.Light;
                }
                ContentDialogResult result = await deleteFileDialog.ShowAsync();
            }
            else if (addPunchType.SelectedIndex == -1)
            {
                ContentDialog deleteFileDialog = new ContentDialog
                {
                    Title = "Missing Information",
                    Content = "Please select a punch type.",

                    CloseButtonText = "Close"
                };
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values["theme"].ToString() == "light")
                {
                    deleteFileDialog.RequestedTheme = ElementTheme.Light;
                }
                ContentDialogResult result = await deleteFileDialog.ShowAsync();
            }
            else
            {



                var time1 = addPunchTime.Time;
                string time = time1.ToString();
                time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2); //removing the : in the time

                var date = addDatePicker.Date;
                string addDate = date.ToString("d");
                addDate = timeFunctions.formattingDate(addDate);

                string type = "1";
                if (addPunchType.SelectedIndex == 0)
                {
                    type = "1";
                }
                else
                {
                    type = "0";
                }


                var selectedEmployee = (Employee)employeeComboBox.SelectedItem;
                string employee = selectedEmployee.employeeID;



                SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");

                string sql = @"INSERT into [Punches]
([employeeID],[dateTime],[type])
VALUES
('"  + employee + "','"+ addDate + time + "','" + type + "');";

                dbConnection.Prepare(sql).Step();
                dbConnection.Dispose();

                string startDate = startDatePicker.Date.ToString("yyyyMMdd");
                string endDate = endDatePicker.Date.ToString("yyyyMMdd");
                await refreshingTimeStamps(selectedEmployee.employeeID, startDate, endDate);
            }

        }

        private string getTotalHours()
        {
            int secTotal = 0;
            int minTotal = 0;
            int hourTotal = 0;
         
                for (int i = 0; i < TimeStamps.Count() ; i++)
                {
                try
                {
                    string[] secMinHour = TimeStamps[i].shiftTime.Split(':');
                    hourTotal += Convert.ToInt32(secMinHour[0]);
                    minTotal += Convert.ToInt32(secMinHour[1]);
                    secTotal += Convert.ToInt32(secMinHour[2]);
                }
                catch { }

                }
                //make sure min and sec isnt over 60
                int carry = 0;
                while (secTotal > 59)
                {
                    secTotal = secTotal - 60;
                    carry += 1;
                }
                minTotal += carry;
                carry = 0;
                while (minTotal > 59)
                {
                    minTotal = minTotal - 60;
                    carry += 1;
                }
                hourTotal += carry;
            
            
            return hourTotal + ":" + minTotal + ":" + secTotal;
        }

        // Printer functions
        public int ticketPerPage = 43;
        async private void printButton_Click(object sender, RoutedEventArgs e)
        {
            
            printMe.Visibility = Visibility.Visible;
            double hi = printMe.Height;
            try
            {
                printMan.PrintTaskRequested += PrintTaskRequested;
            }
            catch { }

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
                printMan.PrintTaskRequested += PrintTaskRequested;
            }
            catch
            {


            }
            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
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

            printFromDate.Text = startDatePicker.Date.ToString("d");
            printToDate.Text = endDatePicker.Date.ToString("d");
            printTotalHours.Text = getTotalHours();

            printMe.Width = printPageDescription.PageSize.Width;
            printMe.MinHeight = printPageDescription.PageSize.Height;
            test1.MinHeight = printPageDescription.PageSize.Height;
            test2.Width = printPageDescription.PageSize.Width;
            test2.Height = printPageDescription.PageSize.Height;
            printMe.Height = printPageDescription.PageSize.Height;

            if (printPageDescription.PageSize.Height == 1056)
            {
                ticketPerPage = 43;
            }
            else if (printPageDescription.PageSize.Height > 1056)
            {
                ticketPerPage = 58;
            }
            if (printPageDescription.PageSize.Height < 1056)
            {
                ticketPerPage = 32;
            }
            else if (printPageDescription.PageSize.Height < 750)
            {
                ticketPerPage = 20;
            }

            await Task.Delay(50);

            listNumber = 0;
            int stampCount = 0;
            foreach(TimeStamp stamp in TimeStamps)
            {
                stampCount += 1;
                if(stampCount == ticketPerPage)
                { 
                    listNumber += 1;
                    stampCount = 0;
                }
            }
            if(stampCount < 0)
            {
                listNumber += 1;
            }

            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(listNumber + 1, PreviewPageCountType.Final);
        }

        private async void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {

            await Task.Delay(200);
            // Provide a UIElement as the print preview.
            TimeStampPrintList.Clear();
            listNumber = 0;
            int stampCount = 0;
            
            foreach(TimeStamp stamp in TimeStamps)
            {
                TimeStampPrintList.Add(stamp);
                stampCount += 1;
                if(stampCount == ticketPerPage)
                {
                        ListViewItem lvi2 = (printList).ContainerFromItem(printList.Items[ticketPerPage - 1]) as ListViewItem;

                        while (lvi2 == null)
                        {
                        try
                        {
                            lvi2 = (printList).ContainerFromItem(printList.Items[ticketPerPage - 1]) as ListViewItem;
                        }
                        catch { }
                            await Task.Delay(1);
                        }
                    
                    //await Task.Delay(1000);
                    printDoc.SetPreviewPage(listNumber + 1, printMe);
                    listNumber += 1;
                    stampCount = 0;
                    TimeStampPrintList.Clear();
                }
            }
            if(stampCount > 0)
            {
                ListViewItem lvi2 = (printList).ContainerFromItem(printList.Items[stampCount - 1]) as ListViewItem;

                while (lvi2 == null)
                {
                    lvi2 = (printList).ContainerFromItem(printList.Items[stampCount-1]) as ListViewItem;
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
            TimeStampPrintList.Clear();
            listNumber = 0;
            int stampCount = 0;
            
            foreach (TimeStamp stamp in TimeStamps)
            {
                TimeStampPrintList.Add(stamp);
                stampCount += 1;
                if (stampCount == ticketPerPage)
                {
                    ListViewItem lvi2 = (printList).ContainerFromItem(printList.Items[ticketPerPage - 1]) as ListViewItem;

                    while (lvi2 == null)
                    {
                        lvi2 = (printList).ContainerFromItem(printList.Items[ticketPerPage - 1]) as ListViewItem;
                        await Task.Delay(1);
                    }
                    printMe.MinHeight = printPageDescription.PageSize.Height;
                    printMe.Height = printPageDescription.PageSize.Height;
                    test1.MinHeight = printPageDescription.PageSize.Height;
                    test2.Width = printPageDescription.PageSize.Width;
                    test2.Height = printPageDescription.PageSize.Height;
                    printDoc.AddPage(printMe);
                    listNumber += 1;
                    stampCount = 0;
                    TimeStampPrintList.Clear();
                }
            }
            if (stampCount > 0)
            {
                ListViewItem lvi2 = (printList).ContainerFromItem(printList.Items[stampCount - 1]) as ListViewItem;

                while (lvi2 == null)
                {
                    lvi2 = (printList).ContainerFromItem(printList.Items[stampCount - 1]) as ListViewItem;
                    await Task.Delay(1);
                }
                printDoc.SetPreviewPage(listNumber + 1, printMe);
                printMe.MinHeight = printPageDescription.PageSize.Height;
                printMe.Height = printPageDescription.PageSize.Height;
                test2.Width = printPageDescription.PageSize.Width;
                test2.Height = printPageDescription.PageSize.Height;
                printDoc.AddPage( printMe);
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
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


        
    }

}

