using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using POS.Models;
using SQLitePCL;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.UI.Controls;
//using System.Drawing;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsAddEdit : Page
    {
        public ObservableCollection<Item> Items;
        public ObservableCollection<Category> Categories;
        public ObservableCollection<Category> Categories2;
        public ObservableCollection<Tax> TaxRates;
        public ObservableCollection<Modifier> ModifierGroups;
        public ObservableCollection<Modifier> ModifierSelectedGroup;
        public string selectedCat = "-1";
        public Category rightClickCat;

        public string selectedModGroup;
        public string selectedModItem;
        public string selectedItemID;

        public string nextMod;
        public string nextModGroup;
        public string nextItem;
        public string nextCat;

        //private string[] modGroups = new string[5];
        List<string> modGroups = new List<string>();
        IAsyncOperation<DataPackageOperation> _dragOperation;


        public ItemsAddEdit()
        {
            this.InitializeComponent();
            

            Items = itemManager.GetItem();
            Categories = categoryManager.GetCategory();
            Categories2 = categoryManager.GetCategory();
            TaxRates = taxManager.GetTax();
            ModifierGroups = modifierManager.GetModifier();
            ModifierSelectedGroup = modifierManager.GetModifier();
            
        }
        

        private void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush2 = new SolidColorBrush(Colors.White);

                brush2.Opacity = 0.5;
                catHeaderColor.Background = brush2;
                itemHederColor.Background = brush2;
                modHeaderColor.Background = brush2;
                Brush brush = new SolidColorBrush(Colors.White);
                brush.Opacity = 0.2;
                modListColor.Background = brush;

                Brush brush3 = new SolidColorBrush(Colors.LightGray);
                brush3.Opacity = 0.6;
                addCatColor.Background = brush3;
                editCatColor.Background = brush3;
                addModGroupColor.Background = brush3;
                addItemColor.Background = brush3;
                editItemColor.Background = brush3;
                addModColor.Background = brush3;
                editModColor.Background = brush3;
            }
            else
            {

            }
                refreshingCategories();
            Item.refreshingItems(selectedCat, Items);
            refreshingModGroup();
            Tax.refreshingTaxRatesView(TaxRates);
            nextMod = refreshingModID();
            nextModGroup = refreshingModGroupID();
            nextItem = refreshingItemID();
            nextCat = refreshingCatID();

        }

        /****************Category Settings***************/
        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            categoryName.Text = "";
            addCategoryPopUp.IsOpen = true;
        }

        private void saveAddCat_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string categoryNameBox = categoryName.Text;
            string sSQL = @"INSERT INTO [Categories] 
