using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectoARC
{
    public partial class Form1 : Form
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;

        public Form1()
        {
            InitializeComponent();
            inicializacaoDosPartidos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Código onde recebe o voto escolhido.
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Message from Client$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inStream = new byte[1025];
            serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            
        }
        /*Código onde os partidos são adicionados à interface.*/
        private void inicializacaoDosPartidos()
        {
            int nmrPartidos = 10;
            for (int i = 0; i <= nmrPartidos; i++)
            {
                comboBox1.Items.Add(i);   
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clientSocket.Connect("127.0.0.1", 8888);

        }
    }
}
