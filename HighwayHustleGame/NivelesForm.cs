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
    public partial class NivelesForm : Form
    {
        public NivelesForm()
        {
            InitializeComponent();
        }

        private void btnLvl1_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial("Historia", 1);
            formInicial.Show();
            this.Close();
        }
        private void btnLvl2_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial("Historia", 2);
            formInicial.Show();
            this.Close();
        }
        private void btnLvl3_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial("Historia", 3);
            formInicial.Show();
            this.Close();
        }
        private void btnLvl4_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial("Historia", 4);
            formInicial.Show();
            this.Close();
        }
        private void btnLvl5_Click(object sender, EventArgs e)
        {
            FormInicial formInicial = new FormInicial("Historia", 5);
            formInicial.Show();
            this.Close();
        }

        private void btnMenuPrincipal_Click(object sender, EventArgs e)
        {
            //abrir el Form de menu principal y cerrar este Form
            MenuInicial menuInicial = new MenuInicial();
            menuInicial.Show();
            this.Close();
        }
    }
}
