using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast_Food_Simulator
{
    public class Customer
    {
        public int numberOfTicket;
        public bool IsTicketCreated = false;

        public Customer(Queue<Customer> queue)
        {
            Task.Factory.StartNew(() =>
            {
                while (!IsTicketCreated) ;
                GoingToServingLine();
            }, TaskCreationOptions.AttachedToParent);
        }

        public void GoingToServingLine()
        {
            Server.AddNewCustomerToQueue(this);
        }
    }
}
