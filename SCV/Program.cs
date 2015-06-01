using BibliotecaDeClasses;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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
            //Verificar se existe um backup primeiro.

            //inicialização da contagem de votos.
            inicializacao(10);
            //Ligação ao SRE
            ProcessosComunicacao oPC = iniciarPC("127.0.0.1");
            Console.WriteLine("Ligação Bem sucedida.\n" +
                "Conectado ao Servidor de Recenseamento Eleitoral em " + oPC.remoteEndPoint() + ".");

            //Ligações dos TRVs
            //A partir desta linha os detalhes de funcionamento do código passam para além do meu conhecimento.
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8888);
            serverSocket.Bind(ip);
            serverSocket.Listen(10);
            //A partir daqui começa a fazer algum sentido.
            while (true)
            {
                handleTRV client = new handleTRV();//Classe criada só para funcionar com o código do qual não entendo nada, acho piada o facto de já ter alterado o código tanto que já nem deve de fazer a mesma coisa.
                Socket cliSock = serverSocket.Accept();
                ProcessosComunicacao cliPC = new ProcessosComunicacao(cliSock);//Eu fiz esta classe muito mais robusta do que pensava.
                cliPC.enviarMensagem(nmrPartidos.ToString());
                client.startClient(cliPC, oPC);//Loucura de código. Loucura mesmo já me obrigou a trocar de lugares e tudo 2X ou pelo menos é a segunda que me lembro.
                Console.WriteLine("Cliente recebido.");
            }
        }

        //Método de inicialização das variáveis do servidor.
        static void inicializacao(int inNmrPartidos)
        {
            nmrPartidos = inNmrPartidos;
            nmrVotosBrancos = 0;
            contagemVotos = new int[nmrPartidos];
            for (int i = 0; i < nmrPartidos; i++)
            {
                contagemVotos[i] = 0;
            }
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

        //Método porque PC é Rei, PC é Amor, PC é Deus.
        static ProcessosComunicacao iniciarPC(string ip)
        {
            try
            {
                return new ProcessosComunicacao(conectar(ip));
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)
                {
                    Console.WriteLine("Conecção ao servidor recusada, pressione qualquer tecla para continuar ou 's' para sair ");
                    if (Console.ReadKey(true).KeyChar.ToString().ToLower() == "s")
                        Environment.Exit(0);
                }
                return iniciarPC(ip);
            }
        }

        //Método de incrementação de votos em branco.
        static void incrementarVotoBranco()
        {
            nmrVotosBrancos++;
        }

        //Método de incrementação de votos de partido
        static void incrementarVotoPartido(int partido)
        {
            if (partido == 0)
            {
                incrementarVotoBranco();
            }
            else
            {
                contagemVotos[partido]++;
            }
            //Gravar o voto, para o caso da energia falhar.
            guardarBackup();

        }

        private static void guardarBackup()
        {
            String texto = "Branco " + nmrVotosBrancos;
            for (int i = 1; i <= nmrPartidos; i++)
            {
                texto += "Partido " + i + " " + contagemVotos[i - 1] + "\n";
            }
            File.WriteAllText(@"backup.txt", texto);
        }

        //Classe das operações do TRV.
        private class handleTRV
        {
            ProcessosComunicacao cliPC;
            ProcessosComunicacao srePC;

            public void startClient(ProcessosComunicacao incliPC, ProcessosComunicacao insrePC)
            {
                this.cliPC = incliPC;
                this.srePC = insrePC;

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
                        String[] mensagem = cliPC.receberMensagem().Split(' ');

                        srePC.enviarMensagem(mensagem[0]);
                        erro = accaoDependeSRE(srePC.receberMensagem());
                    } while (erro);

                }
                catch (TRVCaiuException)
                {
                    Console.WriteLine("O cliente desconectou-se.");
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054)
                    {
                        Console.WriteLine("Desconectado do SRE.");
                        cliPC.enviarMensagem("4");
                    }

                }
            }

            private bool accaoDependeSRE(string mensagem)
            {
                try
                {
                    switch (mensagem)
                    {
                        //Se o BI falhar mandar aviso
                        case "BI Nao Encontrado"://ESSE BI NÃO EXISTE.

                            cliPC.enviarMensagem("1");
                            return true;
                        //Se o BI já tiver sido usado mandar aviso
                        case "BI Usado"://ESSE BI JÁ FOI USADO.

                            cliPC.enviarMensagem("2");
                            return true;
                        //Se tudo funcionar mandar que está tudo bem.
                        default://ESTÁ TUDO A FUNCIONAR MAS NÃO ME CULPES A MIM.
                            incrementarVotoPartido(Convert.ToInt32(mensagem[1]));
                            cliPC.enviarMensagem("3");
                            return false;
                    }
                }
                catch (SocketException)
                {
                    throw new TRVCaiuException();
                }
            }
        }
    }
}
