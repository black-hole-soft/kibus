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
    public partial class Epocas : Form
    {
        Kibus gm;
        public Epocas(Kibus k)
        {
            gm = k;
            InitializeComponent();
        }
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            gm.epoks = (int)numEpks.Value;
            this.Close();
        }
        private void Epocas_Load(object sender, EventArgs e)
        {
            btnEnviar.Focus();
        }
        private void numEpks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                gm.epoks = (int)numEpks.Value;
                this.Close();
            }
            if (e.KeyChar == 27)
                this.Close();
        }
    }
}
