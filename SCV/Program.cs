using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCV
{
    class Program
    {
        static int nmrPartidos;
        static int nmrVotosBrancos;
        static int[] contagemVotos;

        static void Main(string[] args)
        {
            inicializacao();
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

    }
}
