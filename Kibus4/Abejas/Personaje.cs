using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Kibus4
{
    class Personaje
    {
        internal PictureBox mono;

        internal static int slp, vel = 50, re, nMov;
        internal static Board[] board = new Board[5];
        internal static bool go = false, maxEpoca = false, found = false;
        internal bool arribe = false, moving = false;

        internal static Random ran = new Random();

        internal String ColDir = "", ColDirAnt = "";

        internal Point p, aux, memoria;

        internal String dir;

        internal Personaje()
        {
            slp = 30;
            vel = 50;
        }
        internal void velocidad(int v)
        {
            slp = v;
        }
    }
}
