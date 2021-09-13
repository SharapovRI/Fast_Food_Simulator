using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Food_Simulator
{
    public static class Server
    {
        private static Queue<Customer> customerOrder;
        private static Queue<Task<int>> orders;
        private static MainWindow Window;

        public static void ServerStart(MainWindow mainWindow, CancellationToken token)
        {
            Window = mainWindow;
            customerOrder = new Queue<Customer>();
            orders = new Queue<Task<int>>();

            Task.Factory.StartNew(() =>
            {
                    while (!token.IsCancellationRequested)
                    {
                        Window.Dispatcher.Invoke(() => Window.pickupCustomersCount.Content = customerOrder.Count);
                    }
                
            });

            Task.Factory.StartNew(() =>
            {
                while(!token.IsCancellationRequested)
                {
                    if (orders.Count > 0)
                    {
                        Task<int> currentOrder = orders.Dequeue();
                        Window.Dispatcher.Invoke(() => Window.lb_currently_available.Content = currentOrder.Result);
                        CallOut(currentOrder);
                    }
                }
            });
        }

        public static void AddNewCustomerToQueue(Customer customer)
        {
            customerOrder.Enqueue(customer);
        }

        public static void GetPreparedOrder(Task<int> order)
        {
            orders.Enqueue(order);
        }

        public static void CallOut(Task<int> order)
        {
            if (customerOrder.Peek().numberOfTicket == order.Result)
            {
                customerOrder.Dequeue();
                Console.WriteLine($"Заказ номер {order.Result} готов к выдаче!");
            }
        }
    }
}
