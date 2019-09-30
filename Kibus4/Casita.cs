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
    class Casita
    {
        internal PictureBox casa;
        internal int x, y;

        internal Casita(int xi, int yi)
        {
            x = xi;
            y = yi;
            casa = new PictureBox();
            casa.Location = new Point(x * 50 + 205, y * 50 + 15);
            casa.Size = new Size(50, 50);
            casa.Image = Image.FromFile("Resources\\Cabana.png");
            casa.Tag = "Cabana";
        }
        delegate void DelHideCasa();
        internal void HideCasa()
        {
            if (casa.InvokeRequired)
                casa.Invoke(new DelHideCasa(HideCasa));
            else
                casa.Hide();
        }
    }
}