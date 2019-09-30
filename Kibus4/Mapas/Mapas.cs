using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Kibus4
{
    class Mapas
    {
        public DataGridView dgMapas;
        public int nMapas = 0;
        public Mapa[] mapas= new Mapa[100];
        public String openMap = "";

        public Mapas()
        {
            if (AbrirArchivos())
            {
                SetupDataGridView();
                PopulateDataGridView();
            }
        }
        public void SetupDataGridView()
        {
            dgMapas = new DataGridView();
            dgMapas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgMapas.Name = "dgMapas";
            dgMapas.Size = new Size(156, 117);
            dgMapas.AllowUserToAddRows = false;
            dgMapas.AllowUserToOrderColumns = true;
            dgMapas.AllowUserToResizeColumns = false;
            dgMapas.AllowUserToResizeRows = false;
            dgMapas.AllowUserToDeleteRows = false;
            dgMapas.Location = new Point(14, 619);
            dgMapas.ReadOnly = true;
            dgMapas.ColumnCount = 1;
            dgMapas.CellDoubleClick += new DataGridViewCellEventHandler(this.dgMapas_CellClick);
            dgMapas.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgMapas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgMapas.ColumnHeadersDefaultCellStyle.Font = new Font(dgMapas.Font, FontStyle.Bold);
            dgMapas.GridColor = Color.Black;
            dgMapas.RowHeadersVisible = false;
            dgMapas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgMapas.MultiSelect = false;
            dgMapas.Columns[0].Name = "       <-Mapas->";
        }
        public void PopulateDataGridView()
        {
            String[] row = new String[1];
            for (int i = 0; i < nMapas; i++)
            {
                row[0] = mapas[i].nom;
                dgMapas.Rows.Add(row);
            }
        }
        private void dgMapas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView ob = (DataGridView)sender;
                openMap = (String)ob.Rows[e.RowIndex].Cells[0].Value;
            }
        }
        public bool AbrirArchivos()
        {
            bool correcto = true;
            String archivo;
            String[] p, c;
            bool open = true;
            int mps = 0;
            while (open)
            {
                try
                {
                    archivo = File.ReadAllText("Mapas\\" + mps + ".txt");
                    p = archivo.Split(new char[] { '\n', '\r' });
                    mapas[mps] = new Mapa(p[0]);
                    for (int i = 2; i < p.Length; i += 2)
                    {
                        c = p[i].Split(',');
                        if (c[0] == "K")
                            mapas[mps].kibus = new Point(int.Parse(c[1]), int.Parse(c[2]));
                        else if (c[0] == "C")
                            mapas[mps].casa = new Point(int.Parse(c[1]), int.Parse(c[2]));
                        else if (c[0] == "A" || c[0] == "R")
                            mapas[mps].elem[int.Parse(c[1]), int.Parse(c[2])] = c[0];
                    }
                    mps++;
                }
                catch (FileNotFoundException)
                {
                    open = false;
                }
                catch (Exception)
                {
                    correcto = false;
                }
            }
            nMapas = mps;
            return correcto;
        }
        public void SalvarArchivos(Casilla[,] grid, String nombre)
        {
            String cad = nombre + "\n\r";
            foreach (Casilla c in grid)
            {
                if (c.okupada)
                {
                    if (c.actor != null)
                    {
                        if (c.roll == "sTree")
                            cad += "A," + c.x + "," + c.y + "\n\r";
                        if (c.roll == "sRock")
                            cad += "R," + c.x + "," + c.y + "\n\r";
                    }
                }
            }
            File.WriteAllText("Mapas\\" + nMapas.ToString() + ".txt",cad);
            nMapas++;
            if (AbrirArchivos())
            {
                SetupDataGridView();
                PopulateDataGridView();
            }
        }
    }
}
