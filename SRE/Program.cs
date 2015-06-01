using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
namespace SRE
{
    class Program
    {
        private static List<String> lista;
        private static Socket socket;
        static void Main(string[] args)
        {
            inicializacao();
            estabelecerConeccao();
        }

        //Método que inicializa a lista, e mete o servidor a ouvir.
        private static void inicializacao()
        {
            leitorEleitores();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6000);
            socket.Bind(ip);
            socket.Listen(10);
        }

        //Método que Lê o ficheiro de texto.
        static void leitorEleitores()
        {
            string line;
            lista = new List<String>();
            try
            {
                StreamReader file = new StreamReader(@"lista.txt");
                while ((line = file.ReadLine()) != null)
                {
                    lista.Add(line);
                }
                file.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("A lista não foi encontrada, Servidor será desligado.");
                Console.ReadLine();
                Environment.Exit(0);
            }
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
            //Verificar as excepções que este método lança.
            File.WriteAllText(@"lista.txt", texto);
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

            Console.WriteLine("Waiting...");
            Socket socket2 = socket.Accept();

            EndPoint ipep = socket2.RemoteEndPoint;
            Console.WriteLine("Client " + ipep + " Connectado.\n");
            return socket2;
        }

        //Método de receber o BI para confirmação
        static void confirmacaoBIconeccao(Socket socket)
        {
            byte[] data = new byte[1024];
            try
            {
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
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    Console.WriteLine("Conecção perdida tentando restabelecer conecção.");
                    estabelecerConeccao();
                }
            }
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

        //Método para estabelecer e restabelecer a connecção.
        static void estabelecerConeccao()
        {
            Socket socket = esperandoLigacao();
            receberBIConeccaoCiclo(socket);
        }
    }
}
