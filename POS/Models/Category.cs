using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Category
    {
        public string categoryID { get; set; }
        public string name { get; set; }
        public string position { get; set; }
    }

    public class categoryManager
    {
        public static ObservableCollection<Category> GetCategory()
        {
            var category = new ObservableCollection<Category>();
            // customers.Add(new Customer { first = "John", last = "Doe", fullname="John Doe", phoneNumber ="5555555555", phoneNumber2 = "5555555554", address= "50 Smith st. Woodville, NY 10955"});
            return category;
        }
    }


}