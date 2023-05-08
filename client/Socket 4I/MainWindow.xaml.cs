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
            //creo la socket, ovvero il canale di comunicazione tra client e server
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //definisco l'indirizzo ip del client
            IPAddress local_address = IPAddress.Parse("127.0.0.1");

            //definisco l'indirizzo ip e la sua porta
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 12000);

            //definisco l'associazione tra la socket al local_point
            socket.Bind(local_endpoint);
            //imposto a false il blocco della socket
            socket.Blocking = false;
            //imposto a true la ricezioni/invio dei messaggi
            socket.EnableBroadcast = true;

            //stampo nella label l'indirizzo ip del client
            lbl_ip.Content = "IP client: " + local_endpoint;


            

            
            //imposto l'intervallo di tempo
            dTimer = new DispatcherTimer();
            //ad ogni intervallo viene eseguito il metodo aggiornamento_dTimer
            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            //imposto la durata del timer a 250 millisecondi
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            //avvio il timer
            dTimer.Start();

        }

        
        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            
            int nBytes = 0;
            //controllo
            if ((nBytes = socket.Available) > 0)
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];
                //viene passato l'indirizzo di rete
                EndPoint remoreEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //lunghezza in byte del messaggio
                nBytes = socket.ReceiveFrom(buffer, ref remoreEndPoint);
                //indirizzo ip del mittente
                string from = ((IPEndPoint)remoreEndPoint).Address.ToString();
                //trascizione il buffer del messaggio ricevuto
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                
                lstMessaggi.Items.Add(from+": "+messaggio);

            }
        }

        //metodo del bottone invia per la gestione dei messaggi da inviare
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //traduzione dell'indirizzo ip del destinatario
            IPAddress remote_address = IPAddress.Parse(txtTo.Text);
            //associo l'indirizzo ip del destinatario con la porta da utilizzare
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 10000);
            //conneto la socket all'indirizzo ip del destinatario
            socket.Connect(remote_endpoint);
            //traduzione del messaggio in byte
            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            //invio del messaggio al destinatario tramite l'utilizzo di remote_endpoint
            socket.SendTo(messaggio, remote_endpoint);
            //aggiungo i messaggi inviati nella listbox dei meessaggi inviati
            list_inviati.Items.Add(txtTo.Text + " " + txtMessaggio.Text);
        }

        //metodo per pulire la listBox
        private void btn_elimina_Click(object sender, RoutedEventArgs e)
        {
            list_inviati.Items.Clear();
        }
    }
}
