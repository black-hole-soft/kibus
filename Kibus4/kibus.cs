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
    class kibus : Personaje
    {
        Thread hilo, hDetener, hBress, hBees, hTraining, hFirstMinor, hDijkstra;
        Point hs;
        bool movido = false;
        Casilla[,] grid;
        Kibus juego;
        Stack<Moves> pila = new Stack<Moves>();
         
        internal Abeja[] bees = new Abeja[5];

        List<Arista> camino = new List<Arista>();
        Vertice[,] malla = new Vertice[21, 15];
        internal bool blind = false;
        Point posIni;
        int epoks, max, min, media, xN, yN;
        bool first;

        internal bool movin = false;//Determina si lo estoy moviendo kon el teclado

        internal kibus(int x, int y, Kibus j)
        {
            juego = j;
            grid = j.grid;
            p.X = x * 50 + 205;
            p.Y = y * 50 + 15;
            mono = new PictureBox();
            mono.Location = p;
            mono.Size = new Size(50, 50);
            mono.Image = Image.FromFile("Resources\\Moves\\F1link.png");
            mono.BackgroundImage = grid[x, y].pic.Image;
            mono.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(PreviewKeyDown);
            mono.Tag = "F1link";
            dir = "F";
            Moves move = new Moves(p, oposite(dir));
            pila.Push(move);
        }
        internal void iniciaHilo()
        {
            hilo = new Thread(new ThreadStart(HiloMethod));
        }
        delegate void DelHideKibus();
        internal void HideKibus()
        {
            if (mono.InvokeRequired)
                mono.Invoke(new DelHideKibus(HideKibus));
            else
                mono.Hide();
        }
        private void HiloMethod()
        {
            while (true)
            {
                if (movido)
                {
                    moverKibus();
                    movido = false;
                    Moves move = new Moves(p, oposite(dir)), mveA = pila.Peek();
                    if (!(move.move == mveA.move && move.dir == mveA.dir))
                        pila.Push(move);
                }
                Thread.Sleep(20);
            }
        }
        private String oposite(String dir)
        {
            if (dir == "F") return "B";
            if (dir == "B") return "F";
            if (dir == "R") return "L";
            if (dir == "L") return "R";
            return "";
        }
        internal void detenerHilo()
        {
            if (hilo != null)
                hilo.Abort();
            if (hBress != null)
                hBress.Abort();
            if (hDetener != null)
                hDetener.Abort();
            if (hTraining != null)
                hTraining.Abort();
            if (hFirstMinor != null)
                hFirstMinor.Abort();
            if (hDijkstra != null)
                hDijkstra.Abort();
            detenerAbejas();
        }
        private void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            aux = p;
            if (e.KeyValue == 32 && !movin)
            {
                hilo.Start();
                movin = true;
            }
            else
            {
                if (!movido)
                {
                    if (e.KeyValue == 37)//Left
                    {
                        dir = "L";
                        p.X -= vel;
                    }
                    if (e.KeyValue == 39)//Right
                    {
                        dir = "R";
                        p.X += vel;
                    }
                    if (e.KeyValue == 38)//Up
                    {
                        dir = "B";
                        p.Y -= vel;
                    }
                    if (e.KeyValue == 40)//Down
                    {
                        dir = "F";
                        p.Y += vel;
                    }
                    movido = true;
                }
            }
        }
        internal void regresarKibus()
        {
            hDetener = new Thread(new ThreadStart(hiloRegresar));
            hDetener.Start();
        }
        internal void hiloRegresar()
        {
            Moves move, movX=new Moves(p, dir);
            int cont = pila.Count;
            for (int i = 0; i < cont; i++)
            {   
                move = pila.Pop();
                dir = move.dir;
                p = move.move;
                if(moverKibus() != "")
                {
                    pila.Clear();
                    pila.Push(movX);
                    return;
                }
                movX = move;
                Thread.Sleep(slp);
            }
            move = new Moves(p, oposite(dir));
            pila.Push(move);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        //------------------------------------Bresenham--------------------------------------//
        /// ///////////////////////////////////////////////////////////////////////////////////
        internal String verificaCelda(Point pA)
        {
            if (pA.X > 1205 || pA.X < 205 || pA.Y > 715 || pA.Y < 15)
                return "obstacle";
            else
            {
                int x = (pA.X - 205) / 50, y = (pA.Y - 15) / 50;
                if (grid[x, y] != null)
                    if (grid[x, y].okupada)
                        if (grid[x, y].actor != null)
                        {
                            String col = grid[x, y].roll;
                            if (col == "sTree" || col == "sRock" || col == "yflag")
                                return "obstacle";
                            if (col == "eflag")
                                return "eflag";
                        }
            }
            return "";
        }
        internal void Bresenham(int Hx, int Hy)
        {
            hs.X = Hx;
            hs.Y = Hy;
            hBress = new Thread(new ThreadStart(hiloBress));
            hBress.Start();
        }
        internal String emergente(String sts)
        {
            Point pA;
            Point[] vac = new Point[8], obs = new Point[8], est = new Point[8];
            int nVac = 0, nObs = 0, nEst = 0, i, j;
            String mov = "trabado";
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    pA = p;
                    pA.X += (i - 1) * vel;
                    pA.Y += (j - 1) * vel;
                    if (pA != p && pA != memoria)
                    {
                        mov = verificaCelda(pA);
                        if (mov == "")
                            vac[nVac++] = pA;
                        if (mov == "obstacle")
                            obs[nObs++] = pA;
                        if (mov == "eflag")
                            est[nEst++] = pA;
                    }
                }
            }
            if (nObs < 8)
            {
                pA = p;
                if (nVac > 0)
                    pA = vac[ran.Next(nVac)];
                else
                {
                    if (nEst > 0)
                        pA = est[ran.Next(nEst)];
                }
                if (pA == p)
                {
                    p = memoria;
                    mov = moverKibus();
                    grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("y");
                    memoria = p;
                    ToFront();
                }
                else
                {
                    p = pA;
                    mov = moverKibus();
                    if (mov == "")
                    {
                        grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("e");
                        ToFront();
                    }
                    else
                    {
                        if (mov == "eflag")
                        {
                            grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("e");
                            ToFront();
                            return emergente(mov);
                        }
                    }
                }
            }
            return mov;
        }
        internal void hiloBress()
        {
            bool cic = true;
            int x, y;
            String sts;
            aux = memoria = p;
            do
            {
                x = (p.X - 205) / 50;
                y = (p.Y - 15) / 50;
                sts = linea(x, y, hs.X, hs.Y);
                if (sts != "")
                    if (emergente(sts) == "trabado")
                        cic = false;
            }while((x != hs.X || y != hs.Y) && cic);
            if (!cic)
                MessageBox.Show("Kibus está encerrado");
        }
        String pixel(int x, int y)
        {
            String mov;
            p.X = x * vel + 205;
            p.Y = y * vel + 15;
            if (p == memoria)
            {
                mov = emergente("eflag");
                grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("e");
                ToFront();
            }
            else
            {
                mov = moverKibus();
                if (mov != "")
                {
                    if (mov == "eflag")
                    {
                        grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("e");
                        ToFront();
                    }
                }
            }
            Thread.Sleep(slp);
            return mov;
        }
        String linea(int xa, int ya, int xb, int yb)
        {
            int dx, dy, tx, ty, p, p0, p1, x, y, aux;
            bool f = false;
            String sts;
            if (xa == xb && ya == yb)
                return "";
            dx = Math.Abs(xb - xa);
            dy = Math.Abs(yb - ya);
            if (dx < dy)
            {
                f = true;
                aux = xa; xa = ya; ya = aux;
                aux = xb; xb = yb; yb = aux;
                aux = dx; dx = dy; dy = aux;
            }
            tx = (xb - xa) > 0 ? 1 : -1;
            ty = (yb - ya) > 0 ? 1 : -1;
            x = xa;
            y = ya;
            p0 = 2 * dy;
            p1 = 2 * (dy - dx);
            p = p0 - dx;
            while (x != xb)
            {
                if (p < 0)
                    p += p0;
                else
                {
                    y += ty;
                    p += p1;
                }
                if (f)
                {
                    if (tx > 0)
                        dir = "F";
                    else
                        dir = "B";
                    sts = pixel(y, x);
                    if (sts != "")
                        return sts;
                }
                else
                {
                    if (tx > 0)
                        dir = "R";
                    else
                        dir = "L";
                    sts = pixel(x, y);
                    if (sts != "")
                        return sts;
                }
                x += tx;
            }
            return pixel(hs.X, hs.Y);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        //---------------------------------------Bees----------------------------------------//
        /// ///////////////////////////////////////////////////////////////////////////////////
        delegate void DelToFront();
        internal void ToFront()
        {
            if (mono.InvokeRequired)
                mono.Invoke(new DelToFront(ToFront));
            else
                mono.BringToFront();
        }
        internal String moverKibus()
        {
            String colide;
            colide = MueveKibus();
            mono.Image = Image.FromFile("Resources\\Moves\\" + dir + "2link.png");
            mono.BackgroundImage = grid[(p.X - 205) / 50, (p.Y - 15) / 50].pic.Image;
            Thread.Sleep(slp);
            MueveKibus();
            mono.Image = Image.FromFile("Resources\\Moves\\" + dir + "1link.png");
            Thread.Sleep(slp);
            MueveKibus();
            mono.Image = Image.FromFile("Resources\\Moves\\" + dir + "3link.png");
            Thread.Sleep(slp);
            MueveKibus();
            mono.Image = Image.FromFile("Resources\\Moves\\" + dir + "1link.png");
            return colide;
        }
        delegate String DelMueveKibus();
        internal String MueveKibus()
        {
            if (mono.InvokeRequired)
                return (String)mono.Invoke(new DelMueveKibus(MueveKibus));
            else
            {
                String colide = "";
                if (p.X > 1205 || p.X < 205 || p.Y > 715 || p.Y < 15)
                {
                    //ColDirAnt = ColDir;
                    //if (p.X < 205 && p.Y < 15) ColDir = "UL";
                    //if (p.X < 205 && p.Y == aux.Y) ColDir = "L";
                    //if (p.X < 205 && p.Y > 715) ColDir = "DL";
                    //if (p.X == aux.X && p.Y < 15) ColDir = "U";
                    //if (p.X == aux.X && p.Y > 715) ColDir = "D";
                    //if (p.X > 1205 && p.Y < 15) ColDir = "UR";
                    //if (p.X > 1205 && p.Y == aux.Y) ColDir = "R";
                    //if (p.X > 1205 && p.Y > 715) ColDir = "UL";
                    colide = "limit";
                    p = aux;
                }
                else
                {
                    int x = (p.X - 205) / 50, y = (p.Y - 15) / 50;
                    if (grid[x, y] != null)
                        if (grid[x, y].okupada)
                            if (grid[x, y].actor != null)
                            {
                                //ColDirAnt = ColDir;
                                //if (p.X < aux.X && p.Y < aux.Y) ColDir = "UL";
                                //if (p.X < aux.X && p.Y == aux.Y) ColDir = "L";
                                //if (p.X < aux.X && p.Y > aux.Y) ColDir = "DL";
                                //if (p.X == aux.X && p.Y < aux.Y) ColDir = "U";
                                //if (p.X == aux.X && p.Y > aux.Y) ColDir = "D";
                                //if (p.X > aux.X && p.Y < aux.Y) ColDir = "UR";
                                //if (p.X > aux.X && p.Y == aux.Y) ColDir = "R";
                                //if (p.X > aux.X && p.Y > aux.Y) ColDir = "UL";
                                colide = grid[x, y].roll;
                                if (colide == "sTree" || colide == "sRock" || colide == "yflag")
                                {
                                    colide = "obstacle";
                                    p = aux;
                                }
                            }
                }
                if (!blind)
                    mono.Location = p;
                if (aux != p)
                {
                    memoria = aux;
                    aux = p;
                }
                return colide;
            }
        }
        internal void detenerAbejas()
        {
            for (int i = 0; i < 5; i++)
                if (bees[i] != null)
                    bees[i].detenerAbeja();
            if (hBees != null)
            {
                hBees.Abort();
                hBees = null;
            }
        }
        internal void BeeFriends(int hx, int hy)
        { 
            int i;
            maxEpoca = false;
            found = false;
            memoria= aux = p;
            hs.X = hx;
            hs.Y = hy;
            for (i = 0; i < 5; i++)
                bees[i] = new Abeja(p, juego, i);
            ToFront();
            hBees = new Thread(new ThreadStart(hiloBees));
            hBees.Start();
        }
        internal void hiloBees()
        {
            int cic, i, xh, yh, h;
            String mov;
            while (!found)
            {
                go = false;
                ToFront();
                do
                {
                    cic = 0;
                    foreach (Abeja b in bees)
                        if (b.arribe)
                            cic++;
                } while (cic < 5);
                re = 0;
                if (maxEpoca || found)
                {
                    for (i = 0; i < 4; i++)
                        if (board[i + 1].heat > board[i].heat)
                            re = i + 1;
                    nMov = bees[re].nMvs;
                    ToFront();
                    for (i = 0; i < nMov; i++)
                    {
                        p = board[re].mvs[i].move;
                        //dir = board[x].mvs[i].dir;
                        mov = moverKibus();
                        xh = (memoria.X - 205) / 50;
                        yh = (memoria.Y - 15) / 50;
                        if (!grid[xh, yh].okupada)
                            grid[xh, yh].posBandera("e");
                        xh = (p.X - 205) / 50;
                        yh = (p.Y - 15) / 50;
                        h = grid[xh, yh].heat - 8;
                        if (h < 0)
                            h = 0;
                        grid[xh, yh].setHeat(h);
                    }
                }
                go = true;
                do
                {
                    cic = 0;
                    foreach (Abeja b in bees)
                        if (b.moving)
                            cic++;
                } while (cic < 5);
                Thread.Sleep(slp);
            }
            p.X = hs.X * 50 + 205;
            p.Y = hs.Y * 50 + 15;
            mov = moverKibus();
            ToFront();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        //-----------------------------Malla de Conocimiento---------------------------------//
        /// ///////////////////////////////////////////////////////////////////////////////////
        delegate bool FDelMueveKibus();
        internal bool FMueveKibus()
        {
            if (mono.InvokeRequired)
                return (bool)mono.Invoke(new FDelMueveKibus(FMueveKibus));
            else
            {
                bool c = true;
                if (p.X > 1205 || p.X < 205 || p.Y > 715 || p.Y < 15)
                {
                    p = aux;
                    c = false;
                }
                else
                {
                    xN = (p.X - 205) / 50;
                    yN = (p.Y - 15) / 50;
                    if (grid[xN, yN].okupada)
                    {
                        p = aux;
                        c = false;
                    }
                    else
                    {
                        if (!blind)
                            mono.Location = p;
                        if (aux != p)
                        {
                            memoria = aux;
                            aux = p;
                        }
                    }
                }
                return c;
            }
        }
        delegate void FSDelMueveKibus();
        internal void FSMueveKibus()
        {
            if (mono.InvokeRequired)
                mono.Invoke(new FSDelMueveKibus(FSMueveKibus));
            else
                mono.Location = aux = memoria = p;
        }
        delegate void delTexto(String s);
        internal void Texto(String s)
        {
            if (juego.txbEpoks.InvokeRequired)
                juego.txbEpoks.Invoke(new delTexto(Texto), s);
            else
                juego.txbEpoks.Text = s;
        }
        internal void entrenamiento(int hx, int hy, int e)
        {
            epoks = e;
            maxEpoca = false;
            posIni = memoria = aux = p;
            hs.X = hx * 50 + 205;
            hs.Y = hy * 50 + 15;
            ToFront();
            hTraining = new Thread(new ThreadStart(hiloEntrenamiento));
            hTraining.Start();
        }
        internal void hiloEntrenamiento()
        {
            int i = 0, x, y, premio;
            first = true;
            xN = (p.X - 205) / 50;
            yN = (p.Y - 15) / 50;
            creaNodo();
            DateTime ti, tf;
            ti = DateTime.Now;
            while (i < epoks)
            {
                Texto(i.ToString());
                while (p != hs)
                {
                    x = ran.Next(3);
                    y = ran.Next(3);
                    if (!(x == 1 && y == 1))
                    {
                        p.X += (x - 1) * 50;
                        p.Y += (y - 1) * 50;
                        if (FMueveKibus())
                        {
                            if (!blind)
                                Thread.Sleep(slp);
                            creaNodo();
                            creaCnxn((memoria.X - 205) / 50, (memoria.Y - 15) / 50, y * 3 + x);
                        }
                    }
                }
                if (first)
                {
                    first = false;
                    max = min = media = premio = camino.Count;
                }
                else
                {
                    media = (max + min) / 2;
                    premio = camino.Count + (camino.Count - media);
                    if (camino.Count > max)
                        max = camino.Count;
                    else if (camino.Count < min)
                        min = camino.Count;
                }
                foreach (Arista a in camino)
                    if(premio < a.peso)
                        a.peso = premio;
                camino.Clear();
                p = posIni;
                i++;
            }
            tf = DateTime.Now;
            TimeSpan tt = new TimeSpan(tf.Ticks - ti.Ticks);
            MessageBox.Show("Time:" + tt.ToString(), "Entrenamiento Terminado");
            FSMueveKibus();
            Texto("0");
        }
        void creaNodo()
        {
            if (malla[xN, yN] == null)
                malla[xN, yN] = new Vertice(new Point(xN, yN));
        }
        void creaCnxn(int xNa, int yNa, int dir)
        {
            if (malla[xNa, yNa].cnx[dir] == null)
                malla[xNa, yNa].cnx[dir] = new Arista(malla[xN, yN], dir);
            camino.Add(malla[xNa, yNa].cnx[dir]);
        }
        internal void firstMinor(int hx, int hy)
        {
            if (malla[(p.X - 205) / 50, (p.Y - 15) / 50] == null)
                MessageBox.Show("Ubicación desconocida");
            else
            {
                posIni = memoria = aux = p;
                hs.X = hx * 50 + 205;
                hs.Y = hy * 50 + 15;
                ToFront();
                hFirstMinor = new Thread(new ThreadStart(hiloFirstMinor));
                hFirstMinor.Start();
            }
        }
        internal void hiloFirstMinor()
        {
            int x, xK, yK;
            Arista min = null;
            List<Arista> libres = new List<Arista>();
            Stack<Point> movs=new Stack<Point>();
            camino.Clear();
            movs.Push(p);
            while (p != hs)
            {
                xK = (p.X - 205) / 50;
                yK = (p.Y - 15) / 50;
                foreach (Arista a in malla[xK, yK].cnx)
                    if (a != null)
                        if (a.nv)
                            libres.Add(a);
                if (libres.Count > 0)
                {
                    libres.Sort(delegate(Arista a1, Arista a2) { return a1.peso.CompareTo(a2.peso); });
                    x = 0;
                    min = libres.ElementAt(x);
                    if (min.indx == 0) { p.X -= 50; p.Y -= 50; }
                    else if (min.indx == 1) { p.Y -= 50; }
                    else if (min.indx == 2) { p.X += 50; p.Y -= 50; }
                    else if (min.indx == 3) { p.X -= 50; }
                    else if (min.indx == 5) { p.X += 50; }
                    else if (min.indx == 6) { p.X -= 50; p.Y += 50; }
                    else if (min.indx == 7) { p.Y += 50; }
                    else if (min.indx == 8) { p.X += 50; p.Y += 50; }
                    libres.Clear();
                    movs.Push(p);
                    malla[xK, yK].cnx[min.indx].nv = false;
                    camino.Add(malla[xK, yK].cnx[min.indx]);
                }
                else
                    if (movs.Count > 0)
                        p = movs.Pop();
                FMueveKibus();
                if (!blind)
                    Thread.Sleep(slp);
            }
            foreach (Arista a in camino)
                a.nv = true;
            MessageBox.Show("Movs: " + camino.Count.ToString(), "Recorrido");
            camino.Clear();
            movs.Clear();
        }
        internal void limpiaVisitados(Vertice v)
        {
            if (!v.nv)
            {
                v.nv = true;
                foreach (Arista a in v.cnx)
                    if (a != null)
                        limpiaVisitados(a.ver);
            }
        }
    }
}
