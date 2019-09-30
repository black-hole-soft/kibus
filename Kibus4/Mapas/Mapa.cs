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
    class Mapa
    {
        public String nom;
        public String[,] elem = new String[21, 15];
        public Point casa;
        public Point kibus;
        public Mapa(String n)
        {
            nom = n;
        }
    }
}
