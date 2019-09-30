using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kibus4
{
    class Casilla
    {
        internal PictureBox pic;
        internal PictureBox actor;
        internal bool okupada;
        internal int x, y, ce = 0;
        internal String roll = "";
        internal int heat = -1;
        Kibus kib;

        internal Casilla(int xi, int yi, Kibus k)
        {
            kib = k;
            x = xi;
            y = yi;
            pic = new PictureBox();
            pic.Location = new Point(x * 50 + 205, y * 50 + 15);
            pic.Size = new Size(50, 50);
            pic.Image = Image.FromFile("Resources\\Pasto\\pastito.png");
            pic.MouseDown += new System.Windows.Forms.MouseEventHandler(kib.MDown);
            pic.MouseEnter += new System.EventHandler(kib.MEnter);
            pic.Tag = "pastoFondo";
            okupada = false;
        }
        internal void setPasto()
        {
            pic.Image = Image.FromFile("Resources\\Pasto\\pastito.png");
            heat = -1;
        }
        internal void setHeat(int h)
        {
            heat = h;
            h = (h / 5) * 5;
            pic.Image = Image.FromFile("Resources\\Pasto\\heat" + h.ToString() +".png");
            if(actor != null)
                actor.BackgroundImage = pic.Image;
        }
        internal void posActor(String a)
        {
            actor = new PictureBox();
            actor.Location = new Point(x * 50 + 205, y * 50 + 15);
            actor.Size = new Size(50, 50);
            actor.Image = Image.FromFile("Resources\\" + a + ".png");
            actor.MouseClick += new System.Windows.Forms.MouseEventHandler(kib.MDown);
            roll = a;
            okupada = true;
            PosActor();
            ActorCar();
        }
        internal void posBandera(String a)
        {
            if (actor == null) bandera(a);
            else
            {
                actor.Image = Image.FromFile("Resources\\" + a + "flag.png");
                roll = a + "flag";
            }
        }
        private void bandera(String a)
        {
            actor = new PictureBox();
            actor.Location = new Point(x * 50 + 205, y * 50 + 15);
            actor.Size = new Size(50, 50);
            actor.Image = Image.FromFile("Resources\\" + a + "flag.png");
            actor.BackgroundImage = pic.Image;
            roll = a + "flag";
            okupada = true;
            PosActor();
            ActorCar();
        }
        delegate void DelActorCar();
        internal void ActorCar()
        {
            if (actor.InvokeRequired)
                actor.Invoke(new DelActorCar(ActorCar));
            else
            {
                actor.BringToFront();
                actor.BackColor = Color.Transparent;
                actor.BackgroundImage = pic.Image;
            }
        }
        delegate void DelPosActor();
        internal void PosActor()
        {
            if (kib.InvokeRequired)
                kib.Invoke(new DelPosActor(PosActor));
            else
                kib.Controls.Add(actor);
        }
        delegate void DelHideActor();
        internal void HideActor()
        {
            if (actor != null)
            {
                if (actor.InvokeRequired)
                    actor.Invoke(new DelHideActor(HideActor));
                else
                {
                    actor.Hide();
                    actor = null;
                }
            }
        }
    }
}
