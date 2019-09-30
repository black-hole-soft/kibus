using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Kibus4
{
    class Abeja : Personaje
    {
        Thread hBee;

        internal int heat = 0, nBee;
        internal static Point kib;
        internal static Kibus gm;
        
        //internal Moves[] mvs = new Moves[6];
        internal int nMvs;

        internal Abeja(Point k, Kibus j, int nb)
        {
            nBee = nb;
            board[nb] = new Board();
            p = aux = memoria = kib = k;
            gm = j;
            mono = new PictureBox();
            mono.Location = p;
            mono.BackColor = Color.Transparent;
            mono.Size = new Size(50, 50);
            mono.Image = Image.FromFile("Resources\\Bee\\h0bee.png");
            mono.BackgroundImage = gm.grid[(p.X - 205) / 50, (p.Y - 15) / 50].pic.Image;
            mono.Tag = "bee";
            j.Controls.Add(mono);
            mono.BringToFront();
            dir = "0";
            Moves move = new Moves(p, oposite(dir));
            hBee = new Thread(new ThreadStart(hiloBee));
            hBee.Start();
        }
        private String oposite(String dir)
        {
            if (dir == "0") return "3";
            if (dir == "3") return "0";
            if (dir == "1") return "2";
            if (dir == "2") return "1";
            return "";
        }
        internal void BeeSearch()
        {
            hBee = new Thread(new ThreadStart(hiloBee));
            hBee.Start();
        }
        delegate void DelHideBee();
        internal void HideBee()
        {
            if (mono.InvokeRequired)
                mono.Invoke(new DelHideBee(HideBee));
            else
                mono.Hide();
        }
        delegate void DelToFront();
        internal void ToFront()
        {
            if (mono.InvokeRequired)
                mono.Invoke(new DelToFront(ToFront));
            else
                mono.BringToFront();
        }
        internal void detenerAbeja()
        {
            if (hBee != null)
            {
                HideBee();
                hBee.Abort();
                hBee = null;
            }
        }
        internal String moverBee()
        {
            String colide = MueveBee();
            //mono.BackgroundImage = gm.grid[(p.X - 205) / 50, (p.Y - 15) / 50].pic.Image;
            Thread.Sleep(slp);
            return colide;
        }
        delegate String DelMueveBee();
        internal String MueveBee()
        {
            if (mono.InvokeRequired)
                return (String)mono.Invoke(new DelMueveBee(MueveBee));
            else
            {
                String colide = "";
                if (p.X > 1205 || p.X < 205 || p.Y > 715 || p.Y < 15)
                {
                    ColDirAnt = ColDir;
                    colide = "limit";
                    p = aux;
                }
                else
                {
                    int x = (p.X - 205) / 50, y = (p.Y - 15) / 50;
                    if (gm.grid[x, y] != null)
                        if (gm.grid[x, y].okupada)
                            if (gm.grid[x, y].actor != null)
                            {
                                ColDirAnt = ColDir;
                                colide = gm.grid[x, y].roll;
                                if (colide == "sTree" || colide == "sRock" || colide == "yflag")
                                {
                                    colide = "obstacle";
                                    p = aux;
                                }
                            }
                }
                //mono.Image = Image.FromFile("Resources\\Bee\\h" + dir + "bee.png");
                mono.Location = p;
                mono.BackgroundImage = gm.grid[(p.X - 205) / 50, (p.Y - 15) / 50].pic.Image;
                if (aux != p)
                {
                    memoria = aux;
                    aux = p;
                }
                return colide;
            }
        }
        internal String verificaCelda(Point pA)
        {
            if (pA.X > 1205 || pA.X < 205 || pA.Y > 715 || pA.Y < 15)
                return "obstacle";
            else
            {
                int x = (pA.X - 205) / 50, y = (pA.Y - 15) / 50;
                if (gm.grid[x, y] != null)
                    if (gm.grid[x, y].okupada)
                        if (gm.grid[x, y].actor != null)
                        {
                            String col = gm.grid[x, y].roll;
                            if (col == "sTree" || col == "sRock" || col == "yflag")
                                return "obstacle";
                            if (col == "eflag")
                                return "eflag";
                        }
            }
            return "";
        }
        internal void adelanta()
        {
            int i;
            String mov;
            for (i = 0; i < nMvs; i++)
            {
                p = board[nBee].mvs[i].move;
                dir = board[nBee].mvs[i].dir;
                mov = moverBee();
            }
        }
        internal void propaga()
        {
            String mov = "trabado";
            Point pA;
            int nVac = 0, nObs = 0, nEst = 0, i, j;
            Point[] vac = new Point[8], obs = new Point[8], est = new Point[8];
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
                    mov = moverBee();
                    nMvs--;
                    if (nMvs < 0)
                        nMvs = 0;
                    gm.grid[(memoria.X - 205) / 50, (memoria.Y - 15) / 50].posBandera("y");
                    memoria = p;
                    //ToFront();
                }
                else
                {
                    p = pA;
                    mov = moverBee();
                }
            }
            heat = gm.grid[(p.X - 205) / 50, (p.Y - 15) / 50].heat;
        }
        internal void retroPropaga()
        {
            int i = nMvs;
            String mov;
            for (i = nMvs - 1; i >= 0; i--)
            {
                dir = board[nBee].mvs[i].dir;
                p = board[nBee].mvs[i].move;
                mov = moverBee();
            }
        }
        internal void hiloBee()
        {
            aux = memoria = p;
            int i, bst;
            Moves x;
            String mov;
            while (!found)
            {
                nMvs = 0;
                board[nBee].mvs[nMvs] = new Moves(p, oposite(dir));
                nMvs++;
                maxEpoca = false;
                while (!maxEpoca && !found)
                {
                    adelanta();
                    propaga();
                    x = new Moves(p, oposite(dir));
                    retroPropaga();
                    arribe = true;
                    if (nMvs < 6)
                    {
                        board[nBee].heat = heat;
                        board[nBee].mvs[nMvs] = x;
                        nMvs++;
                    }
                    if (nMvs == 6 || heat == 100)
                    {
                        if (nMvs == 6)
                            maxEpoca = true;
                        if (heat == 100)
                            found = true;
                        moving = false;
                        while (!go)
                            Thread.Sleep(slp);
                        ToFront();
                        arribe = false;
                        moving = true;
                    }
                    else
                    {
                        moving = false;
                        while (!go)
                            Thread.Sleep(slp);
                        bst = nBee;
                        for (i = 0; i < 5; i++)
                            if (i != nBee)
                                if (board[i].heat > board[bst].heat)
                                    bst = i;
                        for (i = 0; i < nMvs; i++)
                            board[nBee].mvs[i] = board[bst].mvs[i];
                        board[nBee].heat = board[bst].heat;
                        arribe = false;
                        moving = true;
                    }
                }
                Thread.Sleep(slp);
                for (i = 0; i < nMov; i++)
                {
                    p = board[re].mvs[i].move;
                    //dir = board[x].mvs[i].dir;
                    mov = moverBee();
                }
            }
        }
    }
}
