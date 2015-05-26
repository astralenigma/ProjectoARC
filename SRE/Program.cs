using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SRE
{
    class Program
    {
        private static List<String> lista;
        static void Main(string[] args)
        {
            inicializacao();
            //teste();
            Socket socket = esperandoLigacao();
            receberBIConeccaoCiclo(socket);
        }

        private static void inicializacao()
        {
            leitorEleitores();
        }


        //Método que Lê o ficheiro de texto.
        static void leitorEleitores()
        {
            string line;
            lista = new List<String>();
            System.IO.StreamReader file = new System.IO.StreamReader(@"lista.txt");
            while ((line = file.ReadLine()) != null)
            {
                lista.Add(line);
            }
            file.Close();
        }

        //Metodo que altera o ficheiro de texto.
        static void escritorEleitores(String votador)
        {
            String texto = "";
            foreach (String eleitor in lista)
            {
                String[] separador = eleitor.Split(' ');

                texto += separador[0] + " ";

                if (separador[0] == votador)
                {
                    texto += "1";
                }
                else
                {
                    texto += separador[1];
                }
                texto += "\n";
            }
            System.IO.File.WriteAllText(@"C:\Users\Rui\Documents\Visual Studio 2013\Projects\ProjectoARC\SRE\lista.txt", texto);
            leitorEleitores();
        }

        //Metodo de verificacao de votacao
        static int votando(String bi)
        {
            foreach (String eleitor in lista)
            {
                if (bi == eleitor.Split(' ')[0])
                {
                    if (eleitor.Split(' ')[1] == "0")
                    {
                        escritorEleitores(bi);
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return 2;
        }

        //Método que espera pelas ligações.
        static Socket esperandoLigacao()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6000);
            socket.Bind(ip);
            socket.Listen(1);
            Console.WriteLine("Waiting...");
            Socket socket2 = socket.Accept();

            EndPoint ipep = socket2.RemoteEndPoint;
            Console.WriteLine("Client " + ipep + " Connectado.\n");

            mensageDeConfirmacaoCliente(socket2);
            return socket2;
        }

        //Método de receber o BI para confirmação
        static void confirmacaoBIconeccao(Socket socket)
        {
            byte[] data = new byte[1024];
            socket.Receive(data);
            string mensagemRecebida = Encoding.ASCII.GetString(data);
            mensagemRecebida = mensagemRecebida.Replace("\0", "");//Limpar os bits nulos.
            switch (votando(mensagemRecebida))
            {
                case 0:
                    data = Encoding.ASCII.GetBytes("Ok");
                    Console.WriteLine("Ok");//check
                    break;
                case 1:
                    data = Encoding.ASCII.GetBytes("BI Usado");
                    Console.WriteLine("BI Usado");//check
                    break;
                case 2:
                    data = Encoding.ASCII.GetBytes("BI Nao Encontrado");
                    Console.WriteLine("BI Nao Encontrado");//check
                    break;
                
            }
            socket.Send(data);
        }

        //Método do ciclo da recepção das mensagens.
        static void receberBIConeccaoCiclo(Socket socket)
        {
            do
            {
                confirmacaoBIconeccao(socket);
            } while (socket.Connected); //This will never happen due to circunstances completely outside my power and knowledge.
            Console.WriteLine("Cliente " + socket.RemoteEndPoint + " disconectado.\n");
        }

        //Método de testes
        static void teste()
        {
            leitorEleitores();
            imprimirEleitores();
            votando("1113441241");
            //escritorEleitores("1113441241");
            imprimirEleitores();
            votando("1113441241");
            //escritorEleitores("1113441241");
            imprimirEleitores();

        }

        //Método controverso no seu uso.
        static void mensageDeConfirmacaoCliente(Socket socket)
        {
            byte[] data = new byte[1024];
            string mensagemEnviada = "Ok";
            data = Encoding.ASCII.GetBytes(mensagemEnviada);
            socket.Send(data);
        }

        //Método que imprime a lista no ecrã usado apenas para debug
        static void imprimirEleitores()
        {
            foreach (String eleitor in lista)
            {
                System.Console.WriteLine(eleitor);
            }
            Console.ReadLine();
        }

    }
}
