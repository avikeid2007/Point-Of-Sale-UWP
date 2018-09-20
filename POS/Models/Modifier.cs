using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Modifier
    {
   
            public string modID { get; set; }
            public string name { get; set; }
            public string modGroup { get; set; }
            public int modgroupInt { get; set; }
            public string changeValue { get; set; }
            public string changeType { get; set; }
            public List<string> items { get; set; }

        public static string getModChangeAmount(string changeType, string changeAmount, int quan, string price)
        {

            string sValue = changeAmount;
            string sValueType = changeType;
            /*** 
            * 4 means no change in price
            * 3 means decrease in percent
            * 2 means decrease in amount
            * 1 means increase in percent
            * 0 means increase in amount
            * ***/
            switch (sValueType)
            {
                case "4":
                    changeAmount = "";
                    break;
                case "3":
                    changeAmount = "-" + Convert.ToString(((Convert.ToDecimal(Convert.ToDouble(price) * quan) - Convert.ToDecimal(sValue)) * Convert.ToDecimal(Convert.ToDouble(price) * quan)) / 100);
                    break;
                case "2":
                    changeAmount = "-" + Convert.ToDecimal(sValue) * quan;
                    break;
                case "1":
                    changeAmount = Convert.ToString(((Convert.ToDecimal(Convert.ToDouble(price) * quan) - Convert.ToDecimal(sValue)) * Convert.ToDecimal(Convert.ToDouble(price) * quan)) / 100);
                    break;
                case "0":
                    changeAmount = "" + Convert.ToDecimal(sValue) * quan;
                    break;
            }
            if (changeAmount != "")
            {


                decimal changeInt = Convert.ToDecimal(changeAmount);
                changeAmount = String.Format("{0:#.00}", changeInt);
                if (changeAmount.Substring(0, 1) == ".")
                {
                    changeAmount = "0" + changeAmount;
                }
                if (changeInt > 0)
                {
                    changeAmount = "+" + changeAmount;
                }
            }
            return changeAmount;
        }



    }

    public class modifierManager
        {
            public static ObservableCollection<Modifier> GetModifier()
            {
                var modifiers = new ObservableCollection<Modifier>();
                // modifiers.Add(new Modifier { modID = "John", name = "Doe", modGroup="John Doe", increaseAmount ="5555555555", increasePercent = "5555555554"});
                return modifiers;
            }
        }
    
}
