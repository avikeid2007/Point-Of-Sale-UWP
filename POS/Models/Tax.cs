using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Tax
    {
        public string taxKey { get; set; }
        public string name { get; set; }
        public string rate { get; set; }
        public string taxAtAmount { get; set; }

        public static string calcTax(ObservableCollection<Item> listView)
        {
            decimal tax = 0;
            foreach (var item in listView)
            {
                tax = tax + Convert.ToDecimal(item.taxAmount);
            }
            if (tax < 1)
            {
                return "0" + String.Format("{0:#.00}", tax);
            }
            return String.Format("{0:#.00}", tax);
        }

        public static void refreshingTaxRates(ObservableCollection<Tax> TaxList)
        {
            TaxList.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("TaxSettings.db");

            string deleteQuery = "DELETE FROM TaxRate WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT [stringTaxID],[name],[rate],[valueToTaxAt] FROM TaxRate WHERE deleted IS NULL";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            string sID = null;
            while (dbState.Step() == SQLiteResult.ROW)
            {
                sID = dbState["stringTaxID"] as string;
                string sName = dbState["name"] as string;
                string sRate = dbState["rate"] as string;
                string sPriceToTax = dbState["valueToTaxAt"] as string;



                //Load into observable collection
                TaxList.Add(new Tax { taxKey = sID, name = sName, rate = sRate, taxAtAmount = sPriceToTax });
            }
            dbState.Dispose();
            dbConnection.Dispose();

        }

        public static void refreshingTaxRatesView(ObservableCollection<Tax> TaxList)
        {
            TaxList.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("TaxSettings.db");

            string deleteQuery = "DELETE FROM TaxRate WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT [stringTaxID],[name],[rate],[valueToTaxAt] FROM TaxRate WHERE deleted IS NULL";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            string sID = null;
            while (dbState.Step() == SQLiteResult.ROW)
            {
                sID = dbState["stringTaxID"] as string;
                string sName = dbState["name"] as string;
                string sRate = dbState["rate"] as string;
                string sPriceToTax = dbState["valueToTaxAt"] as string;



                //Load into observable collection
                TaxList.Add(new Tax { taxKey = sID, name = sName, rate = sRate, taxAtAmount = sPriceToTax });
            }
            dbState.Dispose();
            dbConnection.Dispose();

        }
    }

    public class taxManager
    {
        public static ObservableCollection<Tax> GetTax()
        {
            var taxRates = new ObservableCollection<Tax>();
            // taxRates.Add(new Tax { taxKey = "1", Name = "Sales", rate="8.75"});

            return taxRates;
        }
    }


}
