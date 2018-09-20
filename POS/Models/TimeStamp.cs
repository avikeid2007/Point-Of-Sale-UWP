using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class TimeStamp
    {
        public long punchID { get; set; }
        public string employeeID { get; set; }
        public string type { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string shiftTime { get; set; }

        public static long getLastPunchID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            string sSQL = @"SELECT [punchID] FROM Punches ORDER BY punchID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            var lastStringID = (long)0;
            while (dbState.Step() == SQLiteResult.ROW)
            {
                lastStringID = (long)dbState["punchID"];
                break;
            }
            dbConnection.Dispose();
            dbState.Dispose();
            return lastStringID;
        }

    }

    public class timeManager
    {
        public static ObservableCollection<TimeStamp> GetTimeStamp()
        {
            var time = new ObservableCollection<TimeStamp>();
            // employees.Add(new Employee { first = "John", last = "Doe", fullname="John Doe", passcode = 123, canDoReturns = true, canDoReturnsWOTicket= true, isAdmin=false });

            return time;
        }
    }
}