([name],[stringCatID]) 
VALUES 
('" + categoryNameBox + "','" + nextCat + "');";

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            dbConnection.Prepare(sSQL).Step();
            dbConnection.Dispose();
            nextCat += 1;

            refreshingCategories();
            addCategoryPopUp.IsOpen = false;

        }

        public void refreshingCategories()
        {
            Categories.Clear();
            Categories2.Clear();//This is used to add no group to the drop down
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Categories WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT [name],[stringCatID],[catPosition] FROM Categories";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sID = dbState["stringCatID"] as string;
                string sName = dbState["name"] as string;
                string sPosition = dbState["catPosition"] as string;

                //Load into observable collection
                Categories.Add(new Category { categoryID = sID, name = sName, position = sPosition });
                Categories2.Add(new Category { categoryID = sID, name = sName, position = sPosition });
            }
            Categories2.Add(new Category { categoryID = "-1", name = "No Group" });
            dbConnection.Dispose();
            dbState.Dispose();
        }
        public string refreshingCatID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string sSQL = @"SELECT * FROM Categories ORDER BY catID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string catID = "1";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                catID = Convert.ToString(Convert.ToDouble(dbState["stringCatID"] as string) + 1);
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();

            return catID;
        }

        private void categoryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            catBack.Visibility = Visibility.Visible;
            var categorySelected = (Category)e.ClickedItem;
            selectedCat = categorySelected.categoryID;
            Item.refreshingItems(selectedCat, Items);
        }

        private void catBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            catBack.Visibility = Visibility.Collapsed;
            categoryGridView.SelectedIndex = -1;
            Item.refreshingItems("-1", Items);
        }

        private void catGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            var s = (FrameworkElement)sender;
            var d = s.DataContext;
            var cat = d as Category;
            rightClickCat = cat;

        }

        private void editCatMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            editCategoryName.Text = rightClickCat.name;
            editCategoryPopUp.IsOpen = true;
        }

        private void editCatSave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string update = "UPDATE Categories set name='" + editCategoryName.Text + "' WHERE catID = '" + rightClickCat.categoryID + "'";
            dbConnection.Prepare(update).Step();
            dbConnection.Dispose();
            editCategoryPopUp.IsOpen = false;
            refreshingCategories();
        }

        private void editCatCancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            editCategoryPopUp.IsOpen = false;
        }

        private async void deleteCatMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete Category?",
                Content = "If you delete this category, all the items will be placed in the no group catgory. Do you wish to continue?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            ContentDialogResult result = await deleteFileDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
                string deleteQuery = "DELETE FROM Categories WHERE catID ='" + rightClickCat.categoryID + "'";
                dbConnection.Prepare(deleteQuery).Step();

                string update = "UPDATE Items set category='" + "-1" + "' WHERE category = '" + rightClickCat.categoryID + "'";
                dbConnection.Prepare(update).Step();
                dbConnection.Dispose();
                refreshingModGroup();

            }
            else
            {
                // Do nothing.
            }
        }

        /****************Item Settings***************/

        private void addItem_Click(object sender, RoutedEventArgs e)
        {
            itemName.Text = "";
            price.Text = "";
            itemDescription.Text = "";
            minQuantity.Text = "";
            itemCost.Text = "";
            if (categoryGridView.SelectedIndex == -1)
            {
                addItemCat.SelectedIndex = Categories2.Count() - 1;
            }
            else
            {
                addItemCat.SelectedIndex = categoryGridView.SelectedIndex;
            }
            addModGridView.SelectedIndex = -1;
            addTaxGridView.SelectedIndex = -1;
            addItemPopUp.IsOpen = true;
        }

        private void addItemPopUpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            string allmods = "";

            //creating modifier ID list
            foreach (var griditem in addModGridView.SelectedItems)
            {
                 var modGridView = (Modifier)griditem;
                 allmods = allmods + modGridView.modID + ";";
            }


            string allTax = "";
            //create tax ID
            foreach (var griditem in addTaxGridView.SelectedItems)
            {
                var modGridView = (Tax)griditem;
                allTax = allTax + modGridView.taxKey + ";";
            }


            var itemCat = (Category)addItemCat.SelectedItem;
            //SQLite stuff

            string itemNameBox = itemName.Text;
            string itemPrice = price.Text;
            if(itemPrice == "")
            {
                itemPrice = "0.00";
            }
            string description = itemDescription.Text;
            string position = itemCat.categoryID;
            string minimumQuan = minQuantity.Text;
            string cost = itemCost.Text;
            string sSQL2 = @"INSERT INTO [Items] 
