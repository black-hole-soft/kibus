using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Kibus4
{
    public partial class Presentacion : Form
    {
        public Presentacion()
        {
            InitializeComponent();
        }
        private void Presentacion_Load(object sender, EventArgs e)
        {
            
        }
        private void clickMouse(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
