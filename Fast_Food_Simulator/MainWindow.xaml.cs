using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fast_Food_Simulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task Simulation;
        private CancellationTokenSource CancelTokenSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            int custInt = 0;
            int fulfilmedInt = 0;

            if (!int.TryParse(this.customerInterval.Text, out custInt) || !int.TryParse(this.fulfilmedInterval.Text, out fulfilmedInt))
            {
                MessageBox.Show("The customer arrival interval or the order fulfilment interval is incorrectly entered.");
            }
            else
            {
                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                this.CancelTokenSource = cancelTokenSource;
                CancellationToken token = cancelTokenSource.Token;
                Simulation = Task.Run(() =>
                {
                    OrderTaker.OrderTakerStart(this, custInt, token);
                    Kitchen.KitchenStart(this, fulfilmedInt, token);
                    Server.ServerStart(this, token);
                });
                ((Button)sender).IsEnabled = false;
                btn_stop.IsEnabled = true;                
            }
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            CancelTokenSource.Cancel();
            Simulation.ConfigureAwait(true).GetAwaiter().GetResult();
            Simulation.Dispose();
            ((Button)sender).IsEnabled = false;
            btn_start.IsEnabled = true;
        }
    }
}
