using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Kibus4
{
    class Vertice
    {
        internal Arista[] cnx = new Arista[9];
        internal bool nv = true;
        internal Point p;
        internal Vertice(Point p)
        {
            this.p = p;
        }
    }
}
