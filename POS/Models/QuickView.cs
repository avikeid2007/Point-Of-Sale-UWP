using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace POS.Models
{
    class QuickView
    {
        public static Task<List<Ticket>> generateQuickView(Ticket ticket,Page page, ObservableCollection<Item> list, ObservableCollection<Notes> notes, ObservableCollection<Tax> TaxList, ObservableCollection<Payments> payments, ListView ticketListview, bool loading, TextBlock custName, TextBlock custPhone, TextBlock ticketTotal, TextBlock readyDate, TextBlock ticketID, TextBlock ticketInputDate, TextBlock ticketTax, TextBlock readyDateTitle, TextBlock notesTitle, TextBlock termsTitle, TextBlock changeAmountTitle, TextBlock changeAmountText)
        {
            loading = true;
            TaskCompletionSource<List<Ticket>> tcs = new TaskCompletionSource<List<Ticket>>();
            Task.Run(async () =>
            {
                List<Ticket> ticketList = new List<Ticket>();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    list.Clear();
                    notes.Clear();
                    payments.Clear();
                });
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        
                        if(ticket.custName == "" || ticket.custName == null)
                        {
                            custName.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            custName.Text = ticket.custName;
                            custName.Visibility = Visibility.Visible;
                        }
                        if(ticket.custNumber == "" || ticket.custNumber == null)
                        {
                            custPhone.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            custPhone.Text = ticket.custNumber;
                            custPhone.Visibility = Visibility.Visible;

                        }

                        //ready date and time
                        if(ticket.readyDate == "" && ticket.readyTime == "")
                        {
                            readyDateTitle.Visibility = Visibility.Collapsed;
                            readyDate.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            readyDateTitle.Visibility = Visibility.Visible;
                            readyDate.Visibility = Visibility.Visible;
                            readyDate.Text = " " + ticket.readyDate + " " + ticket.readyTime;
                        }
                        ticketID.Text = ticket.ticketID;
                        ticketTotal.Text = ticket.total;
                        if (ticketTotal.Text.Substring(0, 1) == ".")
                        {
                            ticketTotal.Text = "0" + ticketTotal.Text;
                        }
                        ticketInputDate.Text = ticket.inputDate + " " + timeFunctions.time24to12(ticket.inputTime); // input time and date
                    });


                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                  
                    if (notes.Count == 0)
                    {
                        notesTitle.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        notesTitle.Visibility = Visibility.Visible;

                    }
                });


                await Ticket.getObservablesAsync(ticket, TaxList, list, notes);

                
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {

                    if (Convert.ToDouble(ticket.changeAmount) > 0)//change was given
                    {
                        changeAmountTitle.Visibility = Visibility.Visible;
                        changeAmountText.Visibility = Visibility.Visible;
                        changeAmountText.Text = ticket.changeAmount;
                    }
                    else if (Convert.ToDouble(ticket.changeAmount) <= 0)//amount due
                    {
                        changeAmountTitle.Visibility = Visibility.Visible;
                        changeAmountText.Visibility = Visibility.Visible;
                        changeAmountTitle.Text = "Amount Due:";
                        changeAmountText.Text = ticket.changeAmount;
                    }

                    
                    
                });




                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Payments.getPaymentsLow(ticket.ticketID, payments);
                    if (payments.Count == 0)
                    {
                        termsTitle.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        termsTitle.Visibility = Visibility.Visible;

                    }
                });
               
                /*
                //setting the datatemplate for each item
                int k = 0;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    foreach (var item in ticketListview.Items)
                    {
                        //var item = (Item)lvi2;

                        ListViewItem lvi = ticketListview.ContainerFromItem(item) as ListViewItem;

                        while (lvi == null)
                        {
                            await Task.Delay(25);
                            lvi = ticketListview.ContainerFromItem(item) as ListViewItem;
                        }
                        //list

                        //lvi.ContentTemplate = (DataTemplate)this.Resources["ItemSelectedDiscountDataTemplate"];
                        if (list[k].modName.Count != 0 && list[k].discount != "")
                        {
                            lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketModDiscountDataTemplate"];//mod discount
                        }
                        else if (list.ElementAt(k).modName.Count != 0 && list.ElementAt(k).discount == "")
                        {
                            lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketModDataTemplate"];//mod
                        }
                        else if (list.ElementAt(k).modName.Count == 0 && list.ElementAt(k).discount != "")
                        {
                            lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketDiscountDataTemplate"];//dicount
                        }
                        else
                        {
                            lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketDataTemplate"];//regular
                        }

                        k++;
                    }
                    
                    loading = false;
                    
                });
                */

                //getting total tax and setting it to textblock
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ticketListview.UpdateLayout();

                        ticketTax.Text = Tax.calcTax(list);
                        if (ticketTax.Text.Substring(0, 1) == ".")
                        {
                            ticketTax.Text = "0" + ticketTax.Text;
                        }
                    });

                loading = false;
                tcs.SetResult(ticketList);
            });

            return tcs.Task;
        }




        public static async void updateDataTemplate(ListView quickListView,Page page)
        {
            foreach (var item in quickListView.Items)
            {
                var item2 = (Item)item;

                ListViewItem lvi = quickListView.ContainerFromItem(item) as ListViewItem;

                while (lvi == null)
                {
                    await Task.Delay(25);
                    lvi = quickListView.ContainerFromItem(item) as ListViewItem;
                }
                //list


                //lvi.ContentTemplate = (DataTemplate)this.Resources["ItemSelectedDiscountDataTemplate"];
                if (item2.modID != "" && item2.discount != "")
                {
                    lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketModDiscountDataTemplate"];//mod discount
                }
                else if (item2.modID != "" && item2.discount == "")
                {
                    lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketModDataTemplate"];//mod
                }
                else if (item2.modID == "" && item2.discount != "")
                {
                    lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketDiscountDataTemplate"];//dicount
                }
                else
                {
                    lvi.ContentTemplate = (DataTemplate)page.Resources["CurrentTicketDataTemplate"];//regular
                }
            }
        }

    }

    
}
