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
    public class Employee
    {
        public string employeeID { get; set;}
        public string first { get; set; }
        public string last { get; set; }
        public string fullname { get; set; }
        public int passcode { get; set; }
        public bool canDoReturns { get; set; }
        public bool canDoReturnsWOTicket { get; set; }
        public bool isAdmin { get; set; }

        public static void refreshingEmployeeList(ObservableCollection<Employee> Employees)//repopulates Employees ObservCollection
        {
            Employees.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            string deleteQuery = "DELETE FROM EmployeeList WHERE first = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT [employeeID],[first],[last] FROM EmployeeList WHERE deleted IS NULL";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                var sID = (long)dbState["employeeID"];
                string sFirst = dbState["first"] as string;
                string sLast = dbState["last"] as string;

                //< correction > 
                sFirst = sFirst.Replace("''", "'");
                sLast = sLast.Replace("''", "'");
                //</ correction > 
                string full = sFirst + " " + sLast;

                //Load into observable collection
                Employees.Add(new Employee { employeeID = Convert.ToString(sID), first = sFirst, last = sLast, fullname = full });
            }
            dbState.Dispose();
            dbConnection.Dispose();
        }

        public static Task<List<Employee>> refreshingEmployeeListAsync(ObservableCollection<Employee> Employees)//repopulates Employees ObservCollection
        {
            TaskCompletionSource<List<Employee>> tcs = new TaskCompletionSource<List<Employee>>();
            Task.Run(async () =>
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
                string sSQL = @"SELECT [employeeID],[first],[last] FROM EmployeeList WHERE deleted IS NULL";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                List<Employee> employeeListAsync = new List<Employee>();


                while (dbState.Step() == SQLiteResult.ROW)
                {
                    var sID = (long)dbState["employeeID"];
                    string sFirst = dbState["first"] as string;
                    string sLast = dbState["last"] as string;

                    //< correction > 
                    sFirst = sFirst.Replace("''", "'");
                    sLast = sLast.Replace("''", "'");
                    //</ correction > 
                    string full = sFirst + " " + sLast;

                    //Load into observable collection
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {

                        Employees.Add(new Employee { employeeID = Convert.ToString(sID), first = sFirst, last = sLast, fullname = full });

                    });
                }
                dbState.Dispose();
                dbConnection.Dispose();
                tcs.SetResult(employeeListAsync);
            });

            return tcs.Task;

        }
    }   

        public class employeeManager
        {
            public static ObservableCollection<Employee> GetEmployee()
            {
                var employees = new ObservableCollection<Employee>();
               // employees.Add(new Employee { first = "John", last = "Doe", fullname="John Doe", passcode = 123, canDoReturns = true, canDoReturnsWOTicket= true, isAdmin=false });

                return employees;
            }
        }

    
}
