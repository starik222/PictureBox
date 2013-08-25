using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using PictureBox.Properties;
using System.Drawing.Drawing2D;

namespace PictureBox
{
    public partial class Form_prev : Form
    {
        public Form_prev()
        {
            InitializeComponent();
        }
        public string[] path;
        bool[] pages;
        int current_p;
        private void Form_prev_Load(object sender, EventArgs e)
        {
            current_p = 0;
                int col_p = path.Length / 40;
                int ost = path.Length % 40;
                if (ost > 0) col_p += 1;
                pages = new bool[col_p];
                tabControl1.Width = this.Width - 10;
                tabControl1.Height = this.Height - 10;
                int cur_tab = 0;
                for (int i = 0; i < col_p; i++)
                {
                    tabControl1.TabPages.Add((i + 1).ToString());
                }
                int x = 20, y = 20;
                for (int i = 0; i < path.Length; i++)
                {
                    if (i > 40) break;
                    CreatePicBox(path[i], x, y, tabControl1.TabPages[cur_tab]);
                    if (this.Width < x + 220)
                    {
                        x = 20;
                        y += 210;
                    }
                    else
                    {
                        x += 210;
                    }
                }
            pages[0] = true;
            
        }
            
        public void CreatePicBox(string pat, int x, int y, TabPage tp)
        {
            System.Windows.Forms.PictureBox PBox = new System.Windows.Forms.PictureBox();
            PBox.Width = 200;
            PBox.Height = 200;
            PBox.Left = x;
            PBox.Top = y;
            PBox.DoubleClick += new EventHandler(PBox_DoubleClick);
            PBox.SizeMode = PictureBoxSizeMode.Zoom;
            PBox.HandleDestroyed += new EventHandler(PBox_HandleDestroyed);
            PBox.Load(pat);
            tp.Controls.Add(PBox);
            PBox.Dispose();
        }

        void PBox_HandleDestroyed(object sender, EventArgs e)
        {
            //GC.Collect();
        }


        void PBox_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int cur_tab = tabControl1.SelectedIndex;
                tabControl1.TabPages[current_p].Controls.Clear();
                GC.Collect();
                current_p = cur_tab;
                //if (!pages[cur_tab])
                //{
                    int x = 20, y = 20;
                    for (int i = cur_tab * 40 + 1; i < path.Length; i++)
                    {
                        if (i > 40 * (cur_tab + 1))
                        {
                            break;
                        }
                        CreatePicBox(path[i], x, y, tabControl1.TabPages[cur_tab]);
                        if (this.Width < x + 220)
                        {
                            x = 20;
                            y += 210;
                        }
                        else
                        {
                            x += 210;
                        }
                    }
               // }
            }
            catch (Exception ex)
            {
            }
        }

        private void Form_prev_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Dispose();
                GC.Collect();
            }
        }

    }
}
