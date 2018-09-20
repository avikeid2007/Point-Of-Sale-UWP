using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    
        public class Notes
        {
            public string note { get; set; }
        }

        public class noteManager
        {
            public static ObservableCollection<Notes> GetNote()
            {
                var note = new ObservableCollection<Notes>();
                // employees.Add(new Employee { first = "John", last = "Doe", fullname="John Doe", passcode = 123, canDoReturns = true, canDoReturnsWOTicket= true, isAdmin=false });

                return note;
            }
        }
    
}
