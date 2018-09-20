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
using Windows.Security.Credentials;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmployeePage : Page
    {
        public ObservableCollection<Employee> Employees;
        public string employeeSelectedID;
        public string lastEmployeeID;

        public EmployeePage()
        {
            Employees = employeeManager.GetEmployee();
            this.InitializeComponent();
        }



        private void addEmploy_Click(object sender, RoutedEventArgs e)//add employee is clicked
        {
            
            
            firstName.Text = "";
            lastName.Text = "";
            passcodeBox.Password = "";
            Popup.IsOpen = true;
            

        }

        private void addEmployeeToList_Click(object sender, RoutedEventArgs e)//Popup add Employee button is pressed
        {
            Popup.IsOpen = false;//closes popup

            string lastNameVar = lastName.Text.Replace("'", "''");
            string firstNameVar = firstName.Text.Replace("'", "''");
            string employID = Convert.ToString(Convert.ToDouble(lastEmployeeID) + 1);
            lastEmployeeID = employID; //increasing the last employee ID
            Employees.Add(new Employee { first = firstName.Text, last = lastName.Text, fullname = firstName.Text + " " + lastName.Text, canDoReturns = true, canDoReturnsWOTicket = true, isAdmin = false, employeeID = employID });


            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Add(new Windows.Security.Credentials.PasswordCredential("POS", employID, passcodeBox.Password));

            string sSQL = @"INSERT INTO [EmployeeList] 
([stringEmployeeID],[first],[last]) 
VALUES 
('" + employID + "','" + firstNameVar + "','" + lastNameVar + "');";

            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            dbConnection.Prepare(sSQL).Step();
            dbConnection.Dispose();

            


        }

        private async void removeButton_Click(object sender, RoutedEventArgs e)//remove employee
        {

            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete employee permanently?",
                Content = "If you delete this employee, you won't be able to access any of their timestamps or reports by their name. Additionally, they can no longer use their login and have access to the software's functionality. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                deleteFileDialog.RequestedTheme = ElementTheme.Light;
            }
            ContentDialogResult result = await deleteFileDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //removing the employee from the list;\
                SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
                string deleteQuery = "UPDATE EmployeeList set deleted ='1' WHERE stringEmployeeID = '" + employeeSelectedID + "'";
                dbConnection.Prepare(deleteQuery).Step();
                dbConnection.Dispose();
                Employee.refreshingEmployeeList(Employees);

                var vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> credentialsList = vault.FindAllByUserName(employeeSelectedID);
                Windows.Security.Credentials.PasswordCredential credential = credentialsList[0];
                credential.RetrievePassword();//add pass to credential
                vault.Remove(new Windows.Security.Credentials.PasswordCredential("POS", employeeSelectedID, credential.Password));

            }
            else
            {
                // The user clicked the CLoseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }


        }

        private void employeeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var employeeSelected = (Employee)e.ClickedItem;
            employeeSelectedID = employeeSelected.employeeID;
            editFirstName.Text = employeeSelected.first;
            editLastName.Text = employeeSelected.last;

            employeeSetingsView.Visibility = Visibility.Visible;

            //setting up the visuals based on the selected employee's data
            canDoRefundSwitch.IsOn = employeeSelected.canDoReturns;
            canDoRefundWOTicketSwitch.IsOn = employeeSelected.canDoReturnsWOTicket;
        }//click any employee for slideout



        public string refreshingLastEmployeeID()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Employees.db");
            string sSQL = @"SELECT * FROM EmployeeList ORDER BY employeeID DESC LIMIT 1;";
            ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
            string lastEmployeeID = "0";
            while (dbState.Step() == SQLiteResult.ROW)
            {
                lastEmployeeID = dbState["stringEmployeeID"] as string;
                break;
            }
            dbState.Dispose();
            dbConnection.Dispose();
            return lastEmployeeID;
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["theme"].ToString() == "light")
            {
                Brush brush2 = new SolidColorBrush(Colors.LightGray);

                brush2.Opacity = 0.65;
                addEmployColor.Background = brush2;
                employeeListColor.Background = brush2;
            }
            Employee.refreshingEmployeeList(Employees);

            lastEmployeeID = refreshingLastEmployeeID();
        }

        private void isAdminSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            canCreateTciket.IsOn = true;
            canCreateCustItem.IsOn = true;
            overMin.IsOn = true;
            adjustPrice.IsOn = true;
            canLoopUpTicket.IsOn = true;
            canDoRefundSwitch.IsOn = true;
            openTill.IsOn = true;
            tillUser.IsOn = true;
            canDoRefundWOTicketSwitch.IsOn = true;
            addOrDrop.IsOn = true;
            viewOwnTime.IsOn = true;
            accessReports.IsOn = true;
            createCust.IsOn = true;
            editCust.IsOn = true;
            histCust.IsOn = true;
        }

        private async void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (changePasswordBox.Password != "") {
                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.Add(new Windows.Security.Credentials.PasswordCredential("POS", employeeSelectedID, changePasswordBox.Password));
                changePasswordBox.Password = "";
            }

            ContentDialog saveDialog = new ContentDialog
            {
                Title = "Changes Saved",
                Content = "The changes made to this employee are saved.",
                CloseButtonText = "Colse"
            };
            ContentDialogResult result = await saveDialog.ShowAsync();



        }
    }
}
