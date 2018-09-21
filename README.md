# Point-Of-Sale-UWP
There are files missing from this software intentially due to intelectual property reasons. This project has been in progress since 5/30/2017. I work off of visual studio team services.

<h2>Home Page</h2>
This app has an intuitive design.  The app features a light and dark theme and follows windows 10's fluent design. The following screen shots are of the home page. 
The home page can be customized to show differnt set of data on the right. For example, last 100 ticket, last 50 tickets, unpaid tickets, etc.
This is also where the emplyoees can punch in and out.
<img src="POS/Assets/GitReadMe/homeLight.png" height="400px" />
The list of tickets on the right side are interactive. There are serveral options the employee can choose from after clicking on a ticket: Quick View, Edit Ticket, Mark as Picked Up(premature), Print, Email Invoice. 
<img src="POS/Assets/GitReadMe/ticketExpanded.PNG" height="200px"  />

The quick view is a custom element that is used acrosss multiple pages. It is a view of the ticket without going into the edit ticket page. The print button brings up a menu that gives the employee three options to print from: Invoice, Customer Copy, Store Copy. Currently the customer copy and store copy print the same ticket.
<br/>
<img src="POS/Assets/GitReadMe/homeQuickView.png" width="400px"  />
<img src="POS/Assets/GitReadMe/printMenu.png" width="400px"  />
<h2>Create Ticket Page</h2>
This page is where the employee can create tickets. They can link customers to the ticket on the left by searching for their name, phone number, or company(premature). The customer panel can be shown or hidden by tapping the arrow. After selecting a customer from the list, their name appear on the top of the ticket. Items and categories are in the center of the screen. The right side of the screen is the ticket and it will show the items, modifiers and discounts. The employee can also add notes to the ticket. The bottom right of the screen shows total, tax and item count.
<img src="POS/Assets/GitReadMe/createTicketPage.png"   />
After clicking "Finalize Ticket" in the bottom corener, the page shows the layout as in the screenshot below. Here the employee can make payments, set the ready date and time, and give discounts(premature) that apply to the entire purchase. After pressing "Finish" a change dialog comes up to give the customer change if needed. After the employee submits the ticket, the ticket will print.
<img src="POS/Assets/GitReadMe/finalizeTicket.PNG"   />
<h2>Printables</h2>
There is currently 4 printable items in this software. The first image on the left is the invoice. It can be printed on the homepage, the create ticket page, and the ticket look up page. The image on the right is from the ticket look up page. It will print from the search results.
<div>
 <img src="POS/Assets/GitReadMe/printInvoice.png" width="435px"  />
 <img src="POS/Assets/GitReadMe/printTicketLookUp.png" width="435px"  />
</div>
The image on the left is the a time sheet printed from the reports page. It has the search critiria at the top with the total amount of time the employee worked.
<div>
<img src="POS/Assets/GitReadMe/timesheet.png" width="435px"  />
</div>

