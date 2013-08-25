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
    public partial class Form_spis_tags : Form
    {
        public Form_spis_tags()
        {
            InitializeComponent();
        }
        public int col = 1000;
        private void Form_spis_tags_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = col;
            PBDataSetTableAdapters.tags1TableAdapter t1 = new PBDataSetTableAdapters.tags1TableAdapter();
            DataTable dt = t1.GetData(col);
            int xx = 12, yy = 64;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Label ll = new Label();
                ll.Name = "la" + i.ToString();
                ll.Left = xx;
                ll.Top = yy;
                ll.Text = dt.Rows[i][0].ToString() + " (" + dt.Rows[i][1].ToString() + ")";
                ll.Click += new EventHandler(ll_Click);
                ll.BackColor = Color.White;
                ll.ForeColor = Color.Red;
                this.Controls.Add(ll);
                xx += 130;
                if (xx > 1000)
                {
                    yy += 30;
                    xx = 12;
                }
            }
        }

        void ll_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;
            string tg = ((Label)sender).Text;
            int pos = tg.IndexOf(" (");
            tg = tg.Remove(pos);
            f1.textBox1.Text = tg;
            f1.coll = col;
            f1.button4_Click(null, EventArgs.Empty);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            col = Convert.ToInt32(numericUpDown1.Value);
            Form_spis_tags_Load(this, EventArgs.Empty);
        }
    }
}
