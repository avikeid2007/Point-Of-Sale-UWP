using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Till
    {
        public string tillID { get; set; }
        public string name { get; set; }
        public string amount { get; set; }



        public static void refreshingTills(ObservableCollection<Till> Tills)//repopulates Till ObservCollection
        {
            Tills.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Tills.db");

            string deleteQuery = "DELETE FROM Tills WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM Tills";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            while (dbState.Step() == SQLiteResult.ROW)
            {
                var sID = (long)dbState["tillID"];
                string sName = dbState["name"] as string;
                string sAmount = dbState["amount"] as string;
                string sDeleted = dbState["deleted"] as string;

                //Load into observable collection
                if(sDeleted!="True")
                {
                    
                Tills.Add(new Till { tillID = sID.ToString(), name = sName, amount = sAmount });
                }
            }
            dbConnection.Dispose();
            dbState.Dispose();
        }

        public static Till getTillByID(string tillID)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Tills.db");

            string sSQL = @"SELECT * FROM Tills WHERE tillID ='"+ tillID +"'";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            Till till = new Till();

            while (dbState.Step() == SQLiteResult.ROW)
            {
                var sID = (long)dbState["tillID"];
                string sName = dbState["name"] as string;
                string sAmount = dbState["amount"] as string;
                string sDeleted = dbState["deleted"] as string;

                
                
                    till = (new Till { tillID = sID.ToString(), name = sName, amount = sAmount });
                
            }
            dbConnection.Dispose();
            dbState.Dispose();
            return till;

            
        }
    }

    public class tillManager
    {
        public static ObservableCollection<Till> GetTill()
        {
            var till = new ObservableCollection<Till>();
            // taxRates.Add(new Tax { taxKey = "1", Name = "Sales", rate="8.75"});

            return till;
        }
    }


}
