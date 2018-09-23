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
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using SQLitePCL;
using System.Threading.Tasks;
using POS.Models;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportsDashboard : Page
    {

        public ObservableCollection<Employee> Employees;
        public ObservableCollection<Item> Items;
        ObservableCollection<ItemAmounts> plotValues1 = new ObservableCollection<ItemAmounts>();


        public ReportsDashboard()
        {
            Employees = employeeManager.GetEmployee();
            Items = itemManager.GetItem();
            this.InitializeComponent();
        }
        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            SolidColorBrush test = new SolidColorBrush();
            test.Color = (Color)this.Resources["SystemAccentColor"];
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush = new SolidColorBrush(Colors.White);
                brush.Opacity = 0.3;
                chartBackColor.Background = brush;
            }

            /*
            //string colorHex = test.Color.ToString();
            LinearGradientBrush test2 = new LinearGradientBrush();
            test2.StartPoint = new Point(0.1, .2);
            test2.EndPoint = new Point(.8, .9);
            var color = GetSolidColorBrush("#FFCD3927").Color;


            GradientStop start = new GradientStop();
            start.Color = test.Color;
            start.Offset = 0.0;
            test2.GradientStops.Add(start);

            GradientStop stop = new GradientStop();
            stop.Color = Colors.Black;
            stop.Offset = .9;
            test2.GradientStops.Add(stop);
            */


            refundBackground.Fill = test;
            pieBackground.Fill = test;
            revenueBackground.Fill = test;
            costBackground.Fill = test;
            hourBackground.Fill = test;
            refreshingEmployeeList();

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            incomeGrid.Fade(value: 1f, duration: 600, delay: 275).StartAsync();
            refundGrid.Fade(value: 1f, duration: 600, delay: 375).StartAsync();
            mostSoldGrid.Fade(value: 1f, duration: 600, delay: 475).StartAsync();
            costGrid.Fade(value: 1f, duration: 600, delay: 575).StartAsync();
            hoursGrid.Fade(value: 1f, duration: 600, delay: 675).StartAsync();
            todayIncome.Text = await IncomeAsync("today");
            weekIncome.Text = await IncomeAsync("week");
            monthIncome.Text = await IncomeAsync("month");
            yearIncome.Text = await IncomeAsync("year");

            string year = DateTime.Today.ToString("yyyy");
            int month = Convert.ToInt32(DateTime.Today.ToString("MM"));// used to cut off graph and to switch to chart days instead of months
            string day = DateTime.Today.ToString("dd");

            if (month >= 4)//if it is the 4-12 plot the month total
            {
                double jan = await MonthGraphIncomeAsync(year + "0101", year + "0132");
                double feb = await MonthGraphIncomeAsync(year + "0201", year + "0232");
                double mar = await MonthGraphIncomeAsync(year + "0301", year + "0332");
                double apr = await MonthGraphIncomeAsync(year + "0401", year + "0432");
                double may = await MonthGraphIncomeAsync(year + "0501", year + "0532");
                double jun = await MonthGraphIncomeAsync(year + "0601", year + "0632");
                double jul = await MonthGraphIncomeAsync(year + "0701", year + "0732");
                double aug = await MonthGraphIncomeAsync(year + "0801", year + "0832");
                double sep = await MonthGraphIncomeAsync(year + "0901", year + "0932");
                double oct = await MonthGraphIncomeAsync(year + "1001", year + "1032");
                double nov = await MonthGraphIncomeAsync(year + "1101", year + "1132");
                double dec = await MonthGraphIncomeAsync(year + "1201", year + "1232");


                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    string completed = await settingChartAsync(jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec, month);
                });
            }
            else//plot the day totals
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    string comp = await settingChartDayAsync(year, Convert.ToString(month), day);
                });
            }

            todayHours.Text = await workHoursCalcAsync("today");
            weekHours.Text = await workHoursCalcAsync("week");
            monthHours.Text = await workHoursCalcAsync("month");
            yearHours.Text = await workHoursCalcAsync("year");

            refreshingItems();
            monthCost.Text = await CostAsync("month");
            todayCost.Text = await CostAsync("today");
            weekCost.Text = await CostAsync("month");
            yearCost.Text = await CostAsync("year");

            LoadPieContents();
            
        }

        public class ItemAmounts
        {
            public string Name { get; set; }
            public double Amount { get; set; }

        }
        private void LoadPieContents()
        {

            SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
            
            string sSQL = @"SELECT [items],[quantities] FROM Tickets  ORDER BY ticketID DESC LIMIT -1";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)//get item ids from tickets
            {
                string sItems = dbState["items"] as string;
                string sQuan = dbState["quantities"] as string;
                string[] itemArry = sItems.Split(':');
                string[] quanArry = sQuan.Split(':');
                for (int i = 0; i < itemArry.Count(); i++)//for each item in ticket
                {
                    foreach (Item item in Items)
                    {
                        if (item.itemID == itemArry[i])
                        {
                            item.minQuantity = Convert.ToString(Convert.ToDouble(item.minQuantity) +Convert.ToDouble(quanArry[i]));
                        }
                    }
                }
            }

            List<ItemAmounts> pieValues = new List<ItemAmounts>();
            foreach (Item item in Items)
            {

                if(pieValues.Count() == 5)
                {
                    ItemAmounts lowest = new ItemAmounts() { Amount =double.MaxValue };
                    foreach(ItemAmounts data in pieValues)
                    {
                            if (data.Amount < lowest.Amount)
                            {
                            lowest = data;
                            }
                    }
                    if (Convert.ToDouble(item.minQuantity) >= lowest.Amount)
                    {
                        ItemAmounts newItem = new ItemAmounts() {Amount = Convert.ToDouble(item.minQuantity), Name = item.name };
                        pieValues[pieValues.IndexOf(lowest)] = newItem;
                    }
                }
               else
                {
                    pieValues.Add(new ItemAmounts() { Name = item.name , Amount = Convert.ToDouble(item.minQuantity) });

                }
            }

            (PieChart.Series[0] as PieSeries).ItemsSource = pieValues;

        }

        public void refreshingItems()
        {
            Items.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Items WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM Items ";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sID = dbState["stringItemID"] as string;
                string sName = dbState["name"] as string;
                string sCost = dbState["cost"] as string;
                if(sCost == "")
                {
                    sCost = "0";
                }


                //Load into observable collection

                    Items.Add(new Item { itemID = sID, name =sName, cost = sCost, minQuantity = "0"});
                
            }
            dbState.Dispose();
            dbConnection.Dispose();
        }

        public Task<string> IncomeAsync(string length)
        {

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                DateTime date = DateTime.Today;
                string startingDate = null;

                switch (length)
                {
                    case "week":
                        int offset = date.DayOfWeek - DayOfWeek.Sunday;
                        DateTime lastSunday = date.AddDays(-offset);
                        startingDate = lastSunday.ToString("yyyyMMdd");
                        break;
                    case "month":
                        startingDate = DateTime.Today.ToString("yyyyMM") + "01";
                        break;
                    case "year":
                        startingDate = DateTime.Today.ToString("yyyy") + "0101";
                        break;
                    case "today":
                        startingDate = DateTime.Today.ToString("yyyyMMdd");
                        break;


                }

 
                SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
                decimal total = 0;
                string sSQL = @"SELECT [total] FROM Tickets WHERE inputDate >='" + startingDate + "' ORDER BY ticketID DESC LIMIT -1";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    string sTotal = dbState["total"] as string;
                    total = total + Convert.ToDecimal(sTotal);
                }
                dbConnection.Dispose();
                tcs.SetResult(Convert.ToString(total));
            });

            return tcs.Task;
        }
        public Task<string> CostAsync(string length)
        {

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                DateTime date = DateTime.Today;
                string startingDate = null;

                switch (length)
                {
                    case "week":
                        int offset = date.DayOfWeek - DayOfWeek.Sunday;
                        DateTime lastSunday = date.AddDays(-offset);
                        startingDate = lastSunday.ToString("yyyyMMdd");
                        break;
                    case "month":
                        startingDate = DateTime.Today.ToString("yyyyMM") + "01";
                        break;
                    case "year":
                        startingDate = DateTime.Today.ToString("yyyy") + "0101";
                        break;
                    case "today":
                        startingDate = DateTime.Today.ToString("yyyyMMdd");
                        break;


                }


                SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
                double totalCost = 0;
                string sSQL = @"SELECT [items],[quantities] FROM Tickets WHERE inputDate >='" + startingDate + "' ORDER BY ticketID DESC LIMIT -1";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


                while (dbState.Step() == SQLiteResult.ROW)//get item ids from tickets
                {
                    string sItems = dbState["items"] as string;
                    string sQuan = dbState["quantities"] as string;
                    string[] itemArry = sItems.Split(':');
                    string[] quanArry = sQuan.Split(':');


                    for (int i = 0; i < itemArry.Count(); i++)//for each item in ticket
                    {
                        //look for cost and add them up
                        foreach (Item item in Items)
                        {
                            if (item.itemID == itemArry[i])
                            {
                                totalCost += Convert.ToDouble(item.cost) * Convert.ToDouble(quanArry[i]);
                            }

                        }

                    }      


                }
                string totalCostResult = String.Format("{0:#.00}", Convert.ToString(totalCost));
                try
                {
                    totalCostResult = totalCostResult.Substring(0, totalCostResult.IndexOf('.') + 3);
                }
                catch { }
                if (totalCostResult.Substring(0,1) == ".")
                {
                    totalCostResult = "0" + totalCostResult;
                }
                dbConnection.Dispose();
                dbState.Dispose();
                tcs.SetResult(totalCostResult);
            });

            return tcs.Task;
        }
        public Task<double> MonthGraphIncomeAsync(string start, string end)//calcs total. includes starting date and ending date.
        {

            TaskCompletionSource<double> tcs = new TaskCompletionSource<double>();
            Task.Run(() =>
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
                double total = 0;
                string sSQL = @"SELECT [total] FROM Tickets WHERE inputDate >='" + start + "' AND inputDate <='" + end + "' ORDER BY ticketID DESC LIMIT -1";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    string sTotal = dbState["total"] as string;
                    total = total + Convert.ToDouble(sTotal);
                }
                dbConnection.Dispose();
                tcs.SetResult(Convert.ToInt64(total));
            });

            return tcs.Task; ;
        }
        public Task<string> workHoursCalcAsync(string length)
        {

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                DateTime date = DateTime.Today;

                string startingDate = null;

                switch (length)
                {
                    case "week":
                        int offset = date.DayOfWeek - DayOfWeek.Sunday;
                        DateTime lastSunday = date.AddDays(-offset);
                        startingDate = lastSunday.ToString("yyyyMMdd");
                        break;
                    case "month":
                        startingDate = DateTime.Today.ToString("yyyyMM") + "01";
                        break;
                    case "year":
                        startingDate = DateTime.Today.ToString("yyyy") + "0101";
                        break;
                    case "today":
                        startingDate = DateTime.Today.ToString("yyyyMMdd");
                        break;


                }



                SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
                string shiftTime = null;


                List<TimeStamp> allTimeStamps = new List<TimeStamp>();

                string sSQL = @"SELECT * FROM Punches WHERE dateTime >='" + startingDate +"000000" + "' ORDER BY dateTime";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    //string sTime = dbState["time"] as string;
                    string sDateTime = dbState["dateTime"] as string;
                    string sType = dbState["type"] as string;
                    string sEmploy = dbState["employeeID"] as string;
                    string time = sDateTime.Substring(8, 6);
                    sDateTime = sDateTime.Substring(0, 8);

                    allTimeStamps.Add(new TimeStamp { time = time, date = sDateTime, type = sType, employeeID = sEmploy });
                }


                foreach (Employee employee in Employees)
                {
                    List<TimeStamp> employeeTimeStamp = new List<TimeStamp>();
                    foreach (TimeStamp punch in allTimeStamps)
                    {
                        if (punch.employeeID == employee.employeeID)
                        {
                            employeeTimeStamp.Add(punch);//get current employees timestamps
                        }
                    }

                    for (int i = 0; i <= employeeTimeStamp.Count - 1; i++)//calc time for current employee
                    {
                        if (i != 0)
                        {
                            if (employeeTimeStamp[i - 1].type == "1")
                            {
                                shiftTime = shiftTime + timeFunctions.calcTimeSpan(employeeTimeStamp[i - 1].time, employeeTimeStamp[i].time) + ";";
                            }
                        }
                    }
                }



                int secTotal = 0;
                int minTotal = 0;
                int hourTotal = 0;
                //add up all employees time
                try
                {
                    string[] shiftTimeArray = shiftTime.Split(';');
                    for (int i = 0; i < shiftTimeArray.Count() - 1; i++)
                    {
                        string[] secMinHour = shiftTimeArray[i].Split(':');
                        hourTotal += Convert.ToInt32(secMinHour[0]);
                        minTotal += Convert.ToInt32(secMinHour[1]);
                        secTotal += Convert.ToInt32(secMinHour[2]);


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
                }
                catch { }

                dbConnection.Dispose();
                tcs.SetResult(hourTotal + ":" + minTotal + ":" + secTotal);
            });
            return tcs.Task;
        }



        public Task<string> settingChartAsync(double jan, double feb, double mar, double apr, double may, double jun, double jul, double aug, double sep, double oct, double nov, double dec, int month)
        {

            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task.Run(async () =>
            {
                List<ItemAmounts> plotValues = new List<ItemAmounts>();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    plotValues.Add(new ItemAmounts() { Name = "Jan", Amount = jan });
                    if (month > 1)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Feb", Amount = feb });
                    }
                    if (month > 2)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Mar", Amount = mar });
                    }
                    if (month > 3)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Apr", Amount = apr });
                    }
                    if (month > 4)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "May", Amount = may });
                    }
                    if (month > 5)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Jun", Amount = jun });
                    }
                    if (month > 6)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Jul", Amount = jul });
                    }
                    if (month > 7)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Aug", Amount = aug });
                    }
                    if (month > 8)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Spet", Amount = sep });
                    }
                    if (month > 9)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Oct", Amount = oct });
                    }
                    if (month > 10)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Nov", Amount = nov });
                    }
                    if (month > 11)
                    {
                        plotValues.Add(new ItemAmounts() { Name = "Dec", Amount = dec });
                    }
                    (yearLine.Series[0] as LineSeries).ItemsSource = plotValues;
                });
                tcs.SetResult("completed");
            });
            return tcs.Task;
        }
        public Task<string> settingChartDayAsync(string year, string month, string day)
        {
            //List<ItemAmounts> plotValues = new List<ItemAmounts>();

            (yearLine.Series[0] as LineSeries).ItemsSource = plotValues1;
           
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task.Run(async () =>
            {
                if(month.Count() == 1)
                {
                    month = "0" + month;
                }
                int dayCount = Convert.ToInt32(year + "0101");
                int endDate = Convert.ToInt32(year + month + day);
                //string name = "";
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //removing x axis labels
                    lineSeries.IndependentAxis =
                            new CategoryAxis
                            {
                                Orientation = AxisOrientation.X,
                                Height = 0
                            };
                });
                for (; dayCount <= endDate; dayCount++)
                {
                    string dayCount1 = Convert.ToString(dayCount);
                    //name = Convert.ToString(dayCount).Substring(Convert.ToString(dayCount).Length - 2, 2 ) + "/" + Convert.ToString(dayCount).Substring(0,Convert.ToString(dayCount).Length - 2);
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //adding a column every for every day
                        ColumnDefinition col1 = new ColumnDefinition();
                        col1.Width = new GridLength(1, GridUnitType.Star);
                        dateGrid.ColumnDefinitions.Add(col1);
                    });

                    if (dayCount1 ==Convert.ToString(year + "0101"))//add jan label at day 1
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            TextBlock jan = new TextBlock();
                            jan.Text = "January";
                            dateGrid.Children.Add(jan);
                            Grid.SetColumn(jan, 1);
                            Grid.SetColumnSpan(jan, 50);
                        });
                    }
                    else if (dayCount1 == Convert.ToString(year + "0201")) //add feb label
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            TextBlock feb = new TextBlock();
                            feb.Text = "February";
                            dateGrid.Children.Add(feb);
                            Grid.SetColumn(feb, 31);
                            Grid.SetColumnSpan(feb, 50);
                        });
                    }
                    else if (dayCount1 == Convert.ToString(year + "0301"))//add march label
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            TextBlock feb = new TextBlock();
                            feb.Text = "March";
                            dateGrid.Children.Add(feb);
                            Grid.SetColumn(feb, 59);
                            Grid.SetColumnSpan(feb, 50);
                        });
                    }


                    double dayTotal = await MonthGraphIncomeAsync(dayCount1, dayCount1); // get day total

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        plotValues1.Add(new ItemAmounts() { Name = dayCount1, Amount = dayTotal }); //update observable collection 
                    });

                    //at the last day of the month, set the count to the next day
                    if (dayCount == Convert.ToInt32(Convert.ToString(year + "0131")))
                    {
                        dayCount = Convert.ToInt32(Convert.ToString(year + "0200"));
                    }
                    else if(dayCount == Convert.ToInt32(Convert.ToString(year + "0228")))
                    {
                        dayCount = Convert.ToInt32(Convert.ToString(year + "0300"));
                    }

                }


                tcs.SetResult("completed");
            });
            return tcs.Task;
        }


        public string formattingTimeBack(string time)
        {
            if (Convert.ToInt32(time.Substring(0, time.IndexOf(":"))) < 10)
            {
                time = "0" + time;
            }

            if (time.Contains("PM") && time.Substring(0, 2) != "12")
            {
                time = Convert.ToString(Convert.ToInt32(time.Substring(0, 2)) + 12) + time.Substring(3, 2) + time.Substring(6, 2);

            }
            else if (time.Contains("PM") || time.Contains("AM"))
            {
                time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2);

            }


            //24 hour time
            return time;
        }

        public void refreshingEmployeeList()//repopulates Employees ObservCollection
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            string sSQL = @"SELECT [stringEmployeeID],[first],[last] FROM EmployeeList";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sID = dbState["stringEmployeeID"] as string;
                string sFirst = dbState["first"] as string;
                string sLast = dbState["last"] as string;

                string full = sFirst + " " + sLast;

                //Load into observable collection
                Employees.Add(new Employee { employeeID = sID, first = sFirst, last = sLast, fullname = full });
            }
            dbState.Dispose();
            dbConnection.Dispose();
        }

       
    }
}
