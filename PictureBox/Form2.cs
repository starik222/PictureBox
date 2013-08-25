using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PictureBox
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public int st = 0;
        public int timest;
        public bool ra=false;
        public bool slajd = false;
        public int sel;
        public bool list;
        private void Form2_Load(object sender, EventArgs e)
        {
            ContextMenuStrip men = new System.Windows.Forms.ContextMenuStrip();

            this.DesktopLocation = new System.Drawing.Point(0, 0);
            int h,w;
            
            h = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            w = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
           // toolStripStatusLabel1.Size = new System.Drawing.Size(w, 13);
            this.Size = new System.Drawing.Size(w, h);
            pictureBox1.Size = new System.Drawing.Size(w, h);
            if (list)
            {
                pictureBox1.Load(sttr[sel]);
                GC.Collect();
                toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                st = sel;
            }
            timer1.Interval = timest;
            if (slajd)
            {
                timer1.Start();
            }
         //   timer1.Enabled = true;
        }
        
        public string[] sttr;
        private void button1_Click(object sender, EventArgs e)
        {
           // Form2 form2 = new Form2();
          //  form2.Size = new System.Drawing.Size(100, 100);
            
        }

        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                timer1.Enabled = false;
                DialogResult = DialogResult.OK;
            }
            if (e.KeyChar == (char)32)
            {
                timer1.Enabled = !timer1.Enabled;
            }
            
        }
        private int ra1;
        public bool[] nepovtor;
        private int koliches = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ra == false)
            {
                if (sttr.Length > st)
                {
                    pictureBox1.Load(sttr[st]);
                    GC.Collect();
                    toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                }
                else
                    st = -1;
                st += 1;
            }
            else
            {
                Random nn = new Random();
                
                ra1 = nn.Next(0, sttr.Length);
                if (!nepovtor[ra1])
                {
                    nepovtor[ra1] = true;
                    pictureBox1.Load(sttr[ra1]);
                    GC.Collect();
                    toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                }
                else
                {
                    if (koliches < sttr.Length - 1)
                    {
                        while (nepovtor[ra1])
                        {
                            ra1 = nn.Next(0, sttr.Length);
                        }
                        nepovtor[ra1] = true;
                        pictureBox1.Load(sttr[ra1]);
                        GC.Collect();
                        toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                    }
                    else
                    {
                        nepovtor = new bool[sttr.Length];
                        koliches = 0;
                    }
                }

                koliches++;
            }


        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != (char)32)
            {
                ToolStripMenuItem men = new ToolStripMenuItem();
                object obj;
                obj = (object)contextMenuStrip1.Items[3];
                men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[0];
                men.Checked = false;
                men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[1];
                men.Checked = true;
            }
            if (e.KeyValue == (char)39)
            {
                st += 1;
                timer1.Stop();
                if (sttr.Length > st)
                {
                    pictureBox1.Load(sttr[st]);
                    GC.Collect();
                    toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                }
                else
                    st = -1;
                
                //timer1.Start();
            }
            if (e.KeyValue == (char)37)
            {
                timer1.Stop();
                    st -= 1;
                if (0 <= st)
                {
                    pictureBox1.Load(sttr[st]);
                    toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
                }
                else
                    st = sttr.Length;
                
                //timer1.Start();
            }
        }

        private void следующееToolStripMenuItem_Click(object sender, EventArgs e)
        {

            timer1.Stop();
            if (sttr.Length > st)
            {
                pictureBox1.Load(sttr[st]);
                GC.Collect();
                toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
            }
            else
                st = -1;
            st += 1;
        }

        private void преToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (0 <= st)
            {
                pictureBox1.Load(sttr[st]);
                GC.Collect();
                toolStripStatusLabel1.Text = "      " + pictureBox1.ImageLocation;
            }
            else
                st = sttr.Length;
            st -= 1;
        }

        private void стартToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  string[] hh;
          //  hh = new string[contextMenuStrip1.Items.Count];
            ToolStripMenuItem men = new ToolStripMenuItem();
            object obj;
            obj = (object)contextMenuStrip1.Items[3];
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[0];
            men.Checked = true;
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[1];
            men.Checked = false;
            timer1.Start();
        }

        private void стопToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem men = new ToolStripMenuItem();
            object obj;
            obj = (object)contextMenuStrip1.Items[3];
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[0];
            men.Checked = false;
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[1];
            men.Checked = true;
            timer1.Stop();
        }

        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (ra == false) 
                ra = true;
            else
                ra = false;
            ((ToolStripMenuItem)sender).Checked = ra;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            slajd = false;
        }

        private void сделатьФономРабочегоСтолаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;
            f1.SetCurrentImageToDesktop(st);
        }




    }
}
