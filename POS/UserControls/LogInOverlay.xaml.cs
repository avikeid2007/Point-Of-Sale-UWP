using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace POS
{
    public sealed partial class LogInOverlay : UserControl
    {
        public string PageToUnlock { get; set; }

        public ObservableCollection<Employee> Employees;
        public Employee loggedInEmployee;
        public LogInOverlay()
        {
            this.InitializeComponent();
            Employees = employeeManager.GetEmployee();
        }

        private void LogInPopupButton_Click(object sender, RoutedEventArgs e)
        {

            if (employeeComboBoxIn.SelectedIndex != -1)
            {

                var selectedEmployee = (Employee)employeeComboBoxIn.SelectedItem;
                var vault = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> credentialsList = vault.FindAllByUserName(selectedEmployee.employeeID);
                Windows.Security.Credentials.PasswordCredential credential = credentialsList[0];
                credential.RetrievePassword();//add pass to credential
                if (logInPass.Password == credential.Password)
                {

                    logInFilter.Visibility = Visibility.Collapsed;
                    logInPopup.IsOpen = false;
                }//end if passcodes equal
                else
                {

                    logInStatus.Text = "Passwords are not a match.";
                    if (logInPass.Password == "")
                    {
                        logInStatus.Text = "Password cannot be blank.";
                    }

                }
            }//end if != -1


        }

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            Employee.refreshingEmployeeListAsync(Employees);
        }

        public Employee getLoggedInEmployee()
        {
                return loggedInEmployee;
        }


    }


}
