using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Food_Simulator
{
    public static class Kitchen
    {
        private static Queue<Task<int>> queueOfTickets;

        private static MainWindow Window;
        private static int fulfilmedInterval;

        public static void KitchenStart(MainWindow mainWindow, int fulfilmedInt, CancellationToken token)
        {
            Window = mainWindow;
            fulfilmedInterval = fulfilmedInt;
            queueOfTickets = new Queue<Task<int>>();

            
            Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (queueOfTickets.Count > 0)
                    {
                        Task ticketCreating = Task.Factory.StartNew(() => PreparingOrder(), TaskCreationOptions.AttachedToParent);
                        ticketCreating.Wait();
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Window.Dispatcher.Invoke(() =>
                        {
                            Window.waitingOrdersCount.Content = queueOfTickets.Count;
                            if (queueOfTickets.Count > 0)
                            {
                                Window.tb_orders.Text = GetAllOrdersInKitchen();
                            }
                            else Window.tb_orders.Text = "";
                        }
                    );
                    
                }
            });
        }

        public static void GetNewTicket(Task<int> newTicket)
        {
            queueOfTickets.Enqueue(newTicket);
        }

        public static void PreparingOrder()
        {
            if (queueOfTickets.Count > 0)
            {
                Task<int> ticket = queueOfTickets.Dequeue();
                Window.Dispatcher.Invoke(() => Window.currentOrder.Content = ticket.Result);
                Thread.Sleep(fulfilmedInterval);

                Task<int> preparedTicket = ticket.ContinueWith<int>(order =>
                {
                    Console.WriteLine(order.Result + " успешно приготовлен!");
                    return order.Result;
                });

                Server.GetPreparedOrder(preparedTicket);
            }
        }

        private static string GetAllOrdersInKitchen()
        {
            string list = "";
            Task<int>[] collection = new Task<int>[queueOfTickets.Count];
            queueOfTickets.CopyTo(collection, 0);
            for (int i = 0; i < collection.Length; ++i)
            {
                list += $"{collection[i].Result} waiting\n";
            }
            return list;
        }
    }
}
