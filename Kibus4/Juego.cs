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
    public partial class Kibus : Form
    {
        internal Casilla[,] grid = new Casilla[21, 15];

        kibus kib;
        Casita hs;
        bool placeKibus = false;
        bool KibusPlaced = false;
        bool HousePlaced = false;
        bool placeTrees = false;
        bool placeRocks = false;
        bool placeForest = true;
        bool placeHouse = false;
        bool eraseActor = false;
        bool click = false;
        internal Mapas lista;
        Thread hiloOpenArch;
        internal int epoks = 1;

        internal Kibus()
        {
            Presentacion p = new Presentacion();
            p.ShowDialog();
            InitializeComponent();
        }
        private void Kibus_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 21; i++)
                for (int j = 0; j < 15; j++)
                {
                    grid[i, j] = new Casilla(i, j, this);
                    this.Controls.Add(grid[i,j].pic);
                }
            lista = new Mapas();
            this.Controls.Add(lista.dgMapas);
            lista.dgMapas.BringToFront();
            lista.dgMapas.Show();
            hiloOpenArch = new Thread(new ThreadStart(openMap));
            hiloOpenArch.Start();
        }
        internal void clickAction(object sender, EventArgs e)
        {
            PictureBox aux = (PictureBox)sender;
            if ((String)aux.Tag == "F1link")
            {
                aux.Focus();
                aux.BringToFront();
            }
            else
            {
                int x = (aux.Location.X - 205) / 50, y = (aux.Location.Y - 15) / 50;
                if (!grid[x, y].okupada)
                {
                    if (placeKibus && !KibusPlaced)
                    {
                        if (kib == null)
                        {
                            kib = new kibus(x, y, this);
                            this.Controls.Add(kib.mono);
                            kib.mono.MouseClick += new System.Windows.Forms.MouseEventHandler(MDown);
                            kib.velocidad(trkVel.Value);
                            kib.mono.BringToFront();
                            kib.mono.BackColor = Color.Transparent;
                            kib.mono.Focus();
                            kib.iniciaHilo();
                            //placeKibus = false;
                            //KibusPlaced = true;
                            placeForest = false;
                            lista.dgMapas.Enabled = false;
                            hiloOpenArch.Abort();
                        }
                        else
                        { 
                            kib.p.X = (x * 50) + 205;
                            kib.p.Y = (y * 50) + 15;
                            kib.mono.Location = kib.memoria = kib.aux = kib.p;
                        }
                    }
                    if (placeHouse && !HousePlaced)
                    {
                        //grid[x, y].okupada = true;
                        grid[x, y].heat = 100;
                        hs = new Casita(x, y);
                        this.Controls.Add(hs.casa);
                        hs.casa.BringToFront();
                        hs.casa.BackColor = Color.Transparent;
                        //setHeat(x, y);
                        if (kib != null)
                            kib.mono.BringToFront();
                        placeForest = false;
                        placeHouse = false;
                        HousePlaced = true;
                    }
                    if (placeTrees)
                        grid[x, y].posActor("sTree");
                    if (placeRocks)
                        grid[x, y].posActor("sRock");
                }
                if (eraseActor)
                    grid[x, y].HideActor();
            }
        }
        internal void MEnter(object sender, EventArgs e)
        {
            if(click)
                clickAction(sender, e);
            Thread.Sleep(1);
        }
        internal void MDown(object sender, EventArgs e)
        {
            if(click)
                click = false;
            else
                click = true;
            clickAction(sender, e);
        }
        private void openMap()
        {
            while (true)
            {
                if (lista.openMap != "")
                {
                    reiniciar(false);
                    for (int i = 0; i < lista.nMapas; i++)
                    {
                        if (lista.openMap == lista.mapas[i].nom)
                        {
                            for (int x = 0; x < 21; x++)
                                for (int y = 0; y < 15; y++)
                                {
                                    if (lista.mapas[i].elem[x, y] == "A")
                                        grid[x, y].posActor("sTree");
                                    if (lista.mapas[i].elem[x, y] == "R")
                                        grid[x, y].posActor("sRock");
                                }
                            if (lista.mapas[i].casa != null)
                            { 
                            }
                            if (lista.mapas[i].kibus != null)
                            {
                            }
                        }
                    }
                    lista.openMap = "";
                }
                Thread.Sleep(20);
            }
        }
        private void btnForest_Click(object sender, EventArgs e)
        {
            if (placeForest)
            {
                Random rand = new Random();
                int x, y, i = 0;
                bool ciclar = true;
                while (ciclar)
                {
                    if (i < (21 * 15) * (numFollaje.Value / 100))
                    {
                        x = rand.Next(21);
                        y = rand.Next(15);
                        if (!grid[x, y].okupada)
                        {
                            if (rand.Next(2) == 0)
                                grid[x, y].posActor("sTree");
                            else
                                grid[x, y].posActor("sRock");
                            i++;
                        }
                    }
                    else
                        ciclar = false;
                }
                placeTrees = false;
                placeRocks = false;
                placeKibus = false;
                placeForest = false;
                placeHouse = false;
                eraseActor = false;
                click = false;
            }
        }
        private void Cerrar(object sender, FormClosingEventArgs e)
        {
            if (kib != null) kib.detenerHilo();
            if (hiloOpenArch != null) hiloOpenArch.Abort();
        }
        private void btnKibus_Click(object sender, EventArgs e)
        {
            placeTrees = false;
            placeRocks = false;
            placeKibus = true;
            placeHouse = false;
            eraseActor = false;
            click = false;
        }
        private void btnCasa_Click(object sender, EventArgs e)
        {
            placeTrees = false;
            placeRocks = false;
            placeKibus = false;
            placeHouse = true;
            eraseActor = false;
            click = false;
        }
        private void btnArbol_Click(object sender, EventArgs e)
        {
            placeTrees = true;
            placeRocks = false;
            placeKibus = false;
            placeHouse = false;
            eraseActor = false;
            click = false;
        }
        private void btnRock_Click(object sender, EventArgs e)
        {
            placeTrees = false;
            placeRocks = true;
            placeKibus = false;
            placeHouse = false;
            eraseActor = false;
            click = false;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            reiniciar(true);
        }
        private void reiniciar(bool re)
        {
            foreach (Casilla c in grid)
            {
                if (c.actor != null)
                    c.HideActor();
                c.actor = null;
                c.okupada = false;
                c.setPasto();
            }
            if (kib != null)
            {
                kib.detenerHilo();
                kib.HideKibus();
                kib.detenerAbejas();
                kib = null;
            }
            if (hs != null)
            {
                hs.casa.Hide();
                hs = null;
            }
            placeKibus = false;
            KibusPlaced = false;
            HousePlaced = false;
            placeTrees = false;
            placeRocks = false;
            placeForest = true;
            placeHouse = false;
            eraseActor = true;
            click = false;
            lista.dgMapas.Enabled = true;
            if (re)
            {
                hiloOpenArch.Abort();
                hiloOpenArch = new Thread(new ThreadStart(openMap));
                hiloOpenArch.Start();
            }
        }
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            if (kib != null)
            {
                kib.blind = rbTrue.Checked;
                kib.velocidad(trkVel.Value);
                kib.regresarKibus();
            }
            click = false;
        }
        private void btnBresenham_Click(object sender, EventArgs e)
        {
            if (kib != null && hs != null)
            {
                kib.blind = rbTrue.Checked;
                kib.velocidad(trkVel.Value);
                kib.Bresenham(hs.x, hs.y);
            }
            click = false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Guardar g = new Guardar(this);
            g.ShowDialog();
            click = false;
        }
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            placeTrees = false;
            placeRocks = false;
            placeKibus = false;
            placeHouse = false;
            click = false;
            eraseActor = true;
        }
        private void trkVel_ValueChanged(object sender, EventArgs e)
        {
            if (kib != null)
                kib.velocidad(trkVel.Value);
        }
        //-----------------------------------Zonas de Calor-----------------------------------------
        internal void setHeat(int x, int y)
        {
            int i, j, p, x1, y1;
            pixel(x, y, 100);
            hs.casa.BackgroundImage = grid[x, y].pic.Image;
            for (i = 1; i < 25; i++)
                circulo(x, y, i, 4 * (25 - i));
            foreach (Casilla c in grid)
                if (c.heat == -1)
                {
                    p = promedio(c.x, c.y) - 2;
                    if (p < 8)
                        p = 0;
                    pixel(c.x, c.y, p);
                }
            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                {
                    x1 = x + i - 1;
                    y1 = y + j - 1;
                    if (x != x1 && y != y1)
                        pixel(x1, y1, 95);
                }
            if (kib != null)
                kib.mono.BackgroundImage = grid[(kib.mono.Location.X - 205) / 50, (kib.mono.Location.Y - 15) /50].pic.Image;
        }
        int promedio(int x, int y)
        {
            int c = 0, x1, y1, p = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    x1 = x + i - 1;
                    y1 = y + j - 1;
                    if (x1 >= 0 && x1 < 21 && y1 >= 0 && y1 < 15 && x != x1 && y != y1)
                        if (grid[x1, y1].heat != -1)
                        {
                            p += grid[x1, y1].heat;
                            c++;
                        }
                }
            }
            return p / c;
        }
        void pixel(int x, int y, int calor)
        {
            if (x >= 0 && x < 21 && y >= 0 && y < 15)
                grid[x, y].setHeat(calor);
        }
        void puntosC(int x, int y, int x1, int y1, int calor)
        {
            pixel(x + x1, y + y1, calor);
            pixel(x - x1, y + y1, calor);
            pixel(x + x1, y - y1, calor);
            pixel(x - x1, y - y1, calor);
            pixel(x + y1, y + x1, calor);
            pixel(x - y1, y + x1, calor);
            pixel(x + y1, y - x1, calor);
            pixel(x - y1, y - x1, calor);
        }
        void circulo(int xc, int yc, int r, int calor)
        {
            int x = 0, y = r, p = 1 - r;
            puntosC(xc, yc, x, y, calor);
            while (x < y)
            {
                if (p < 0)
                {
                    x = x + 1;
                    p = p + 2 * x + 1;
                }
                else
                {
                    x++;
                    y--;
                    p = p + 2 * (x - y) + 1;
                }
                puntosC(xc, yc, x, y, calor);
            }
        }
        private void btsBees_Click(object sender, EventArgs e)
        {
            if (kib != null && hs != null)
            {
                kib.blind = rbTrue.Checked;
                kib.velocidad(trkVel.Value);
                setHeat(hs.x, hs.y);
                kib.BeeFriends(hs.x, hs.y);
            }
            click = false;
        }
        //------------------------------------Conocimiento--------------------------------------
        private void rbTrue_CheckedChanged(object sender, EventArgs e)
        {
            if (kib != null)
                kib.blind = rbTrue.Checked;
        }
        private void btnEntrenamiento_Click(object sender, EventArgs e)
        {
            if (kib != null && hs != null)
            {
                kib.blind = rbTrue.Checked;
                kib.velocidad(trkVel.Value);
                Epocas epks = new Epocas(this);
                epks.ShowDialog();
                kib.entrenamiento(hs.x, hs.y, epoks);
            }
            click = false;
        }
        private void btnPrimeroElMenor_Click(object sender, EventArgs e)
        {
            if (kib != null && hs != null)
            {
                kib.blind = rbTrue.Checked;
                kib.velocidad(trkVel.Value);
                kib.firstMinor(hs.x, hs.y);
            }
            click = false;
        }
    }
}
