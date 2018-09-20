using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace POS.UserControls
{
    public sealed partial class PrintTicket : UserControl
    {
        public Ticket ticket;
        public ObservableCollection<Payments> PrintTicketPayments;
        public ObservableCollection<Item> Items;
        public ObservableCollection<Notes> notes;
        public ObservableCollection<Tax> TaxList;

        private IPrintDocumentSource printDocSource;
        public PrintTaskOptions printingOptions;
        private PrintManager printMan;
        private PrintDocument printDoc;
        public PrintPageDescription printPageDescription;

        public PrintTicket()
        {
            this.InitializeComponent();
            notes = noteManager.GetNote();
            PrintTicketPayments = payManager.GetPay();
            Items = itemManager.GetItem();
            TaxList = taxManager.GetTax();
        }





        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            
            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
        }


        public async void printTicket(Ticket printTicket)
        {
            ticket = printTicket;
            printMan.PrintTaskRequested += null;
            printDoc.Paginate += null;
            printDoc.GetPreviewPage += null;
            printDoc.AddPages += null;

            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
            printMan.PrintTaskRequested += PrintTaskRequested;
            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI

                    Items.Clear();
                    await Ticket.getObservablesAsync(ticket, TaxList, Items, notes);
                    await PrintManager.ShowPrintUIAsync();

                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }

        }


        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
            //printTask.IsPreviewEnabled = false;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        private async void Paginate(object sender, PaginateEventArgs e)
        {
            printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            printPageDescription = printingOptions.GetPageDescription(0);


            printMe.Width = printPageDescription.PageSize.Width;
            printMe.MinHeight = printPageDescription.PageSize.Height;
            printInvoiceWrapper.Width = printPageDescription.PageSize.Width;
            printInvoiceWrapper.Height = printPageDescription.PageSize.Height;
            printMe.Height = printPageDescription.PageSize.Height;

            printTicketNumber.Text = ticket.ticketID;

            if (ticket.custName == "" || ticket.custName == null)
            {
                printCustName.Visibility = Visibility.Collapsed;
            }
            else
            {
                printCustName.Visibility = Visibility.Visible;
                printCustName.Text = ticket.custName;
            }
            if (ticket.custNumber == "" || ticket.custNumber == null)
            {
                printPhone.Visibility = Visibility.Collapsed;
                if (ticket.custName == "" || ticket.custName == null)
                {
                    printCustomerTopBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                printPhone.Visibility = Visibility.Visible;
                printPhone.Text = ticket.custNumber;
            }
            
            



            if (ticket.readyDate == "" && ticket.readyTime == "")
            {
                printReadyDateTitle.Visibility = Visibility.Collapsed;
                printReadyDate.Visibility = Visibility.Collapsed;
                readyDateBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                printReadyDateTitle.Visibility = Visibility.Visible;
                printReadyDate.Visibility = Visibility.Visible;
                readyDateBar.Visibility = Visibility.Visible;
                printReadyDate.Text = ticket.readyDate + " " + ticket.readyTime;
            }



            
            

            string taxAmount = Tax.calcTax(Items);
            if (taxAmount.Substring(0, 1) == ".")
            {
                taxAmount = "0" + taxAmount;
            }
            if (taxAmount == "0.00")
            {
                printTax.Visibility = Visibility.Collapsed;
                printTaxTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                printTax.Visibility = Visibility.Visible;
                printTaxTitle.Visibility = Visibility.Visible;
                printTax.Text = "$" + taxAmount;
            }
            printTotal.Text = "$" + taxAmount;

            string[] timesplit = timeFunctions.time24to12(ticket.inputTime).Split(':');

            printDateTime.Text = ticket.inputDate + " " + timesplit[0]+":"+timesplit[1]+" "+ timesplit[2].Substring(timesplit[2].Length - 2,2);


            
            if (Convert.ToDouble(ticket.changeAmount) > 0)//change was given
            {
                printChangeGrid.Visibility = Visibility.Visible;
                printAmountDueGrid.Visibility = Visibility.Collapsed;
                printChange.Text = (Math.Abs(Convert.ToDecimal(ticket.changeAmount))).ToString("c", CultureInfo.CurrentCulture);
            }
            else// They owe or have nothing due
            {
                printAmountDue.Text = Convert.ToDecimal(ticket.changeAmount).ToString("c", CultureInfo.CurrentCulture);
                printChangeGrid.Visibility = Visibility.Collapsed;
                printAmountDueGrid.Visibility = Visibility.Visible;
            }

            if (notes.Count == 0)
            {
                printNotesTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                printNotesTitle.Visibility = Visibility.Visible;
            }

            PrintTicketPayments.Clear();
            Payments.getPaymentsLow(ticket.ticketID, PrintTicketPayments);
            if (PrintTicketPayments.Count == 0)
            {
                printPaymentTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                printPaymentTitle.Visibility = Visibility.Visible;
            }



            string addres3 = "";
            string busCity = "";
            string busState = "";
            string busZip = "";

            busCity = ApplicationData.Current.LocalSettings.Values["busCity"].ToString();
            busState = ApplicationData.Current.LocalSettings.Values["busState"].ToString();
            busZip = ApplicationData.Current.LocalSettings.Values["busZip"].ToString();

            if (busCity != "")
            {
                addres3 = busCity + ", " + busState + " " + busZip;
            }
            else
            {
                addres3 = busState + " " + busZip;
            }
            if (addres3 != "" && addres3 != " ")
            {
                businessAddress2.Text = addres3;
            }
            else
            {
                businessAddress2.Visibility = Visibility.Collapsed;
            }


            foreach (var item in ticketListBoxPrtint.Items)
            {
                var item2 = (Item)item;

                ListViewItem lvi = ticketListBoxPrtint.ContainerFromItem(item) as ListViewItem;

                while (lvi == null)
                {
                    await Task.Delay(25);
                    lvi = ticketListBoxPrtint.ContainerFromItem(item) as ListViewItem;
                }
                //list


                //lvi.ContentTemplate = (DataTemplate)this.Resources["ItemSelectedDiscountDataTemplate"];
                if (item2.modID != "" && item2.discount != "")
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModDiscountDataTemplate"];//mod discount
                }
                else if (item2.modID != "" && item2.discount == "")
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketModDataTemplate"];//mod
                }
                else if (item2.modID == "" && item2.discount != "")
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketDiscountDataTemplate"];//dicount
                }
                else
                {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["CurrentTicketDataTemplate"];//regular
                }
            }


            businessAddress.Text = ApplicationData.Current.LocalSettings.Values["busAddress1"].ToString();

            businessName.Text = ApplicationData.Current.LocalSettings.Values["businessName"].ToString();
            businessPhone.Text = ApplicationData.Current.LocalSettings.Values["busPhone"].ToString();

            await Task.Delay(50);
                        


        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            try
            {
                printDoc.SetPreviewPage(1, printMe);
            }
            catch { }
        }

        private async void AddPages(object sender, AddPagesEventArgs e)
        {
            //await Task.Delay(10);
            // Provide a UIElement to print.


            printDoc.AddPage(printMe);
            //await newFile.DeleteAsync();



            try
            {
                printDoc.AddPagesComplete();
            }
            catch { }
        }

        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                //progress1.IsActive = false;
                //progress1.Visibility = Visibility.Collapsed;
                //progressTint.Visibility = Visibility.Collapsed;
                //resetTicketPage();
            });


            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                var test = args.Completion;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {

                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });


            }
            if (printDoc == null)
            {

                return;
            }

    
            printMan.PrintTaskRequested -= PrintTaskRequested;

        }

        
    }
}
