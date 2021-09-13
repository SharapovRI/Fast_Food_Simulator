using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Food_Simulator
{
    public static class OrderTaker
    {
        private static Queue<Customer> customerOrder;
        private static int NumberOfTicket = 0;
        private static MainWindow Window;
        private static int customerInterval;

        public static void OrderTakerStart(MainWindow mainWindow, int customerInt, CancellationToken token)
        {
            Window = mainWindow;
            customerInterval = customerInt;
            customerOrder = new Queue<Customer>();
            Task.Factory.StartNew(() =>
            {
                
                    while (!token.IsCancellationRequested)
                    {
                        Thread.Sleep(customerInterval);
                        AddNewCustomer();
                    }
                
            }, token);

            Task.Factory.StartNew(() =>
            {
                
                    while (!token.IsCancellationRequested)
                    {
                        Window.Dispatcher.Invoke(() => Window.queueCustomersCount.Content = customerOrder.Count);
                    }
                
            }, token);

            Task.Run(() =>
            {
                
                    while (!token.IsCancellationRequested)
                    {
                        if (customerOrder.Count > 0)
                        {
                            Task ticketCreating = Task.Factory.StartNew(() => CreateTicket(), TaskCreationOptions.AttachedToParent);
                            ticketCreating.Wait();
                        }
                    }
                
            }, token);
        }

        static void AddNewCustomer()
        {
            Customer customer = new Customer(customerOrder);
            customerOrder.Enqueue(customer);
        }

        public static void CreateTicket()
        {
            if (customerOrder.Count > 0)
            {
                Customer customer = customerOrder.Dequeue();
                int TicketNum = ++NumberOfTicket;
                Window.Dispatcher.Invoke(() => Window.currentlyTakenTicket.Content = TicketNum);
                Thread.Sleep(6000);

                customer.numberOfTicket = TicketNum;

                Task<int> ticket = new Task<int>(() =>
                {
                    Console.WriteLine($"Заказ номер {TicketNum} поставлен в очередь!");
                    return TicketNum;
                });
                ticket.Start();

                customer.IsTicketCreated = true;
                Kitchen.GetNewTicket(ticket);
            }
        }
    }
}
