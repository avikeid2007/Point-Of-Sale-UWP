using POS.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinancialPage : Page
    {

        public ObservableCollection<Tax> TaxRates;
        public ObservableCollection<Till> Tills;
        Tax selectedTax;
        Till selectedTill;

        public string lastTaxID;
        public FinancialPage()
        {
            this.InitializeComponent();
            TaxRates = taxManager.GetTax();
            Tills = tillManager.GetTill();
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            //Till.refreshingTills(Tills);
            Tax.refreshingTaxRatesView(TaxRates);
            Till.refreshingTills(Tills);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try
            {
                if (localSettings.Values["acceptCash"].ToString() == "1")
                {
                    cash.IsChecked = true;
                }
                else
                {
                    cash.IsChecked = false;
                }
                if (localSettings.Values["acceptCard"].ToString() == "1")
                {
                    card.IsChecked = true;
                }
                else
                {
                    card.IsChecked = false;
                }
                if (localSettings.Values["acceptCheck"].ToString() == "1")
                {
                    check.IsChecked = true;
                }
                else
                {
                    check.IsChecked = false;
                }
            }
            catch { }
            try
            {
                if (localSettings.Values["taxBeforeDiscount"].ToString() == "true")
                {
                    taxBeforeAfter.IsOn = false;
                }
                else
                {
                    taxBeforeAfter.IsOn = true;
                }
            }
            catch { }

           
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush2 = new SolidColorBrush(Colors.LightGray);
                brush2.Opacity = 0.6;

                addTaxColor.Background = brush2;
                editTaxColor.Background = brush2;
                tillSetColor.Background = brush2;
                editTillColor.Background = brush2;
            }
        }

            /****************Till Settings***************/

        private void addTill_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TillSetPopup.IsOpen = true;
        }

        private void acceptTillAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string name = tillName.Text;
            string amount = tillAmount.Text;
            

            SQLiteConnection dbConnection = new SQLiteConnection("Tills.db");
            string sSQL = @"INSERT INTO [Tills] 
