using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Item
    {
        public string itemID { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string price { get; set; }
        public string modID { get; set; }
        public List<string> modName { get; set; }
        public List<string> modPrice { get; set; }
        public string minQuantity { get; set; }
        public string discount { get; set; }
        public string description { get; set; }
        public string taxID { get; set; }
        public string taxAmount { get; set; }
        public string cost { get; set; }
        public bool gray { get; set; }

        public static void refreshingItems(string catID, ObservableCollection<Item> Items )
        {
            Items.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Items WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM Items WHERE category = '" + catID + "'";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sID = dbState["stringItemID"] as string;
                string sName = dbState["name"] as string;
                string sPrice = dbState["price"] as string;
                string sPosition = dbState["category"] as string;
                string sTax = dbState["taxID"] as string;
                string sModGroups = dbState["modGroupID"] as string;
                string sCost = dbState["cost"] as string;
                string sMinQuantity = dbState["minQuantity"] as string;
                string sDeleted = dbState["deleted"] as string;

                //Load into observable collection
                if (sDeleted != "1")
                {
                    Items.Add(new Item { itemID = sID, price = sPrice, name = sName, category = sPosition, minQuantity = sMinQuantity, taxID = sTax, modID = sModGroups, cost = sCost });
                }
            }
            dbState.Dispose();
            dbConnection.Dispose();
        }
    }

    public class itemManager
    {
        public static ObservableCollection<Item> GetItem()
        {
            var items = new ObservableCollection<Item>();
            // items.Add(new Item { name = "t-shirt", category = "Shirts", price="9.99", minQuantity ="10", TaxID1= "1"});
            return items;
        }
    }
}