([stringItemID],[name],[price],[modGroupID],[category],[minQuantity],[taxID],[description],[cost]) 
VALUES 
('" + nextItem + "','" + itemNameBox + "','" + itemPrice + "','"+ allmods + "','" + position + "','" + minimumQuan + "','" + allTax + "','" + description + "','" + cost + "');";

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            dbConnection.Prepare(sSQL2).Step();//put it back into table
            dbConnection.Dispose();
            Item.refreshingItems(selectedCat, Items);
            //Items.Add(new Item { itemID = nextItem, price = itemPrice, name = itemNameBox, category = position, minQuantity = minimumQuan, taxID = allTax, modID = allmods });
            //RefreshingItems();
            nextItem = Convert.ToString(Convert.ToDouble(nextItem) +1);

            addItemPopUp.IsOpen = false;// close pupup
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = (Item)e.ClickedItem;
            selectedItemID = selectedItem.itemID;
            editModGridView.SelectedIndex = -1;
            //update cat combo box
            editItemCat.SelectedIndex = Categories.Count() ;
            int i = 0;
            foreach (Category cat in Categories)
            {
                if(cat.categoryID == selectedItem.category)
                {
                    editItemCat.SelectedIndex = i;
                }
                i += 1;
            }

            //Selecting tax items
            editTaxGridView.SelectedIndex = -1;
            string[] taxID;
            taxID = selectedItem.taxID.Split(';');
            foreach (string taxGroupID in taxID)
            {
                foreach (Tax taxINCollect in TaxRates)
                {
                    if (taxGroupID == taxINCollect.taxKey)
                    {
                        //editModGridView.SelectedIndex = editModGridView.Items.IndexOf(modGroup);
                        editTaxGridView.SelectedItems.Add(taxINCollect);
                    }
                }
            }



            string[] modCode;
            modCode = selectedItem.modID.Split(';');

            foreach (string modGroupID in modCode)
            {
                foreach (Modifier modGroup in ModifierGroups)
                {
                    if (modGroupID == modGroup.modID)
                    {
                        //editModGridView.SelectedIndex = editModGridView.Items.IndexOf(modGroup);
                        editModGridView.SelectedItems.Add(modGroup);
                    }

                }


            }
            try
            {
                editItemName.Text = selectedItem.name;
            }
            catch { editMinQuantity.Text = ""; }
            try
            {
                editMinQuantity.Text = selectedItem.minQuantity;
            }
            catch { editMinQuantity.Text = ""; }
            try
            {
                editPrice.Text = selectedItem.price;
                    }
                    catch { editPrice.Text = ""; }
                    try
            {
                editItemCost.Text = selectedItem.cost;
            }
            catch
            {
                editItemCost.Text = "";
            }

            try
            {
                editItemDescription.Text = selectedItem.description;
            }
            catch { editItemDescription.Text = ""; }
            
            

            editItemPopUp.IsOpen = true;
        }

        public string refreshingItemID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string sSQL = @"SELECT * FROM Items ORDER BY itemID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string itemID = "1";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                itemID = Convert.ToString(Convert.ToDouble(dbState["stringItemID"] as string) + 1);
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();
            
            return itemID;
        }

        private void deleteItemPopUpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //removing the Item from the list
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string update = "UPDATE Items set deleted='"+"1"+ "' WHERE stringItemID = '" + selectedItemID + "'";
            dbConnection.Prepare(update).Step();
            dbConnection.Dispose();
       
            editItemPopUp.IsOpen = false;

            Item.refreshingItems(selectedCat, Items);
        }

        private void editItemPopUpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            string allTax = "";
            //create tax ID
            foreach (var griditem in editTaxGridView.SelectedItems)
            {
                var modGridView = (Tax)griditem;
                allTax = allTax + modGridView.taxKey + ";";
            }

            string allmods = null;

            //creating modifier ID list
            foreach (var griditem in editModGridView.SelectedItems)
            {
                var editModGridView = (Modifier)griditem;
                allmods = allmods + editModGridView.modID + ";";
            }

            var itemCat = (Category)editItemCat.SelectedItem;

            string itemNameBox = editItemName.Text;
            string itemPrice = editPrice.Text;
            if (itemPrice == "")
            {
                itemPrice = "0.00";
            }
            string description = editItemDescription.Text;
            string position = itemCat.categoryID;
            string minimumQuan = editMinQuantity.Text;
            string cost = editItemCost.Text;
            editItemPopUp.IsOpen = false;

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string update = "UPDATE Items set name='" + itemNameBox + "', price='"+ itemPrice +"', category = '"+position+"', minQuantity = '"+ minimumQuan+"', description = '"+ description + "', cost = '"+ cost +"', modGroupID = '"+ allmods + "', taxID = '" + allTax + "'  WHERE stringItemID = '" + selectedItemID + "'";
            dbConnection.Prepare(update).Step();
            dbConnection.Dispose();

            Item.refreshingItems(selectedCat, Items);
        }



        /****************Modifier Settings***************/
        private void addModifiers_Click(object sender, RoutedEventArgs e)
        {
            adjustPrice.IsOn = false;
            changeValue.IsReadOnly = true;
            addModifiersPopUp.IsOpen = true;
        }

        private void changeValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            string x = changeValue.Text;
            string xnum = changeValue.Text;
            int position = changeValue.SelectionStart;
            while (xnum != "")
            {
                if (xnum.Length == 1)//if there is only 1 char in x
                {
                    if (!xnum.Any(char.IsDigit) && xnum != "." && xnum != "-")//is it not a digit or a decimal
                    {
                        x = x.Replace(xnum, "");//replace it with nothing

                        changeValue.Text = x;//set the output
                        changeValue.SelectionStart = position - 1;//set the position
                        break;
                    }
                    xnum = "";
                    break;
                }
                if (!xnum.Substring(0, 1).Any(char.IsDigit) && xnum.Substring(0, 1) != "." && xnum.Substring(0, 1) != "-")// if its not a digit or a decimal 
                {
                    x = x.Replace(xnum.Substring(0, 1), "");//replace the bad char with nothing
                    changeValue.Text = x;
                    changeValue.SelectionStart = position - 1;
                }
                if (xnum.Length != 1)
                {
                    xnum = xnum.Substring(1, xnum.Length - 1);//removes first char to check the next
                }

            }//end while
        }

        private void addModinPOP_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string valueType = Convert.ToString(changeValueComboBox.SelectedIndex);
            if(adjustPrice.IsOn == false)
            {
                valueType = "4";  
            }
            /*** 
             * 4 means no change in price
             * 3 means decrease in percent
             * 2 means decrease in amount
             * 1 means increase in percent
             * 0 means increase in amount
             * ***/

            

            string modname = modName.Text;
            //string modgroup = modGroup.Text;
            string changevalue = changeValue.Text;

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string sSQL = @"INSERT INTO [Modifiers] 
([name],[stringModID],[modGroup],[value],[valueType]) 
VALUES 
('" + modname + "','" + nextMod + "','" + selectedModGroup + "','" + changevalue + "','" + valueType + "');";

            
            dbConnection.Prepare(sSQL).Step();

           refreshingModGroup();
            addModifiersPopUp.IsOpen = false;

            nextMod = Convert.ToString(Convert.ToDouble(nextMod) + 1); ;
            dbConnection.Dispose();
            

            //ModifierGroups.Add(new Modifier { modID = "", name = modName.Text, modGroup = modGroup.Text, changeValue = changeValue.Text, changeType= valueType});
        }

        public void refreshingModGroup()
        {
            ModifierGroups.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Modifiers WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM ModGroups";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);

            //Button add = new Button();
            //List<Button> butt = new List<Button>();
            //butt.Add()
            for (int i =0;  dbState.Step() == SQLiteResult.ROW; i++)
            {
               
                string sID = dbState["stringModGroupID"] as string;
                string sName = dbState["name"] as string;
                string sDeleted = dbState["deleted"] as string;

                if (sDeleted != "1")
                {
                    //Expander test = new Expander();
                    //test.Header = sName;
                    /*
                    TextBlock group = new TextBlock();
                    group.Text = sName;
                    group.FontSize = 18;
                    group.VerticalAlignment = VerticalAlignment.Center;

                    Grid expander = new Grid();
                    RowDefinition top = new RowDefinition();
                    
                    RowDefinition expanded = new RowDefinition();
                    expanded.Height = new GridLength(1, GridUnitType.Star);
                    top.Height = new GridLength(40);
                    expander.RowDefinitions.Add(top);
                    expander.RowDefinitions.Add(expanded);
                    
                    Button add = new Button();
                    //butt.Add(add);
                    add.Content = "";
                    add.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    add.Tapped += addMod_Tapped;
                    //buttDict.Add(sID, );

                    var color = (Windows.UI.Color)this.Resources["SystemAccentColor"];
                    Border border = new Border();
                    border.Background = new SolidColorBrush(color);
                    expander.Children.Add(border);



                    StackPanel header = new StackPanel();
                    header.Orientation = Orientation.Horizontal;
                    header.Children.Add(group);
                    header.Children.Add(add);
                    expander.Children.Add(header);

                    TextBlock test = new TextBlock();
                    test.Text = "hello";

                    expander.Children.Add(test);
                    Grid.SetRow(test, 1);



                    //expander.Width = new GridLength(1, GridUnitType.Pixel);



                    ModGroupsStack.Children.Add(expander);
                    */
                    string sSQL2 = @"SELECT * FROM Modifiers WHERE modGroup ='"+ sID +"'";
                    ISQLiteStatement dbState2 = dbConnection.Prepare(sSQL2);

                    List<string> mods = new List<string>();
                    string ssName = null;
                    while (dbState2.Step() == SQLiteResult.ROW)
                    {
                        ssName = dbState2["name"] as string;
                        string ssDeleted = dbState2["deleted"] as string;
                        if(ssDeleted != "1")
                        {
                            mods.Add(ssName);
                        }

                    }
                    dbState2.Dispose();
                    ModifierGroups.Add(new Modifier { modID = sID, name = sName, items = mods, modgroupInt = Convert.ToInt32(sID) });



                }
            }
            dbState.Dispose();
            dbConnection.Dispose();
            

        }

        public void refreshingModItems()
        {
            ModifierGroups.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Modifiers WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM Modifiers";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            for (int i = 0; dbState.Step() == SQLiteResult.ROW; i++)
            {
                bool match = false;
                string sID = dbState["stringModID"] as string;
                string sName = dbState["name"] as string;
                string sModGroup = dbState["modGroup"] as string;
                string sChangeType = dbState["value"] as string;
                string sChangeValue = dbState["valueType"] as string;
                string sDeleted = dbState["deleted"] as string;

                if (sDeleted != "1")
                {
                    if (i == 0)//if its the first create a group
                    {
                        ModifierGroups.Add(new Modifier { modID = sID, name = sName, modGroup = sModGroup, changeType = sChangeType, changeValue = sChangeValue });

                    }
                    else //for the rest
                    {
                        foreach (Modifier mod in ModifierGroups)
                        {
                            if (sModGroup == mod.modGroup)
                            {
                                match = true;
                            }

                        }

                        if (match == false)//if the group name isnt there add it to the collection
                        {
                            //Load into observable collection 
                            ModifierGroups.Add(new Modifier { modID = sID, name = sName, modGroup = sModGroup, changeType = sChangeType, changeValue = sChangeValue });

                        }
                    }
                }
            }
            dbState.Dispose();
            dbConnection.Dispose();
            
            modGroups.Clear(); //clears the list
            foreach (Modifier mod in ModifierGroups)
            {

                modGroups.Add(mod.modGroup); //fills the list with obserCollec
            }
        }

        public string refreshingModID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            //fix this
            string sSQL = "SELECT * FROM Modifiers ORDER BY modID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string modID = "1";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                modID = Convert.ToString(Convert.ToDouble(dbState["stringModID"] as string) + 1);
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();
            return modID;
        }

        public string refreshingModGroupID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            //fix this
            string sSQL = "SELECT * FROM ModGroups ORDER BY modGroupID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string modID = "1";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                modID = Convert.ToString(Convert.ToDouble(dbState["stringModGroupID"] as string) + 1);
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();
            return modID;
        }

        public void refreshingSelectedModGroup()
        {
            ModifierSelectedGroup.Clear();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string deleteQuery = "DELETE FROM Modifiers WHERE name = '" + " " + "'";
            dbConnection.Prepare(deleteQuery).Step();

            string sSQL = @"SELECT * FROM Modifiers";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);


            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sID = dbState["stringModID"] as string;
                string sName = dbState["name"] as string;
                string sModGroup = dbState["modGroup"] as string;
                string sChangeType = dbState["value"] as string;
                string sChangeValue = dbState["valueType"] as string;


                if (selectedModGroup == sModGroup)
                {


                    //Load into observable collection
                    ModifierSelectedGroup.Add(new Modifier { modID = sID, name = sName, modGroup = sModGroup, changeType = sChangeType, changeValue = sChangeValue });

                }
                
            }
            dbState.Dispose();
            dbConnection.Dispose();
            
        }

        private void adjustPrice_Toggled(object sender, RoutedEventArgs e)
        {
            if (adjustPrice.IsOn == true)
            {
                changeValue.IsReadOnly = false;
            }
            else
            {
                changeValue.IsReadOnly = true;
                changeValue.Text = "";
            }
        }

        //Mod Group Buttons
        private void addMod_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Button name = sender as Button;
            Modifier selectModGroup = name.DataContext as Modifier;
            modGroupAddMod.Text = selectModGroup.name;
            modName.Text = "";
            changeValue.Text = "";
            selectedModGroup = selectModGroup.modID;

            adjustPrice.IsOn = false;
            changeValue.IsReadOnly = true;
            addModifiersPopUp.IsOpen = true;
        }

        private void saveModGroup_Tapped(object sender, TappedRoutedEventArgs e)
        {

            string modGroup = modGroupName.Text;
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string sSQL = @"INSERT INTO [ModGroups] 
([name],[stringModGroupID]) 
VALUES 
('" + modGroup + "','" + nextModGroup + "');";


            dbConnection.Prepare(sSQL).Step();
            dbConnection.Dispose();

            refreshingModGroup();
            addModGroupPopUp.IsOpen = false;

            nextModGroup = Convert.ToString(Convert.ToDouble(nextModGroup) + 1); ;
           


        }

        private void addModGroup_Click(object sender, RoutedEventArgs e)
        {
            modGroupName.Text = "";
            addModGroupPopUp.IsOpen = true;
        }

        private async void removeModGroup_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button name = sender as Button;
            Modifier selectModGroup = name.DataContext as Modifier;
            

            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete modifier group permanently?",
                Content = "If you delete this modifer group, you won't be able to use any of the items inside. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            ContentDialogResult result = await deleteFileDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
                string deleteQuery = "DELETE FROM ModGroups WHERE stringModGroupID ='" + selectModGroup.modID + "'";
                dbConnection.Prepare(deleteQuery).Step();

                string update = "UPDATE Modifiers set deleted='" + "1" + "' WHERE modGroup = '" + selectModGroup.modID + "'";
                dbConnection.Prepare(update).Step();
                dbConnection.Dispose();
                refreshingModGroup();

            }
            else
            {
                // Do nothing.
            }

        }

        //Edit Modifier Items
        private void modItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            var modGroupCol = sender as ListView;
            string modGroup = Convert.ToString(Grid.GetRow(modGroupCol));//Binded the row to to the group ID
            selectedModGroup = modGroup; //for delete and save

            var modItem = (string)e.ClickedItem;
            editModName.Text = modItem;
            selectedModItem = modItem; // for delete and save
            

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string sSQL2 = @"SELECT * FROM Modifiers WHERE modGroup ='" + modGroup + "' AND name = '" + modItem +"'";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL2);
            while (dbState.Step() == SQLiteResult.ROW)
            {
                string sValue = dbState["value"] as string;
                string sValueType = dbState["valueType"] as string;
                if(sValueType != "4")
                {
                    editChangeValueComboBox.SelectedIndex = Convert.ToInt16(sValueType);
                    editAdjustPrice.IsOn = true;
                    editChangeValue.Text = sValue;
                }
                else
                {
                    editAdjustPrice.IsOn = false;
                    editChangeValue.Text = "";
                }
                

            }
            dbState.Dispose();
            dbConnection.Dispose();


            editModifiersPopUp.IsOpen = true;
        }

        private void editSaveModinPOP_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string valueType = Convert.ToString(editChangeValueComboBox.SelectedIndex);
            if (editAdjustPrice.IsOn == false)
            {
                valueType = "4";
            }
            /*** 
             * 4 means no change in price
             * 3 means decrease in percent
             * 2 means decrease in amount
             * 1 means increase in percent
             * 0 means increase in amount
             * ***/

            string modname = editModName.Text;
            //string modgroup = modGroup.Text;
            string changevalue = editChangeValue.Text;

            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");

            string update = "UPDATE Modifiers set deleted='" + "1" + "' WHERE name = '" + selectedModItem + "' AND modGroup ='" + selectedModGroup + "'";
            dbConnection.Prepare(update).Step();

            string sSQL = @"INSERT INTO [Modifiers] 
