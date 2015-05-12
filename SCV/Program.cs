using BibliotecaDeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            ProcessosComunicacao oPC = new ProcessosComunicacao(conectar("127.0.0.1"));//Estou a começar a ver problemas nisto. TRV vai provavelmente entrar em conflicto. Mudar portas?
            if (oPC.receberMensagem() == "OK")
                Console.WriteLine("Ligação Bem sucedida");
            //teste(oPC);

            //Ligações dos TRVs
            //A partir desta linha os detalhes de funcionamento do código passam para além do meu conhecimento.
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            //A partir daqui começa a fazer algum sentido
            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                handleTRV client = new handleTRV();//Classe criada só para funcionar com o código do qual não entendo nada
                client.startClient(clientSocket,oPC);//Loucura de código.
            }

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

        //Método conecção
        static Socket conectar(String ipStr)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipad = IPAddress.Parse(ipStr);
            IPEndPoint ip = new IPEndPoint(ipad, PORTA);

            socket.Connect(ip);

            return socket;
        }

        public class handleTRV
        {
            TcpClient clientSocket;
            ProcessosComunicacao cliPC;
            public void startClient(TcpClient inClientSocket, ProcessosComunicacao cliPC)
            {
                this.clientSocket = inClientSocket;
                this.cliPC = cliPC;

                Thread ctThread = new Thread(doVoto);
                ctThread.Start();
            }

            private void doVoto()
            {

                Boolean erro = false;
                try
                {
                    do
                    {
                        
                        cliPC.enviarMensagem("");
                        erro = false;
                        int switch_on=0;//placeholder for amazing things.
                        switch (switch_on)
                        {
                            //Se o BI falhar mandar aviso
                            case 1://ESSE BI NÃO EXISTE.
                            //Se o BI já tiver sido usado mandar aviso
                            case 2://ESSE BI JÁ FOI USADO.
                            //Se tudo funcionar mandar que está tudo bem.
                            default://ESTÁ TUDO A FUNCIONAR MAS NÃO ME CULPES A MIM.
                                break;
                        }
 
                    } while (erro);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }

        private static void testeDeLigacaoSRE(ProcessosComunicacao oPC)
        {
            oPC.enviarMensagem(" ");
            Console.WriteLine(oPC.receberMensagem());
            oPC.enviarMensagem("1113441241");
            Console.WriteLine(oPC.receberMensagem());
            oPC.enviarMensagem("1113441241");
            Console.WriteLine(oPC.receberMensagem());
        }

        //Método de testes da praxe.
        static void teste(ProcessosComunicacao oPC)
        {
            testeDeLigacaoSRE(oPC);
            Console.ReadLine();
        }
    }
}
