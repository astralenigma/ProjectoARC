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
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //NetworkStream serverStream;

        public Form1()
        {
            InitializeComponent();
            inicializacaoDosPartidos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Código onde recebe o voto escolhido.
            clientSocket.Connect("127.0.0.1", 8888);
            //Não gosto do facto de estar a usar uma string para meter o IP em vez de algo 
            //mais automático com menos necessidade de um programador a fuçar no código
            
            //Aqui ele envia a informação do utilizador.
            String informacao = textBox1.Text + " " + comboBox1.SelectedItem;
            byte[] mensagemAEnviar = System.Text.Encoding.ASCII.GetBytes(informacao);
            clientSocket.Send(mensagemAEnviar);

            //Aqui ele recebe a resposta não quer dizer que ele goste dela.
            byte[] mensagemAReceber = new byte[1024];
            clientSocket.Receive(mensagemAReceber);
            string returndata = System.Text.Encoding.ASCII.GetString(mensagemAReceber);
            
            //Apagar o input e mostrar o resultado.
            label3.Text = returndata;
            limparCampos();
            //Uso duvidoso do operador NOT. Pelo menos ele não está a usar um método toggle. Esquece.
            toggleVisibilidade();
            clientSocket.Disconnect(true);
        }
        /*Código onde os partidos são adicionados à interface.*/
        private void inicializacaoDosPartidos()
        {
            int nmrPartidos = 9;
            for (int i = 0; i <= nmrPartidos; i++)
            {
                comboBox1.Items.Add(i);
            }
            
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

        //Método que limpa os campos de input
        private void limparCampos()
        {
            textBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