([name],[stringModID],[modGroup],[value],[valueType]) 
VALUES 
('" + modname + "','" + nextMod + "','" + selectedModGroup + "','" + changevalue + "','" + valueType + "');";
            dbConnection.Prepare(sSQL).Step();


            dbConnection.Dispose();
            refreshingModGroup();
            addModifiersPopUp.IsOpen = false;

            nextMod = Convert.ToString(Convert.ToDouble(nextMod) + 1);


            editModifiersPopUp.IsOpen = false;
        }

        private void editAdjustPrice_Toggled(object sender, RoutedEventArgs e)
        {
            if (editAdjustPrice.IsOn == true)
            {
                editChangeValue.IsReadOnly = false;
            }
            else
            {
                editChangeValue.IsReadOnly = true;
                editChangeValue.Text = "";
            }
        }

        private void removeModItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            string update = "UPDATE Modifiers set deleted='" + "1" + "' WHERE modGroup ='" + selectedModGroup + "' AND name = '" + selectedModItem + "'";
            dbConnection.Prepare(update).Step();
            editModifiersPopUp.IsOpen = false;
            dbConnection.Dispose();

            refreshingModGroup();
        }

        
        //Drag and Drop
        private void catDropBorder_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void catDropBorder_Drop(object sender, DragEventArgs e)
        {

        }

        private void itemDragGrid_PointerMoved(object sender, PointerRoutedEventArgs e)//register dragOperation
        {
            if(e.Pointer.IsInContact && (_dragOperation == null))
            {
                //itemDragGrid.
                //_dragOperation = itemDragGrid.StartDragAsync(e.GetCurrentPoint(itemDragGrid));
                //_dragOperation.Completed = itemDragGrid;
            }
        }

        private void itemDragGrid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {

        }

        
    }
}
