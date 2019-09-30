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
    class Moves
    {
        internal Point move;
        internal String dir;
        internal Moves(Point m, String d)
        {
            move = m;
            dir = d;
        }
    }
}
