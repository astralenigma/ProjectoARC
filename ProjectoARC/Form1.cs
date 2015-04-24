using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectoARC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            inicializacaoDosPartidos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Código onde recebe o voto escolhido.
        }

        private void inicializacaoDosPartidos()
        {
            int nmrPartidos = 10;
            for (int i = 0; i <= nmrPartidos; i++)
            {
                comboBox1.Items.Add(i);   
            }
            
        }
    }
}
