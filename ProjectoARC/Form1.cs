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

            byte[] inStream = new byte[1024];
            serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            
            //Apagar o input e mostrar o resultado.
            label3.Text = returndata;
            //Uso duvidoso do operador NOT. Pelo menos ele não está a usar um método toggle. Esquece.
            toggleVisibilidade();
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

        //Método do botão da mensagem.
        private void button2_Click(object sender, EventArgs e)
        {
            toggleVisibilidade();
        }

        //Método que troca a visiblidade das mensagens. Eu tentei impedir isto mas cada vez mais soou como uma boa ideia.
        private void toggleVisibilidade()
        {
            label3.Visible = !label3.Visible;
            button2.Visible = !button2.Visible;
            button1.Visible = !button1.Visible;
            comboBox1.Visible = !comboBox1.Visible;
            textBox1.Visible = !textBox1.Visible;
            label1.Visible = !label1.Visible;
            label2.Visible = !label2.Visible;
        }
    }
}
