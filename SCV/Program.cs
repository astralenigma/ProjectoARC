using BibliotecaDeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SCV
{
    class Program
    {
        static int nmrPartidos;
        static int nmrVotosBrancos;
        static int[] contagemVotos;
        const int PORTA = 6000;

        static void Main(string[] args)
        {
            inicializacao();
            //Ligação ao SRE
            conectar("127.0.0.1");//Estou a começar a ver problemas nisto. TRV vai provavelmente entrar em conflicto. Mudar portas?

            
            //Ligações dos TRVs


        }

        //Método de incrementação de votos em branco.
        static void incrementarVotoBranco()
        {
            nmrVotosBrancos++;
        }

        //Método de inicialização das variáveis do servidor.
        static void inicializacao()
        {
            nmrPartidos = 10;//no futuro talvez receba o número de partidos na linha de comandos
            nmrVotosBrancos = 0;
            contagemVotos = new int[nmrPartidos];
            for (int i = 0; i < nmrPartidos; i++)
            {
                contagemVotos[i] = 0;
            }
        }

        //Método de incrementação de votos de partido
        static void incrementarVotoPartido(int partido)
        {
            contagemVotos[partido]++;
        }

        //Método de testes da praxe.
        static void teste() { 

        }

        //Método conecção
        static Socket conectar(String ipStr)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipad = IPAddress.Parse(ipStr);
            IPEndPoint ip = new IPEndPoint(ipad, PORTA);

            socket.Connect(ip);

            return socket;
        }

    }
}
