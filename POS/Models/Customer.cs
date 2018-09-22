using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace POS.Models
{
    public class Customer
    {
        public string customerID { get; set; }
        public string full { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string spouse { get; set; }
        public string home { get; set; }
        public string cell { get; set; }
        public string work { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public string adrress2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string company { get; set; }

        public static Customer getCustomerFromID(string customerID)
        {
            SQLiteConnection dbConnection2 = new SQLiteConnection("Customers.db");
            //get customer info
            Customer cust = new Customer();
            string sSQL = @"SELECT * FROM Customers WHERE custID ='" + customerID + "'";
            ISQLiteStatement dbState = dbConnection2.Prepare(sSQL);
            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sCustID = dbState["stringCustomerID"] as string;
                string sFirst = dbState["first"] as string;
                string sLast = dbState["last"] as string;
                string sSpouse = dbState["spouse"] as string;
                string sHome = dbState["home"] as string;
                try
                {
                    sHome = addDash(sHome);
                }
                catch { }
                string sWork = dbState["work"] as string;
                try
                {
                    sWork = addDash(sWork);
                }
                catch { }
                string sCell = dbState["cell"] as string;
                try
                {
                    sCell = addDash(sCell);
                }
                catch { }
                string sEmail = dbState["email"] as string;
                string sCity = dbState["city"] as string;
                string sState = dbState["state"] as string;
                string sZip = dbState["zip"] as string;
                string sAdd1 = dbState["add1"] as string;
                string sAdd2 = dbState["add2"] as string;
                string sCompany = dbState["company"] as string;
                cust = new Customer { customerID = sCustID, full = sFirst + " " + sLast, first = sFirst, last = sLast, spouse = sSpouse, home = sHome, work = sWork, cell = sCell, state = sState, city = sCity, address1 = sAdd1, email = sEmail, zip = sZip, adrress2 = sAdd2, company = sCompany  };
                break;
            }

            dbState.Dispose();
            dbConnection2.Dispose();
            return cust;

        }

        public static Task<List<Ticket>> FilterCustomerAsync(ObservableCollection<Customer> searchResults, int searchType, string query)
        {
            TaskCompletionSource<List<Ticket>> tcs = new TaskCompletionSource<List<Ticket>>();

            Task.Run(async () =>
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchRequested"] = "true";
                while((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchLoading"] == "true")
                {
                    await Task.Delay(100);

                }
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchRequested"] = "false";
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchLoading"] = "true";
                List<Ticket> ticketList = new List<Ticket>();

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                            searchResults.Clear();
                    });

                if (query.Length >= 1)
                {
                    query = query.Replace("-", "");
                    SQLiteConnection dbConnection = new SQLiteConnection("Customers.db");
                    string sSQL = null;

                    sSQL = @"SELECT [stringCustomerID],[first],[last],[spouse],[home],[work],[cell],[company]  FROM Customers";
                    ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


                    
;

                    switch (searchType) {


                        //Load into observable collection
                        case 0://name search
                            
                                for (int i = 0; dbState.Step() == SQLiteResult.ROW; i++)
                                {
                                    if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchRequested"] == "true")
                                    {
                                        break;

                                    }
                                    string sCustID = dbState["stringCustomerID"] as string;
                                    string sFirst = dbState["first"] as string;
                                    string sLast = dbState["last"] as string;
                                    string sSpouse = dbState["spouse"] as string;
                                    string sHome = dbState["home"] as string;
                                    string sWork = dbState["work"] as string;
                                    string sCell = dbState["cell"] as string;

                                    try
                                    {
                                        if (sFirst.ToUpper().Contains(query) || sLast.ToUpper().Contains(query) || sSpouse.ToUpper().Contains(query))
                                        {
                                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                            {
                                                searchResults.Add(new Customer { customerID = sCustID, full = sFirst + " " + sLast, first = sFirst, last = sLast, spouse = sSpouse, home = sHome, work = sWork, cell = sCell });
                                            });
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                            break;
                        case 1://number search
                            
                                for (int i = 0; dbState.Step() == SQLiteResult.ROW; i++)
                                {
                                    if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchRequested"] == "true")
                                    {
                                        break;

                                    }

                                    string sCustID = dbState["stringCustomerID"] as string;
                                    string sFirst = dbState["first"] as string;
                                    string sLast = dbState["last"] as string;
                                    string sSpouse = dbState["spouse"] as string;
                                    string sHome = dbState["home"] as string;
                                    string sWork = dbState["work"] as string;
                                    string sCell = dbState["cell"] as string;

                                    if (sHome.Contains(query) || sCell.Contains(query) || sWork.Contains(query))
                                    {
                                        try
                                        {
                                            sHome = addDash(sHome);
                                        }
                                        catch { }
                                        try
                                        {
                                            sWork = addDash(sWork);
                                        }
                                        catch { }
                                        try
                                        {
                                            sCell = addDash(sCell);
                                        }
                                        catch { }

                                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {
                                            searchResults.Add(new Customer { customerID = sCustID, full = sFirst + " " + sLast, first = sFirst, last = sLast, spouse = sSpouse, home = sHome, work = sWork, cell = sCell });
                                        });
                                    }

                                }
                            break;
                        case 2:
                            for (int i = 0; dbState.Step() == SQLiteResult.ROW; i++)
                            {
                                if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchRequested"] == "true")
                                {
                                    break;

                                }

                                string sCustID = dbState["stringCustomerID"] as string;
                                string sFirst = dbState["first"] as string;
                                string sLast = dbState["last"] as string;
                                string sSpouse = dbState["spouse"] as string;
                                string sHome = dbState["home"] as string;
                                string sWork = dbState["work"] as string;
                                string sCell = dbState["cell"] as string;
                                string sCompany = dbState["company"] as string;

                                if (sCompany.ToUpper().Contains(query))
                                {
                                    try
                                    {
                                        sHome = addDash(sHome);
                                    }
                                    catch { }
                                    try
                                    {
                                        sWork = addDash(sWork);
                                    }
                                    catch { }
                                    try
                                    {
                                        sCell = addDash(sCell);
                                    }
                                    catch { }

                                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        searchResults.Add(new Customer { customerID = sCustID, full = sFirst + " " + sLast, first = sFirst, last = sLast, spouse = sSpouse, home = sHome, work = sWork, cell = sCell });
                                    });
                                }

                            }
                            break;
                      
                    }
                    dbState.Dispose();
                    dbConnection.Dispose();
                    tcs.SetResult(ticketList);
                }
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["customerSearchLoading"] = "false";
            });
            return tcs.Task;

        }//search through customer database

        private static string addDash(string number)
        {
            return number = number.Substring(0, 3) + "-" + number.Substring(3, 3) + "-" + number.Substring(6, 4); ;
        }
    }

    

    public class customerManager
    {
        public static ObservableCollection<Customer> GetCustomer()
        {
            var customers = new ObservableCollection<Customer>();
            // customers.Add(new Customer { first = "John", last = "Doe", fullname="John Doe", phoneNumber ="5555555555", phoneNumber2 = "5555555554", address= "50 Smith st. Woodville, NY 10955"});
            return customers;
        }
    }


}
