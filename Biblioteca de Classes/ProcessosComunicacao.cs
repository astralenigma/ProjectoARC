using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BibliotecaDeClasses
{
    public class ProcessosComunicacao
    {
        Socket socket;
        
       public ProcessosComunicacao(Socket socket)
        {
            this.socket = socket;
        }

        public void enviarMensagens()
        {
            string mensagem = "";
            do
            {
                mensagem = enviarMensagem(Console.ReadLine());
            } while (mensagem != "exit");

        }

        public String enviarMensagem(string mensagem)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(mensagem);
            socket.Send(data);
            return mensagem;
        }

        public void receberMensagens()
        {
            do
            {
                receberMensagem();
            } while (socket.Connected);
        }

        public String receberMensagem()
        {
            byte[] data = new byte[1024];
            socket.Receive(data);
            string mensagemRecebida = Encoding.ASCII.GetString(data);
            mensagemRecebida=mensagemRecebida.Replace("\0", "");
            return mensagemRecebida;
        }

        public EndPoint remoteEndPoint()
        {
            return socket.RemoteEndPoint;
        }

        public string getOwnIP()
        {
            IPHostEntry host= Dns.GetHostEntry(Dns.GetHostName());
            string localIP = "?";
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }
}
