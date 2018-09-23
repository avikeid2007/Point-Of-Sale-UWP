using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{

    public class Payments
    {
        public string payID { get; set; }
        public string tillID { get; set; }
        public string type { get; set; }
        public string amount { get; set; }
        public string till { get; set; }

        
        public static void getPaymentsLow(string ticketID, ObservableCollection<Payments>  payments)
        {


            SQLiteConnection dbConnection2 = new SQLiteConnection("Tickets.db");
            string sSQL2 = @"SELECT [payType],[tenderAmount] FROM Payments WHERE stringTicketID = '" + ticketID + "' AND deleted != '"+true+"'";
            ISQLiteStatement dbState2 = dbConnection2.Prepare(sSQL2);
            while (dbState2.Step() == SQLiteResult.ROW)
            {
                var sPayType = (long)dbState2["payType"];
                double sTenderAmount = (double)dbState2["tenderAmount"];
                string payType = "";
                switch (sPayType)
                {
                    case 1:
                        payType = "Cash";
                        break;
                    case 2:
                        payType = "Credit";
                        break;
                    case 3:
                        payType = "Check";
                        break;
                }

                payments.Add(new Payments { type = payType, amount = "$" + String.Format("{0:#.00}", sTenderAmount) });
                

            }
            dbConnection2.Dispose();
            dbState2.Dispose();

        }

        public static ObservableCollection<Payments> getPayments(string ticketID)
        {
            ObservableCollection<Payments> Payments;

            Payments = payManager.GetPay();

            SQLiteConnection dbConnection2 = new SQLiteConnection("Tickets.db");
            string sSQL2 = @"SELECT [payType],[tenderAmount],[drawerID],[PayID] FROM Payments WHERE stringTicketID = '" + ticketID + " AND deleted != '"+true+"'";
            ISQLiteStatement dbState2 = dbConnection2.Prepare(sSQL2);
            while (dbState2.Step() == SQLiteResult.ROW)
            {
                var sPayID = (long)dbState2["PayID"];
                var sPayType = (long)dbState2["payType"];
                var sDrawerID = (long)dbState2["PayID"];
                try//this is here because it is an error when credit card doesnt have a drawer
                {
                   sDrawerID = (long)dbState2["drawerID"];
                }
                catch { sDrawerID = -1; }
                double sTenderAmount = (double)dbState2["tenderAmount"];
                string payType = "";
                switch (sPayType)
                {
                    case 1:
                        payType = "Cash";
                        break;
                    case 2:
                        payType = "Credit";
                        break;
                    case 3:
                        payType = "Check";
                        break;
                }

                string tillName = Till.getTillByID(Convert.ToString(sDrawerID)).name;
                Payments.Add(new Payments { type = payType, amount = "$" + String.Format("{0:#.00}", sTenderAmount),till = tillName, tillID= Convert.ToString(sDrawerID), payID =Convert.ToString(sPayID) });


            }
            dbConnection2.Dispose();
            dbState2.Dispose();

            return Payments;
        }

        public static int getTicketPayCount(string ticketID)
        {
            int paymentCount = 0;
            SQLiteConnection dbConnection2 = new SQLiteConnection("Tickets.db");
            string sSQL2 = @"SELECT [payType] FROM Payments WHERE stringTicketID = '" + ticketID + "'";
            ISQLiteStatement dbState = dbConnection2.Prepare(sSQL2);
            while (dbState.Step() == SQLiteResult.ROW)
            {

                paymentCount += 1;
            }
            dbConnection2.Dispose();
            dbState.Dispose();
            return paymentCount;
        }

    }

    public class payManager
    {
        public static ObservableCollection<Payments> GetPay()
        {
            var pay = new ObservableCollection<Payments>();
            // employees.Add(new Employee { first = "John", last = "Doe", fullname="John Doe", passcode = 123, canDoReturns = true, canDoReturnsWOTicket= true, isAdmin=false });

            
            return pay;
        }
    }

}
