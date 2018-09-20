using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace POS.UserControls
{
    public sealed partial class PrintInvoice : UserControl
    {
        public ObservableCollection<Payments> InvoicePayments;
        public int elementsPerPage = 50;
        public int totalPrintPageCount;

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        public PrintTaskOptions printingOptions;
        public PrintPageDescription printPageDescription;

        public ProgressRing progress1;
        public Rectangle progressTint1;

        public Ticket printTicket;
        public ObservableCollection<Tax> TaxList;
        public ObservableCollection<Item> ticketToSend;
        public ObservableCollection<Item> QuickViewTicket;

        public PrintInvoice()
        {
            this.InitializeComponent();
            InvoicePayments = payManager.GetPay();
            QuickViewTicket = itemManager.GetItem();
            ticketToSend = itemManager.GetItem();
        }


        

        public async void printInvoice(Ticket ticket, ObservableCollection<Tax> tax, ProgressRing progress, Rectangle progressTint)
        {
            printTicket = ticket;
            TaxList = tax;
            progress1 = progress;
            progressTint1 = progressTint;

            printMan.PrintTaskRequested += null;
            printDoc.Paginate += null;
            printDoc.GetPreviewPage += null;
            printDoc.AddPages += null;

            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
            printMan.PrintTaskRequested += PrintTaskRequested;
            ApplicationData.Current.LocalSettings.Values["loadingPrint"] = "true";
            ApplicationData.Current.LocalSettings.Values["invoiceImagesMade"] = "false";

            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can't proceed at this time.",
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


        /*
  * print button -> printTaskRequest -> printTaskSourceRequest -> Pagination -> get preview page
  */

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
            //test1.MinHeight = printPageDescription.PageSize.Height;
            printInvoiceWrapper.Width = printPageDescription.PageSize.Width;
            printInvoiceWrapper.Height = printPageDescription.PageSize.Height;
            printMe.Height = printPageDescription.PageSize.Height;


            /*
            if (printPageDescription.PageSize.Height == 1056)//regular sheet portrait
            {
                elementsPerPage = 35;
            }
            else if (printPageDescription.PageSize.Height > 1056)
            {
                elementsPerPage = 50; // bigger than 50
            }
            if (printPageDescription.PageSize.Height < 1056)
            {
                elementsPerPage = 26;// less than 50
            }
            else if (printPageDescription.PageSize.Height < 750)
            {
                elementsPerPage = 18;//smallest
            }
            */


            await Task.Delay(50);





            //get the total page count

            Invoice.getEmailItems(printTicket, QuickViewTicket, true, TaxList); //get items and mod and gray, returns them in QuickViewTicket
            int itemsDueToLogo = await Invoice.getItemsDueToLogo();
            int paymentCount = Payments.getTicketPayCount(printTicket.ticketID); //get ticketPayment Count


            int totalItemCount = QuickViewTicket.Count() + itemsDueToLogo + 9 + paymentCount + 3;

            totalPrintPageCount = Invoice.getPageCount(elementsPerPage, totalItemCount, paymentCount);

            if ((ApplicationData.Current.LocalSettings.Values["invoiceImagesMade"] as string) != "true")
            {
                Invoice.createInvoiceImage(printTicket, QuickViewTicket, ticketToSend, frame, TaxList, progress1, progressTint1);
            }
            ApplicationData.Current.LocalSettings.Values["invoiceImagesMade"] = "true";



            printDoc.SetPreviewPageCount(totalPrintPageCount, PreviewPageCountType.Final);
        }

        private async void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {

            while ((ApplicationData.Current.LocalSettings.Values["loadingPrint"] as string) == "true")
            {
                await Task.Delay(25);

            }


            await Task.Delay(200);
            // Provide a UIElement as the print preview.
            for (int i = 1; i <= totalPrintPageCount; i++)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile newFile = await folder.GetFileAsync("Invoice " + printTicket.ticketID + "." + i + ".png");
                var bitmapImage = new BitmapImage();

                bitmapImage.SetSource(await newFile.OpenAsync(FileAccessMode.Read));
                printMe.Source = bitmapImage;

                try
                {
                    printDoc.SetPreviewPage(i, printMe);
                }
                catch { }
            }

        }

        private async void AddPages(object sender, AddPagesEventArgs e)
        {
            //await Task.Delay(10);
            // Provide a UIElement to print.
            for (int i = 1; i <= totalPrintPageCount; i++)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile newFile = await folder.GetFileAsync("Invoice " + printTicket.ticketID + "." + i + ".png");
                var bitmapImage = new BitmapImage();

                bitmapImage.SetSource(await newFile.OpenAsync(FileAccessMode.Read));
                printMe.Source = bitmapImage;

                printDoc.AddPage(printMe);
                //await newFile.DeleteAsync();
            }


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
                progress1.IsActive = false;
                progress1.Visibility = Visibility.Collapsed;
                progressTint1.Visibility = Visibility.Collapsed;
            });
            //delete images
            for (int i = 1; i <= totalPrintPageCount; i++)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile newFile = await folder.GetFileAsync("Invoice " + printTicket.ticketID + "." + i + ".png");
                await newFile.DeleteAsync();
            }
            ApplicationData.Current.LocalSettings.Values["invoiceImagesMade"] = "false";


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
    }
}