([name],[amount]) 
VALUES 
('" + name + "','" + amount + "');";
            dbConnection.Prepare(sSQL).Step();


            sSQL = @"SELECT [tillID] FROM Tills ORDER BY tillId DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                var tillId = (long)dbState["tillID"];
                Tills.Add(new Till { tillID = tillId.ToString(), name = name, amount = amount });
                break;
            }


            dbState.Dispose();
            dbConnection.Dispose();
            

            TillSetPopup.IsOpen = false;

        }

        private void cash_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (cash.IsChecked == true)
            {
                localSettings.Values["acceptCash"] = "1";
            }
            else
            {
                localSettings.Values["acceptCash"] = "0";
            }
        }
        private void card_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (card.IsChecked == true)
            {
                localSettings.Values["acceptCard"] = "1";
            }
            else
            {
                localSettings.Values["acceptCard"] = "0";
            }
        }

        private void check_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (check.IsChecked == true)
            {
                localSettings.Values["acceptCheck"] = "1";
            }
            else
            {
                localSettings.Values["acceptCheck"] = "0";
            }
        }

        private void saveEditTill_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Tills.db");
            if (tillSetAmount.IsChecked == true)//ser amount
            {
                string amount = editTillAmount.Text;
                string sSQL = @"INSERT INTO [TillChanges] 
([tillID],[type],[amount]) 
VALUES 
('" + selectedTill.tillID + "','" + "1" + "','" + amount + "');";

                dbConnection.Prepare(sSQL).Step();
                string update = "UPDATE Tills set amount='" + amount + "' WHERE tillID = '" + selectedTill.tillID + "'";
                dbConnection.Prepare(update).Step();
            }
            else if (tillAddAmount.IsChecked == true)//add amount
            {
                double amount =  Convert.ToDouble(editTillAmount.Text) + Convert.ToDouble(selectedTill.amount);
                string sSQL = @"INSERT INTO [TillChanges] 
([tillID],[type],[amount]) 
VALUES 
('" + selectedTill.tillID + "','" + "2" + "','" + editTillAmount.Text + "');";

                dbConnection.Prepare(sSQL).Step();
                string update = "UPDATE Tills set amount='" + amount + "' WHERE tillID = '" + selectedTill.tillID + "'";
                dbConnection.Prepare(update).Step();
            }
            else if(tillDropAmount.IsChecked == true)//drop amount
            {
                double amount = Convert.ToDouble(selectedTill.amount) - Convert.ToDouble(editTillAmount.Text);
                string sSQL = @"INSERT INTO [TillChanges] 
([tillID],[type],[amount]) 
VALUES 
('" + selectedTill.tillID + "','" + "3" + "','" + editTillAmount.Text + "');";

                dbConnection.Prepare(sSQL).Step();
                string update = "UPDATE Tills set amount='" + amount + "' WHERE tillID = '" + selectedTill.tillID + "'";
                dbConnection.Prepare(update).Step();
            }
            string changeName = "UPDATE Tills set name='" + editTillName.Text + "' WHERE tillID = '" + selectedTill.tillID + "'";
            dbConnection.Prepare(changeName).Step();

            dbConnection.Dispose();
            Till.refreshingTills(Tills);
            editTillPopup.IsOpen = false;
        }

        private void deleteTill_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Tills.db");

            string update = "UPDATE Tills set deleted='" + true + "' WHERE tillID = '" + selectedTill.tillID + "'";
            dbConnection.Prepare(update).Step();
            editTillPopup.IsOpen = false;
            Tills.Remove(selectedTill);
            dbConnection.Dispose();
        }

        private void tillGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectedTill = (Till)e.ClickedItem;
            editTillName.Text = selectedTill.name;
            editTillPopup.IsOpen = true;
        }

        private void tillSetAmount_Checked(object sender, RoutedEventArgs e)
        {
            editTillAmount.PlaceholderText = "Enter New Drawer Amount";
        }

        private void tillAddAmount_Checked(object sender, RoutedEventArgs e)
        {
            editTillAmount.PlaceholderText = "Enter Add Amount";
        }

        private void tillDropAmount_Checked(object sender, RoutedEventArgs e)
        {
            editTillAmount.PlaceholderText = "Enter Drop Amount";
        }
        /****************Tax Settings***************/
        private void addTaxRate_Tapped(object sender, TappedRoutedEventArgs e)//add taxrate button in slideout
        {
            //Clear boxes before open
            addTaxName.Text = "";
            addRate.Text = "";
            addChargeAtPricePoint.IsOn = false;
            addTaxAtPriceValue.Text = "";


            TaxPopup.IsOpen = true;
        }

        private void addTaxToList_Tapped(object sender, TappedRoutedEventArgs e)//adds Tax info to database
        {

            string addTaxNameVar = addTaxName.Text;
            string addRateVar = addRate.Text;
            string addTaxAtPriceValueVar = addTaxAtPriceValue.Text;
            if (addTaxAtPriceValue.Text == null || addChargeAtPricePoint.IsOn == false)
            {
                addTaxAtPriceValueVar = "0";
            }



            string taxID = "0";
            if (lastTaxID != "")
            {
                taxID = Convert.ToString(Convert.ToDouble(lastTaxID) + 1);
            }

            lastTaxID = taxID;

            string sSQL = @"INSERT INTO [TaxRate] 
([stringTaxID],[name],[rate],[valueToTaxAt]) 
VALUES 
('" + taxID + "','" + addTaxNameVar + "','" + addRateVar + "','" + addTaxAtPriceValueVar + "');";

            SQLiteConnection dbConnection = new SQLiteConnection("TaxSettings.db");
            dbConnection.Prepare(sSQL).Step();

            dbConnection.Dispose();
            Tax.refreshingTaxRatesView(TaxRates);
            TaxPopup.IsOpen = false;

        }

        private void editTax_Tapped(object sender, TappedRoutedEventArgs e)//opens edit tax popup
        {
            //var employeeSelected = (Employee)e.ClickedItem;
            Button button = sender as Button;
            selectedTax = button.DataContext as Tax;
            editTaxName.Text = selectedTax.name;
            editRate.Text = selectedTax.rate;
            editTaxAtPriceValue.Text = selectedTax.taxAtAmount;
            if (Convert.ToInt64(selectedTax.taxAtAmount) > 0)
            {
                editChargeAtPricePoint.IsOn = true;
            }
            else
            {
                editChargeAtPricePoint.IsOn = false;
            }

            EditTaxPopup.IsOpen = true;
        }

        private void deleteTax_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Button button = sender as Button;
            Tax selectedTax = button.DataContext as Tax;
            string selectedTaxID = selectedTax.taxKey;

            SQLiteConnection dbConnection = new SQLiteConnection("TaxSettings.db");
            string update = "UPDATE TaxRate set deleted='" + "1" + "' WHERE stringTaxID = '" + selectedTaxID + "'";
            dbConnection.Prepare(update).Step();

            dbConnection.Dispose();
            Tax.refreshingTaxRatesView(TaxRates);
        }

        private void taxRateSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TaxListExpandDataTemplate"];
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["TaxListDataTemplate"];
            }
        }

        private void saveChangeToList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string addTaxNameVar = editTaxName.Text;
            string addRateVar = editRate.Text;
            string addTaxAtPriceValueVar = editTaxAtPriceValue.Text;
            string selectedTaxID = selectedTax.taxKey;
            if (editTaxAtPriceValue.Text == null || editChargeAtPricePoint.IsOn == false)
            {
                addTaxAtPriceValueVar = "0";
            }

            string taxID = "0";
            if (lastTaxID != "")
            {
                taxID = Convert.ToString(Convert.ToDouble(lastTaxID) + 1);
            }

            lastTaxID = taxID;

            SQLiteConnection dbConnection = new SQLiteConnection("TaxSettings.db");
            string update = "UPDATE TaxRate set deleted='" + "1" + "' WHERE stringTaxID = '" + selectedTaxID + "'";
            dbConnection.Prepare(update).Step();

            string sSQL = @"INSERT INTO [TaxRate] 
([stringTaxID],[name],[rate],[valueToTaxAt]) 
VALUES 
('" + taxID + "','" + addTaxNameVar + "','" + addRateVar + "','" + addTaxAtPriceValueVar + "');";

            dbConnection.Prepare(sSQL).Step();
            dbConnection.Dispose();

            EditTaxPopup.IsOpen = false;
            Tax.refreshingTaxRatesView(TaxRates);
        }

        private void insightDefaultTax_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void taxBeforeAfter_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (taxBeforeAfter.IsOn == true)
            {
                localSettings.Values["taxBeforeDiscount"] = "false";
            }
            else
            {
                localSettings.Values["taxBeforeDiscount"] = "true";
            }
        }
    }
}
