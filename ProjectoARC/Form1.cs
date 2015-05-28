using BibliotecaDeClasses;
using System;
using System.Net.Sockets;
using System.Windows.Forms;
namespace ProjectoARC
{
    public partial class Form1 : Form
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ProcessosComunicacao oPC;
        public Form1()
        {
            InitializeComponent();
            inicializacaoDosPartidos();
            oPC = new ProcessosComunicacao(clientSocket);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Código onde recebe o voto escolhido.

            //Aqui ele envia a informação do utilizador.
            oPC.enviarMensagem(textBox1.Text + " " + comboBox1.SelectedItem);
            
            //Aqui ele recebe a resposta não quer dizer que ele goste dela.
            mensagens(oPC.receberMensagem());
            //Apagar o input e mostrar o resultado.

            limparCampos();
            //Uso duvidoso do operador NOT. Pelo menos ele não está a usar um método toggle. Esquece.
            toggleVisibilidade();
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
        private void mensagens(String mensagemRecebida)
        {
            switch (mensagemRecebida)
            {
                case "1":
                    label3.Text = "O número do cartão do CC ou BI que enviou não está na lista.";
                    break;
                case "2":
                    label3.Text = "O número do cartão do CC ou BI que usou já foi usado.";
                    break;
                case "3":
                    label3.Text = "Votação bem sucedida.";
                    break;
                //case 4:
                //    label3.Text = returndata;
                //    break;
                //case 5:
                //    label3.Text = returndata;
                //    break;
                default:
                    label3.Text = "Erro Estranho impossível de perceber FUJAM.";
                    break;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            clientSocket.Connect("127.0.0.1", 8888);
            //Não gosto do facto de estar a usar uma string para meter o IP em vez de algo 
            //mais automático com menos necessidade de um programador a fuçar no código
        }
    }
}
