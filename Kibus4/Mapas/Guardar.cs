using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kibus4
{
    public partial class Guardar : Form
    {
        internal Kibus kib;
        internal Guardar(Kibus k)
        {
            kib = k;
            InitializeComponent();
        }
        private void guardar()
        {
            if (txbNombre.Text != "")
            {
                kib.lista.dgMapas.Hide();
                kib.lista.SalvarArchivos(kib.grid, txbNombre.Text);
                kib.Controls.Add(kib.lista.dgMapas);
                kib.lista.dgMapas.BringToFront();
                kib.lista.dgMapas.Show();
                this.Close();
            }
        }
        internal void btnGuardar_Click(object sender, EventArgs e)
        {
            guardar();
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txbNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                guardar();
            if (e.KeyChar == 27)
                this.Close();
        }
    }
}
