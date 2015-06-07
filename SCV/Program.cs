using BibliotecaDeClasses;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace SCV
{
    class Program
    {
        static int nmrPartidos;
        static int nmrVotosBrancos;
        static int[] contagemVotos;
        private static ProcessosComunicacao oPC;
        private static Socket serverSocket;
        const int PORTASRE = 6000;
        const int PORTATRV = 8888;

        static void Main(string[] args)
        {
            //Verificar se existe um backup primeiro.
            Console.WriteLine("Verificando se é a primeira vez que o servidor está a correr...");
            try
            {
                carregarBackup();
            }
            catch (Exception)
            {
                Console.WriteLine("Servidor correndo pela primeira vez começando inicialização");
                Console.WriteLine("Insira o número de partidos a concorrer nas eleições.");
                //inicialização da contagem de votos.
                nmrPartidos = Convert.ToInt32(Console.ReadLine());
                inicializacao(nmrPartidos);
            }
            //Ligação ao SRE
            estabelecerLigacaoSRE();

            //Ligações dos TRVs
            //O iniciado o servidor para ouvir os TRVs.
            esperandoPorUmTRV();
            //Os TRVs são aceites.
            aceitarTRVs();
            
            
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

        //Método para estabelecer ligacao ao SRE.
        private static void estabelecerLigacaoSRE()
        {
            oPC = iniciarPC("127.0.0.1");
            Console.WriteLine("Ligação Bem sucedida.\n" +
                "Conectado ao Servidor de Recenseamento Eleitoral em " + oPC.getRemoteEndPoint() + ".");
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

        //Método de conecção com o SRE
        static Socket conectar(String ipStr)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipad = IPAddress.Parse(ipStr);
            IPEndPoint ip = new IPEndPoint(ipad, PORTASRE);

            socket.Connect(ip);

            return socket;
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
                contagemVotos[partido-1]++;
            }
            //Gravar o voto, para o caso da energia falhar.
            guardarBackup();

        }

        //Método de incrementação de votos em branco.
        private static void incrementarVotoBranco()
        {
            nmrVotosBrancos++;
        }

        //Método para guardar o backup dos votos.
        private static void guardarBackup()
        {
            String texto = nmrVotosBrancos + "\n";
            for (int i = 0; i < nmrPartidos; i++)
            {
                texto += contagemVotos[i] + "\n";
            }
            File.WriteAllText(@"backup.txt", texto);
        }

        //Método para carregar o backup dos votos.
        private static void carregarBackup()
        {
            List<int> lista=new List<int>();
            StreamReader file = new StreamReader(@"backup.txt");
            nmrVotosBrancos=Convert.ToInt32(file.ReadLine());
            string line = "";

            while ((line = file.ReadLine()) != null)
            {
                lista.Add(Convert.ToInt32(line));
            }
            file.Close();
            contagemVotos = lista.ToArray();
            nmrPartidos = contagemVotos.Length;
        }

        //Eu preciso de um TRV, eu estou à espera de um TRV pelo fim da noite.
        private static void esperandoPorUmTRV()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORTATRV);
            serverSocket.Bind(ip);
            serverSocket.Listen(10);
        }

        //Método que aceita e distribui TRVs por threads.
        private static void aceitarTRVs(){
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

                //Boolean erro = false;
                try
                {
                    do
                    {
                        String[] mensagem = cliPC.receberMensagem().Split(' ');

                        srePC.enviarMensagem(mensagem[0]);
                        /*erro =*/ accaoDependeSRE(srePC.receberMensagem(),mensagem[1]);
                    } while (true);

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

            private bool accaoDependeSRE(string respostaSRE,string mensagem)
            {
                try
                {
                    switch (respostaSRE)
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
                            incrementarVotoPartido(Convert.ToInt32(mensagem));
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
