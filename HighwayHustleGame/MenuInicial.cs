using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HighwayHustleGame
{
    public partial class MenuInicial : Form
    {
        public MenuInicial()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHistoria_Click(object sender, EventArgs e)
        {
            //Abrir el NivelesForm y cerrar este Form
            NivelesForm nivelesForm = new NivelesForm();
            nivelesForm.Show();
            this.Hide();
        }

        private void btnDesafio_Click(object sender, EventArgs e)
        {
            
            FormInicial formInicial = new FormInicial("Desafio", 6);
            formInicial.Show();
            this.Hide();
        }
    }
}
