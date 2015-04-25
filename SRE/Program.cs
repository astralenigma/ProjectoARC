using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRE
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (String eleitor in leitorEleitores())
            //{
            //    System.Console.WriteLine(eleitor);
            //}
            //Console.ReadLine();
        }

        static List<String> leitorEleitores()
        {
            List<String> lista=new List<String>();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Rui\Documents\Visual Studio 2013\Projects\ProjectoARC\SRE\lista.txt");
            while ((line = file.ReadLine()) != null)
            {
                lista.Add(line);
            }
            file.Close();
            return lista;
        }
    }
}
