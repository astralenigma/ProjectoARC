using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRE
{
    class Program
    {
        private static List<String> lista;
        static void Main(string[] args)
        {
            teste();
        }


        //Método que Lê o ficheiro de texto.
        static void leitorEleitores()
        {
            string line;
            lista=new List<String>();
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Rui\Documents\Visual Studio 2013\Projects\ProjectoARC\SRE\lista.txt");
            while ((line = file.ReadLine()) != null)
            {
                lista.Add(line);
            }
            file.Close();
        }

        //Metodo que altera o ficheiro de texto.
        static void escritorEleitores(String votador)
        {
            String texto="";
            foreach (String eleitor in lista)
            {
                String[] separador = eleitor.Split(' ');

                texto += separador[0]+" ";

                if (separador[0] == votador)
                {
                    texto += "1";
                }
                else {
                    texto+= separador[1];
                }
                texto += "\n";
            }
            System.IO.File.WriteAllText(@"C:\Users\Rui\Documents\Visual Studio 2013\Projects\ProjectoARC\SRE\lista.txt", texto);
            leitorEleitores();
        }

        //Metodo de verificacao de votacao
        static void votando(String bi)
        {
            foreach (String eleitor in lista)
            {
                if(bi == eleitor.Split(' ')[0]){
                    if (eleitor.Split(' ')[1] == "0")
                    {
                        escritorEleitores(bi);
                    }
                }
            }
        }

        //Método que imprime a lista no ecrã
        static void imprimirEleitores()
        {
            foreach (String eleitor in lista)
            {
                System.Console.WriteLine(eleitor);
            }
            Console.ReadLine();
        }

        //Método de testes
        static void teste() {
            leitorEleitores();
            imprimirEleitores();
            votando("1113441241");
            //escritorEleitores("1113441241");
            imprimirEleitores();
            votando("1113441241");
            //escritorEleitores("1113441241");
            imprimirEleitores();
        }
    }
}
