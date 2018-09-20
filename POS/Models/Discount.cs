using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Discount
    {
        public string discountID { get; set; }
        public string amount { get; set; }
        public string type { get; set; }
    }

    public class discountManager
    {
        public static ObservableCollection<Discount> GetDiscount()
        {
            var dis = new ObservableCollection<Discount>();
            // employees.Add(new Employee { first = "John", last = "Doe", fullname="John Doe", passcode = 123, canDoReturns = true, canDoReturnsWOTicket= true, isAdmin=false });
            return dis;
        }
    }

}
