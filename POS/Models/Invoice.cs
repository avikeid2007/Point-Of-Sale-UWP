using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace POS.Models
{
    class Invoice
    {


        public static void storeCustomerTicketData(Ticket ticket, Customer customer)
        {
            ApplicationData.Current.LocalSettings.Values["ticketCustomerName"] = customer.full;
            ApplicationData.Current.LocalSettings.Values["ticketCustomerHome"] = customer.home;
            ApplicationData.Current.LocalSettings.Values["ticketCustomerCompany"] = customer.company;
            ApplicationData.Current.LocalSettings.Values["ticketCustomerEmail"] = customer.email;
            ApplicationData.Current.LocalSettings.Values["ticketTotal"] = ticket.total;
            ApplicationData.Current.LocalSettings.Values["ticketCustomerCell"] = customer.cell;
            ApplicationData.Current.LocalSettings.Values["ticketCustomerWork"] = customer.work;
            ApplicationData.Current.LocalSettings.Values["ticketID"] = ticket.ticketID;
            ApplicationData.Current.LocalSettings.Values["ticketReadyDate"] = ticket.readyDate;
            ApplicationData.Current.LocalSettings.Values["ticketReadyTime"] = ticket.readyTime;
            ApplicationData.Current.LocalSettings.Values["ticketInputDate"] = ticket.inputDate;
            ApplicationData.Current.LocalSettings.Values["ticketInputTime"] = ticket.inputTime;

            ApplicationData.Current.LocalSettings.Values["ticketAmountDue"] = ticket.changeAmount;
        }

        public static int getPageCount(int itemsCanBeOnPage, int totalItemsDueToInvoice, int paymentCount)
        {
            int pageCount = totalItemsDueToInvoice / itemsCanBeOnPage;
            double test2 = Convert.ToDouble(totalItemsDueToInvoice) / itemsCanBeOnPage;
            if (pageCount < test2 || pageCount == test2)
            {
                pageCount += 1;
            }
            if (paymentCount == 0)
            {
                int dupItemcount = totalItemsDueToInvoice;
                while (dupItemcount > 0)
                {
                    dupItemcount -= itemsCanBeOnPage;
                    if (51 >= totalItemsDueToInvoice - 1)
                    {
                        pageCount -= 1;
                    }
                }
            }
            if (pageCount == 0)
            {
                pageCount = 1;
            }
            return pageCount;
        }

        public async static Task<int> getItemsDueToLogo()
        {
            //get logo ratio
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            int itemsDueToLogo = 0;

            string showLogo = ApplicationData.Current.LocalSettings.Values["showLogo"].ToString();
            if (showLogo == "false")
            {
                itemsDueToLogo = 0;
            }
            else
            {
                try
                {
                    StorageFile newFile = await folder.GetFileAsync("logo.png");
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(await newFile.OpenAsync(FileAccessMode.Read));
                    double height = bitmapImage.PixelHeight;
                    double width = bitmapImage.PixelWidth; ;
                    double heightToWidthRatio = height / width;
                    if (heightToWidthRatio < 1)
                    {
                        itemsDueToLogo = 0;
                    }
                    else if (heightToWidthRatio >= 1 && heightToWidthRatio < 1.1)
                    {
                        itemsDueToLogo = 1;
                    }
                    else if (heightToWidthRatio >= 1.1 && heightToWidthRatio < 1.2)
                    {
                        itemsDueToLogo = 2;
                    }
                    else if (heightToWidthRatio >= 1.2 && heightToWidthRatio < 1.3)
                    {
                        itemsDueToLogo = 3;
                    }
                    else
                    {
                        itemsDueToLogo = 4;
                    }
                }
                catch { itemsDueToLogo = 0; }
            }
            return itemsDueToLogo;
        }

        public static void getEmailItems(Ticket ticket, ObservableCollection<Item> list, bool dup, ObservableCollection<Tax> TaxList)
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            list.Clear();

            string[] itemID = ticket.items.Split(':');
            string[] itemPrices = ticket.prices.Split(':');
            string[] itemQuan = ticket.quantities.Split(':');
            string[] itemDiscount = ticket.discounts.Split(':');
            string[] itemModID = ticket.modifiers.Split(':');

            string[] tax = ticket.taxID.Split(':');// between each tax key there is a ; between each item set is :

            List<string> itemNames = new List<string>();
            SQLiteConnection dbConnection = new SQLiteConnection("Items.db");
            bool gray = false;
            foreach (string id in itemID)
            {
                string sSQL = @"SELECT [name] FROM Items WHERE stringItemID = '" + id + "'";
                ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                while (dbState.Step() == SQLiteResult.ROW)
                {

                    string sName = dbState["name"] as string;
                    itemNames.Add(sName);
                    break;
                }
                dbState.Dispose();
            }


            //Load into observable collection and get tax for each item

            for (int i = 0; i != itemNames.Count; i++)
            {
                decimal taxAmount = 0;

                string[] taxItem = tax[i].Split(';');// incase there is more than 1 tax type on one item
                for (int j = 0; j != taxItem.Length; j++)//Generate tax amount
                {

                    foreach (Tax taxObject in TaxList)
                    {
                        if (taxItem[j] == taxObject.taxKey)
                        {
                            decimal rate = Convert.ToDecimal(taxObject.rate);
                            double rateAT = Convert.ToDouble(taxObject.taxAtAmount);
                            if (Convert.ToDouble(itemPrices[i]) >= rateAT)
                            {
                                taxAmount = taxAmount + Convert.ToDecimal(taxObject.rate) * Convert.ToDecimal(itemPrices[i]) / 100;
                            }

                        }

                    }//end foreach
                }//end foreach

                taxAmount = taxAmount * Convert.ToDecimal(itemQuan[i]);
                if (taxAmount == 0)
                {
                    tax[i] = "";
                }
                else
                {
                    tax[i] = "X";
                }
                if (itemPrices[i].Substring(0, 1) == ".")
                {
                    itemPrices[i] = "0" + itemPrices[i];
                }

                list.Add(new Item { name = itemNames[i], gray = gray, itemID = itemID[i], minQuantity = itemQuan[i], price = itemPrices[i], discount = "No Discount", taxAmount = Convert.ToString(taxAmount), taxID = tax[i], modID = itemModID[i] });

                List<string> modNames = new List<string>();
                List<string> modPrice = new List<string>();
                if (itemModID[i] != "")
                {
                    string[] individualModID = itemModID[i].Split(';');
                    foreach (string modID in individualModID)
                    {
                        string sSQL = @"SELECT * FROM Modifiers WHERE stringModID = '" + modID + "'";
                        ISQLiteStatement dbState = dbConnection.Prepare(sSQL);
                        while (dbState.Step() == SQLiteResult.ROW)
                        {
                            string sName = dbState["name"] as string;
                            string sValue = dbState["value"] as string;
                            string sValueType = dbState["valueType"] as string;
                            string changeAmount = Modifier.getModChangeAmount(sValueType, sValue, Convert.ToInt32(itemQuan[i]), itemPrices[i]);

                            list.Add(new Item { name = sName, gray = gray, minQuantity = "0", price = changeAmount, discount = "No Discount" });
                            break;

                        }
                        dbState.Dispose();
                    }
                }//ends get mods
                if (itemDiscount[i] != "")
                {
                    list.Add(new Item { price = itemPrices[i], discount = itemDiscount[i], gray = gray, modID = "No Mods" });
                }
                try
                {


                    if (localSettings.Values["shadeItems"].ToString() == "true")
                    {
                        if (gray == false)
                        {
                            gray = true;
                        }
                        else
                        {
                            gray = false;
                        }

                    }
                }
                catch
                {
                    if (gray == false)
                    {
                        gray = true;
                    }
                    else
                    {
                        gray = false;
                    }
                }


            }//end for every item

            dbConnection.Dispose();
            string totalTax = Tax.calcTax(list);
            if (totalTax.Substring(0, 1) == ".")
            {
                totalTax = "0" + totalTax;
            }

            ApplicationData.Current.LocalSettings.Values["ticketTax"] = totalTax;

        }

        public static async Task<StorageFile> getAttachmentAsync(int pageCount,string ticketID )
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            string attachName = "";
            if (pageCount > 1)
            {

                attachName = "Invoice " + ticketID + ".zip";
                try
                {
                    ZipFile.CreateFromDirectory(folder.Path + "\\Invoice " + ticketID + ".zip", ApplicationData.Current.LocalFolder.Path + "\\Invoice " + ticketID + ".zip");

                }
                catch { }


                //ZipArchive test = new ZipArchive()
                for (int j = 1; j != pageCount + 1; j++)
                {

                    using (ZipArchive archive = ZipFile.Open(ApplicationData.Current.LocalFolder.Path + "\\Invoice " + ticketID + ".zip", ZipArchiveMode.Update))
                    {
                        archive.CreateEntryFromFile(ApplicationData.Current.LocalFolder.Path + "\\Invoice " + ticketID + "." + j + ".png", "Invoice " + ticketID + "." + j + ".png");
                        //archive.ExtractToDirectory(extractPath);
                    }
                    StorageFile image = await folder.GetFileAsync("Invoice " + ticketID + "." + j + ".png");
                    await image.DeleteAsync();

                }
            }
            else
            {
                attachName = "Invoice " + ticketID + ".1.png";
            }
            StorageFile attachmentFile = await folder.GetFileAsync(attachName);

            return attachmentFile;
        }

        public static async void createEmailToSend(int pageCount, string ticketID, Customer ticketCust, ProgressRing progress, Rectangle progressTint)
        {
            //get attachment
            StorageFile attachmentFile = await Invoice.getAttachmentAsync(pageCount, ticketID);
            

            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            string message = "";
            string sender1 = "";
            string greeting = "";
            string closing = "";
            string toggleClosing = "";
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                message = localSettings.Values["messageText"].ToString();
                sender1 = localSettings.Values["senderName"].ToString();
                int greetingIndex = Convert.ToInt32(localSettings.Values["greetingIndex"].ToString());
                switch (greetingIndex)
                {
                    case 0:

                        if (ticketCust.full != null && ticketCust.full != "" && ticketCust.full != " ")
                        {
                            greeting = ticketCust.full + ",";
                        }
                        else
                            greeting = "Valued Customer,";
                        break;
                    case 1:
                        greeting = "Valued Customer,";
                        break;

                }
                int closing2 = Convert.ToInt32(localSettings.Values["closingIndex"].ToString());
                switch (closing2)
                {
                    case 0:
                        closing = "Sincerely,";
                        break;
                    case 1:
                        closing = "Yours truly,";
                        break;
                    case 2:
                        closing = "Best,";
                        break;
                    case 3:
                        closing = "All the best,";
                        break;
                    case 4:
                        closing = "Hope to hear from you soon,";
                        break;
                    case 5:
                        closing = "Kind Regards,";
                        break;
                    case 6:
                        closing = "Take care,";
                        break;
                    case 7:
                        closing = "Show me the money,";
                        break;

                }
                toggleClosing = localSettings.Values["closingEnable"].ToString();
                emailMessage.Subject = localSettings.Values["messageSub"].ToString();
                if (toggleClosing == "1")
                {
                    emailMessage.Body = "Dear " + greeting + System.Environment.NewLine + System.Environment.NewLine + message + System.Environment.NewLine + System.Environment.NewLine + closing + System.Environment.NewLine + sender1; ;
                }
                else
                {
                    emailMessage.Body = "Dear " + greeting + System.Environment.NewLine + System.Environment.NewLine + message + System.Environment.NewLine + System.Environment.NewLine;
                }
            }
            catch { emailMessage.Body = ""; }




            
            string emailAddress = ticketCust.email;
            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                    attachmentFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }

            if (emailAddress != null && emailAddress != "")
            {
                var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(emailAddress);
                emailMessage.To.Add(emailRecipient);
            }

            progress.IsActive = false;
            progress.Visibility = Visibility.Collapsed;
            progressTint.Visibility = Visibility.Collapsed;

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);//bring up mail app
            await attachmentFile.DeleteAsync();//delete the sent file, has to be after mail app is shown

        }

        public static async void createEmail(Ticket selectedTicket, ObservableCollection<Item> QuickViewTicket, ObservableCollection<Item> ticketToSend, Frame frame, ObservableCollection<Tax> TaxList,  ProgressRing progress, Rectangle progressTint)
        {
            //Show loading
            progress.IsActive = true;
            progress.Visibility = Visibility.Visible;
            progressTint.Visibility = Visibility.Visible;
            ApplicationData.Current.LocalSettings.Values["loadingInvoive"] = "true";

            Customer ticketCust = Customer.getCustomerFromID(selectedTicket.customerID); //getCustomerData
            Invoice.storeCustomerTicketData(selectedTicket, ticketCust);//store customer/ticket info to be retrieved at invoicePage
            int paymentCount = Payments.getTicketPayCount(selectedTicket.ticketID); //get ticketPayment Count
            Invoice.getEmailItems(selectedTicket,  QuickViewTicket, true, TaxList); //get items and mod and gray

            //get logo ratio
            int itemsDueToLogo = await Invoice.getItemsDueToLogo();



            int totalItemCount = QuickViewTicket.Count() + itemsDueToLogo + 9 + paymentCount + 3; // ticketItems + logoOverlap  + headerNoOverlap + payCount + payNoPayments
            //if i move dates up, remove 2 items that entire invoice needs to hold
            if ((ticketCust.first == "" && ticketCust.last == "" && ticketCust.home == "" && ticketCust.cell == "" && ticketCust.work == "" && ticketCust.company == "") || (ApplicationData.Current.LocalSettings.Values["showCustName"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustCompany"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustPhone"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustEmail"].ToString() == "false"))
            {
                totalItemCount -= 2;
            }

            //get page count for pagination label
            int pageAllows = 50;
            int pageCount = Invoice.getPageCount(pageAllows, totalItemCount, paymentCount);

            int lastIndex = 0;
            int i = 1;
            for (; totalItemCount >= 0; i++)
            {
                int itemsAllowable = 34 + 4 - itemsDueToLogo; ; //with header and no additional payments
                if (i == 1)
                {
                    ApplicationData.Current.LocalSettings.Values["ShowHeader"] = "true";
                    //if no customer data or custdata is blank allow two more  items because ready date and time will move up
                    if((ticketCust.first == "" && ticketCust.last == "" && ticketCust.home == "" && ticketCust.cell == "" && ticketCust.work == "" && ticketCust.company == "") || (ApplicationData.Current.LocalSettings.Values["showCustName"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustCompany"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustPhone"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustEmail"].ToString() == "false"))
                    {
                        itemsAllowable += 2;
                    }
                }
                else
                {
                    itemsAllowable += 13 - 4 + itemsDueToLogo;
                    ApplicationData.Current.LocalSettings.Values["ShowHeader"] = "false";
                }

                if (i == pageCount)
                {
                    ApplicationData.Current.LocalSettings.Values["ShowPaymentTotal"] = "true";
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["ShowPaymentTotal"] = "false";
                    itemsAllowable += 2;// two because there is 3 lines on the payment but 1 of them is the page count that wont be removed
                }

                ApplicationData.Current.LocalSettings.Values["pageCount"] = "Page " + i + " of " + pageCount;

                ticketToSend.Clear();
                int j = lastIndex;
                for (; j <= lastIndex + itemsAllowable; j++)//add Items to send
                {
                    if (j > QuickViewTicket.Count - 1)
                    {
                        break;
                    }
                    ticketToSend.Add(QuickViewTicket[j]);

                }
                lastIndex = j;

                //send item list to printInvoicePage
                frame.Navigate(typeof(printInvoicexaml), ticketToSend);
                var a = (printInvoicexaml)frame.Content;
                ListView invoiceListView = (ListView)a.FindName("invoiceList");

                //await Task.Delay(QuickViewTicket.Count() * 25);

                //wait for printInvoicePage to finish
                while ((ApplicationData.Current.LocalSettings.Values["loadingInvoive"] as string) == "true")
                {
                    await Task.Delay(25);
                }


                //create image File
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(frame);
                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = await folder.CreateFileAsync("Invoice " + selectedTicket.ticketID + "." + i + ".png", CreationCollisionOption.ReplaceExisting);//create file

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))//set file contents
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        pixelBuffer.ToArray());

                    await encoder.FlushAsync();

                }
                frame.BackStack.Clear();
                frame.Content = null;

                totalItemCount -= pageAllows;

            }//end creating Images


            Invoice.createEmailToSend(pageCount, selectedTicket.ticketID, ticketCust, progress, progressTint);
        }

        public static async void createInvoiceImage(Ticket selectedTicket, ObservableCollection<Item> QuickViewTicket, ObservableCollection<Item> ticketToSend, Frame frame, ObservableCollection<Tax> TaxList, ProgressRing progress, Rectangle progressTint)
        {
            try
            {
                //Show loading
                progress.IsActive = true;
                progress.Visibility = Visibility.Visible;
                progressTint.Visibility = Visibility.Visible;
                ApplicationData.Current.LocalSettings.Values["loadingInvoive"] = "true";
                ApplicationData.Current.LocalSettings.Values["loadingPrint"] = "true";

                Customer ticketCust = Customer.getCustomerFromID(selectedTicket.customerID); //getCustomerData
                Invoice.storeCustomerTicketData(selectedTicket, ticketCust);//store customer/ticket info to be retrieved at invoicePage
                int paymentCount = Payments.getTicketPayCount(selectedTicket.ticketID); //get ticketPayment Count
                Invoice.getEmailItems(selectedTicket, QuickViewTicket, true, TaxList); //get items and mod and gray

                //get logo ratio
                int itemsDueToLogo = await Invoice.getItemsDueToLogo();



                int totalItemCount = QuickViewTicket.Count() + itemsDueToLogo + 9 + paymentCount + 3; // ticketItems + logoOverlap  + headerNoOverlap + payCount + payNoPayments
                                                                                                      //if i move dates up, remove 2 items that entire invoice needs to hold
                if ((ticketCust.first == "" && ticketCust.last == "" && ticketCust.home == "" && ticketCust.cell == "" && ticketCust.work == "" && ticketCust.company == "") || (ApplicationData.Current.LocalSettings.Values["showCustName"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustCompany"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustPhone"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustEmail"].ToString() == "false"))
                {
                    totalItemCount -= 2;
                }

                //get page count for pagination label
                int pageAllows = 50;
                int pageCount = Invoice.getPageCount(pageAllows, totalItemCount, paymentCount);

                int lastIndex = 0;
                int i = 1;
                for (; totalItemCount >= 0; i++)
                {
                    int itemsAllowable = 34 + 4 - itemsDueToLogo; ; //with header and no additional payments
                    if (i == 1)
                    {
                        ApplicationData.Current.LocalSettings.Values["ShowHeader"] = "true";
                        //if no customer data or custdata is blank allow two more  items because ready date and time will move up
                        if ((ticketCust.first == "" && ticketCust.last == "" && ticketCust.home == "" && ticketCust.cell == "" && ticketCust.work == "" && ticketCust.company == "") || (ApplicationData.Current.LocalSettings.Values["showCustName"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustCompany"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustPhone"].ToString() == "false" && ApplicationData.Current.LocalSettings.Values["showCustEmail"].ToString() == "false"))
                        {
                            itemsAllowable += 2;
                        }
                    }
                    else
                    {
                        itemsAllowable += 13 - 4 + itemsDueToLogo;
                        ApplicationData.Current.LocalSettings.Values["ShowHeader"] = "false";
                    }

                    if (i == pageCount)
                    {
                        ApplicationData.Current.LocalSettings.Values["ShowPaymentTotal"] = "true";
                    }
                    else
                    {
                        ApplicationData.Current.LocalSettings.Values["ShowPaymentTotal"] = "false";
                        itemsAllowable += 2;// two because there is 3 lines on the payment but 1 of them is the page count that wont be removed
                    }

                    ApplicationData.Current.LocalSettings.Values["pageCount"] = "Page " + i + " of " + pageCount;

                    ticketToSend.Clear();
                    int j = lastIndex;
                    for (; j <= lastIndex + itemsAllowable; j++)//add Items to send
                    {
                        if (j > QuickViewTicket.Count - 1)
                        {
                            break;
                        }
                        ticketToSend.Add(QuickViewTicket[j]);

                    }
                    lastIndex = j;

                    //send item list to printInvoicePage
                    frame.Navigate(typeof(printInvoicexaml), ticketToSend);
                    var a = (printInvoicexaml)frame.Content;
                    ListView invoiceListView = (ListView)a.FindName("invoiceList");

                    //await Task.Delay(QuickViewTicket.Count() * 25);

                    //wait for printInvoicePage to finish
                    while ((ApplicationData.Current.LocalSettings.Values["loadingInvoive"] as string) == "true")
                    {
                        await Task.Delay(25);
                    }


                    //create image File
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(frame);
                    var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile file = await folder.CreateFileAsync("Invoice " + selectedTicket.ticketID + "." + i + ".png", CreationCollisionOption.ReplaceExisting);//create file

                    try
                    {
                        using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))//set file contents
                        {
                            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                            encoder.SetPixelData(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Ignore,
                                (uint)renderTargetBitmap.PixelWidth,
                                (uint)renderTargetBitmap.PixelHeight,
                                DisplayInformation.GetForCurrentView().LogicalDpi,
                                DisplayInformation.GetForCurrentView().LogicalDpi,
                                pixelBuffer.ToArray());

                            await encoder.FlushAsync();

                        }
                    }
                    catch { }
                    frame.BackStack.Clear();
                    frame.Content = null;

                    totalItemCount -= pageAllows;

                }//end creating Images
                ApplicationData.Current.LocalSettings.Values["loadingPrint"] = "false";
            }
            catch { }
        }
    }
}
