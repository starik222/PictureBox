using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PictureBox
{
    public partial class Form_options : Form
    {
        public Form_options()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg == null)
            {
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            }
            myreg.SetValue("DisableLoadTags", checkBox1.Checked);
            myreg.SetValue("NoSaveCache", checkBox2.Checked);
            Close();

        }

        private void Form_options_Load(object sender, EventArgs e)
        {
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg == null)
            {
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            }
            checkBox1.Checked = Convert.ToBoolean(myreg.GetValue("DisableLoadTags", false));
            checkBox2.Checked = Convert.ToBoolean(myreg.GetValue("NoSaveCache", false));
        }
    }
}
