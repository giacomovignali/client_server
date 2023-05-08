using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
using System.Windows.Threading;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress local_address = IPAddress.Parse("127.0.0.1");
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 10000);
            lbl_ip.Content = "IP server: " + local_endpoint;

            socket.Bind(local_endpoint);

            socket.Blocking = false;
            socket.EnableBroadcast = true;

            dTimer = new DispatcherTimer();

            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dTimer.Start();

        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;

            if ((nBytes = socket.Available) > 0)
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];

                EndPoint remoreEndPoint = new IPEndPoint(IPAddress.Any, 0);

                nBytes = socket.ReceiveFrom(buffer, ref remoreEndPoint);

                string from = ((IPEndPoint)remoreEndPoint).Address.ToString();

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);


                lstMessaggi.Items.Add(from+": "+messaggio);

            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse(txtTo.Text);

            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 12000);

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            socket.SendTo(messaggio, remote_endpoint);

            lstInviati.Items.Add(txtTo.Text + " " + txtMessaggio.Text);
        }

        private void btn_elimina_Copy_Click(object sender, RoutedEventArgs e)
        {
            lstInviati.Items.Clear();
        }

        
    }
}
