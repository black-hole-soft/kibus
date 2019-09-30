using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kibus4
{
    class Arista
    {
        internal int peso = int.MaxValue / 2;
        internal Vertice ver;
        internal bool nv = true;//No visitada
        internal int indx;
        internal Arista(Vertice v, int x)
        {
            ver = v;
            indx = x;
        }
    }
}
