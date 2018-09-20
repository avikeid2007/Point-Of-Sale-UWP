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
    public class Ticket
    {
        public string ticketID { get; set; }
        public string customerID { get; set; }
        public string custNumber { get; set; }
        public string custName { get; set; }
        public string inputDate { get; set; }
        public string inputTime { get; set; }
        public string readyDate { get; set; }
        public string readyTime { get; set; }
        public string total { get; set; }
        public string items { get; set; }
        public string quantities { get; set; }
        public string prices { get; set; }
        public string taxID { get; set; }
        public string modifiers { get; set; }
        public string discounts { get; set; }
        public string payments { get; set; }
        public string notes { get; set; }
        public string changeAmount{get; set;}

        public static string getTicketID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Tickets.db");
            string sSQL = @"SELECT * FROM Tickets ORDER BY TicketID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string lastTicketID = "0";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                lastTicketID = dbState["stringTicketID"] as string;
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();
            return lastTicketID;
        }

        public static async Task getObservablesAsync(Ticket ticket, ObservableCollection<Tax> TaxList, ObservableCollection<Item> Items, ObservableCollection<Notes> notes)
        {

            //seperate each item 
            string[] itemID = ticket.items.Split(':');
            string[] itemPrices = ticket.prices.Split(':');
            string[] itemQuan = ticket.quantities.Split(':');
            string[] itemDiscount = ticket.discounts.Split(':');
            string[] itemModID = ticket.modifiers.Split(':');

            string[] tax = ticket.taxID.Split(':');// between each tax key there is a ; between each item set is :
            string[] ticketNotes = ticket.notes.Split(';');

            //rebuilding notes
            foreach (string note1 in ticketNotes)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Notes notes2 = new Notes { note = note1 };
                    notes.Add(notes2);
                });
            }
            //removes the last blank
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                notes.RemoveAt(notes.Count - 1);

            });

            //getting item name
            List<string> itemNames = new List<string>();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            foreach (string id in itemID)
            {
                string sSQL = @"SELECT [name] FROM Items WHERE stringItemID = '" + id + "'";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                while (dbState.Step() == SQLiteResult.ROW)
                {

                    string sName = dbState["name"] as string;
                    itemNames.Add(sName);
                    break;
                }
                dbState.Dispose();
            }


            //Load into observable collection and get tax for each item
            for (int i = 0; i != itemNames.Count; i++)
            {
                decimal taxAmount = 0;

                string[] taxItem = tax[i].Split(';');// incase there is more than 1 tax type on one item
                for (int j = 0; j != taxItem.Length; j++)//Generate tax amount
                {

                    foreach (Tax taxObject in TaxList)
                    {
                        if (taxItem[j] == taxObject.taxKey)
                        {
                            decimal rate = Convert.ToDecimal(taxObject.rate);
                            double rateAT = Convert.ToDouble(taxObject.taxAtAmount);
                            if (Convert.ToDouble(itemPrices[i]) >= rateAT)
                            {
                                taxAmount = taxAmount + Convert.ToDecimal(taxObject.rate) * Convert.ToDecimal(itemPrices[i]) / 100;
                            }

                        }

                    }//end foreach
                }//end foreach


                taxAmount = taxAmount * Convert.ToDecimal(itemQuan[i]);
                if (itemPrices[i].Substring(0, 1) == ".")//add 0 to the front of tax
                {
                    itemPrices[i] = "0" + itemPrices[i];
                }
                try
                {
                    if (itemDiscount[i].Substring(0, 1) == ".")//add 0 to the front of discount
                    {
                        itemDiscount[i] = "0" + itemDiscount[i];
                    }
                }
                catch { }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Items.Add(new Item { name = itemNames[i], itemID = itemID[i], minQuantity = itemQuan[i], price = itemPrices[i], discount = itemDiscount[i], taxAmount = Convert.ToString(taxAmount), taxID = tax[i], modID = itemModID[i] });
                });
            }//end for

            //get modifiers foreach item
            foreach (Item item in Items)
            {
                List<string> modNames = new List<string>();
                List<string> modPrice = new List<string>();
                if (item.modID != "")
                {
                    string[] individualModID = item.modID.Split(';');
                    foreach (string modID in individualModID)
                    {
                        string sSQL = @"SELECT * FROM Modifiers WHERE stringModID = '" + modID + "'";
                        ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                        while (dbState.Step() == SQLiteResult.ROW)
                        {
                            string sName = dbState["name"] as string;
                            string sValue = dbState["value"] as string;
                            string sValueType = dbState["valueType"] as string;
                            //get the price change
                            string changeAmount = Modifier.getModChangeAmount(sValueType, sValue, Convert.ToInt32(item.minQuantity), item.price);

                            modPrice.Add(changeAmount);
                            modNames.Add(sName);
                            break;



                        }
                        dbState.Dispose();
                    }

                }
                item.modPrice = modPrice;
                item.modName = modNames;
            }
            dbConnection.Dispose();

        }
    }


    public class ticketManager
    {
        public static ObservableCollection<Ticket> GetTicket()
        {
            var tickets = new ObservableCollection<Ticket>();
            // items.Add(new Item { name = "t-shirt", category = "Shirts", price="9.99", minQuantity ="10", TaxID1= "1"});
            return tickets;
        }
    }
}
