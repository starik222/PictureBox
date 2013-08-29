using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.Win32;
using System.Windows;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using PictureBox.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DesktopW;

namespace PictureBox
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public Form1()
        {

            InitializeComponent();
        }
        public struct Con_string
        {
            public string Data_Source;
            public bool Remote_add;
            public string AttachDbFilename;
            public bool Integrated_Security;
            public string User_ID;
            public string User_pass;
            public string Database;
        }
        delegate void ProgressHandler(int chisl);
        public string[] str;

        public bool DisableLoadTags;
        public bool NoSaveCache;
        public string Con_str()
        {
            Con_string c_str = new Con_string();



            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg != null)
            {
                if ((myreg.ValueCount > 3) && ((string)myreg.GetValue("Server") != ""))
                {
                    c_str.Data_Source = (string)myreg.GetValue("Server");
                    if ((string)myreg.GetValue("dbf") == "Нет")
                        c_str.Remote_add = true;
                    else
                        c_str.Remote_add = false;

                    c_str.AttachDbFilename = (string)myreg.GetValue("path_dbf");
                    c_str.Database = (string)myreg.GetValue("database");
                    if ((string)myreg.GetValue("ayth") == "Аутентификация Windows")
                        c_str.Integrated_Security = true;
                    else
                        c_str.Integrated_Security = false;
                    c_str.User_ID = (string)myreg.GetValue("login");
                    string pass = "";
                    string new_pass = "";
                    pass = (string)myreg.GetValue("pass");
                    int ppp;
                    for (int i = 0; i < pass.Length; i++)
                    {
                        ppp = (int)pass[i] - i;
                        new_pass += (char)ppp;
                    }
                    c_str.User_pass = new_pass;
                }
                else
                {
                    return "error";
                }

            }
            else
            {
                return "error";
            }
            myreg.Close();


            string first = "Data Source=";
            first = first + c_str.Data_Source + ";";
            if (!c_str.Remote_add)
            {
                first = first + "AttachDbFilename=" + c_str.AttachDbFilename + ";";
            }
            else
            {
                first = first + "Database=" + c_str.Database + ";";
            }
            if (!c_str.Integrated_Security)
            {
                first = first + "Integrated Security=false;";
                first = first + "User ID=" + c_str.User_ID + ";";
                first = first + "Password=" + c_str.User_pass + ";";
            }
            else
            {
                first = first + "Integrated Security=true;";
            }


            return first;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 prosm = new Form2();
            int secon = Convert.ToInt32(numericUpDown1.Value * 1000);
            prosm.timest = secon;
            prosm.sttr = str;
            prosm.nepovtor = new bool[str.Length];
            prosm.slajd = true;
            ToolStripMenuItem men = new ToolStripMenuItem();
            object obj;
            obj = (object)prosm.contextMenuStrip1.Items[3];
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[0];
            men.Checked = true;
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[1];
            men.Checked = false;
            men = (ToolStripMenuItem)((ToolStripMenuItem)obj).DropDownItems[2];
            men.Checked = checkBox1.Checked;
            prosm.ra = checkBox1.Checked;
            prosm.ShowDialog();



        }
        public void reloadDataFromDB()
        {
            if (!DisableLoadTags)
            {
                this.tagsTableAdapter.Fill(this.pBDataSet.tags);
            }
            else
            {
                pBDataSet.tags.Clear();
                pBDataSet.tags.Dispose();
                GC.Collect();
            }
            if (!NoSaveCache)
            {
                this.archiveTableAdapter.Fill(this.pBDataSet.archive);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if(Con_str()!="error")
            Properties.Settings.Default["PBConnectionString"] = Con_str();
            //Properties.Settings set = new Settings();
            //set["PBConnectionString"] = Con_str();
           // set.Save();
           // set.
            // TODO: данная строка кода позволяет загрузить данные в таблицу "pBDataSet.tags". При необходимости она может быть перемещена или удалена.
            try
            {

                RegistryKey myreg;
                // RunExe("1");
                myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting");
                if (myreg != null)
                {
                    DisableLoadTags = Convert.ToBoolean(myreg.GetValue("DisableLoadTags", false));
                    NoSaveCache = Convert.ToBoolean(myreg.GetValue("NoSaveCache", false));
                    string all_path = (string)myreg.GetValue("PathPict");
                    char[] charSeparators = new char[] { '|' };
                    string[] mass_path;
                    mass_path = all_path.Split(charSeparators);
                    for (int i = 0; i < mass_path.Length; i++)
                    {
                        listBox3.Items.Add(mass_path[i]);
                    }
                }
                if (!DisableLoadTags)
                {
                    this.tagsTableAdapter.Fill(this.pBDataSet.tags);
                }
                this.archiveTableAdapter.Fill(this.pBDataSet.archive);
                label9.Text = Convert.ToString(archiveTableAdapter.GetCount());
                /*if (NoSaveCache)
                {
                    this.archiveTableAdapter.Dispose();
                }*/
            }
            catch (Exception ex)
            {
                notifyIcon1.ShowBalloonTip(3000, "Ошибка подключения", ex.Message, ToolTipIcon.Error);
                Form_connect f_conn = new Form_connect();
                f_conn.Owner = this;
                f_conn.ShowDialog();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg == null)
            {
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            }
            

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {


               // label1.Text = folderBrowserDialog1.SelectedPath;
                listBox3.Items.Add(folderBrowserDialog1.SelectedPath);
                string all_path = "";
                for (int i = 0; i < listBox3.Items.Count; i++)
                {
                    if (i != listBox3.Items.Count - 1)
                    {
                        all_path = all_path + (string)listBox3.Items[i] + "|";
                    }
                    else
                    {
                        all_path = all_path + (string)listBox3.Items[i];
                    }
                }
                myreg.SetValue("PathPict", all_path);
            }
            myreg.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string[] str;
            ochistka();
            DirectoryInfo[] di = new DirectoryInfo[listBox3.Items.Count];
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                di[i] = new DirectoryInfo((string)listBox3.Items[i]);
            }
            FileInfo[][] file2 = new FileInfo[di.Length][];
            for (int i = 0; i < di.Length; i++)
            {
                file2[i] = di[i].GetFiles("*.*", SearchOption.AllDirectories);
            }
            int razmer=0;
            for (int i = 0; i < di.Length; i++)
            {
                razmer += file2[i].Length;
            }
            FileInfo[] file = new FileInfo[razmer];
            int indexes=0;
            for (int i = 0; i < di.Length; i++)
            {
                Array.Copy(file2[i], 0, file, indexes, file2[i].Length);
                indexes = indexes+file2[i].Length;
            }
            label2.Text = "Всего изображений:" + Convert.ToString(file.Length);
            pBDataSet.archive.Rows.Clear();
           // archiveTableAdapter.Update(pBDataSet.archive);
            Hashtable data = new Hashtable();
            object dummy = new object();
            string[] ext = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".wmf" };
            for (int i = 0; i < ext.Length; i++)
            {
                data.Add(ext[i], dummy);
            }
            progressBar1.Step = 1;
            progressBar1.Maximum = file.Length * 2 + 1;
            for (int i = 0; i < file.Length; i++)
            {
                DataRow dr = pBDataSet.archive.NewRow();

                dr["Pic_path"] = file[i].FullName;
                dr["Pic_name"] = file[i].Name;
                dr["Pic_num"] = Pic_nimber(file[i].Name);

                if (data.Contains(file[i].Extension.ToLower()))
                {
                    pBDataSet.archive.Rows.Add(dr);
                }
                Progress_1_p2(i);

            }
            Thread th = new Thread(new ThreadStart(this.Upd_Arc));
            th.Start();
         //   archiveTableAdapter.Update(pBDataSet.archive);
         //   SetTags();
        }
        private int Pic_nimber(string text)
        {
            char[] charSeparators = new char[] { ' ' };
            string[] mas_tag;
            mas_tag = text.Split(charSeparators);
            if (mas_tag.Length > 3)
            {
                try
                {
                    return Convert.ToInt32(mas_tag[2]);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            else return 0;

        }
        private void Upd_Arc()
        {
            archiveTableAdapter.Update(pBDataSet.archive);
            SetTags();
        }

        private void Progress_1_p2(int chisl)
        {
            if (this.progressBar1.InvokeRequired)
            {
                ProgressHandler d = new ProgressHandler(Progress_1_p2);
                this.Invoke(d, new object[] { chisl });
            }
            else
            {
                this.progressBar1.Value = chisl;
            }
        }

        public void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DataRow[] dr;
            string expr, temp;
            expr = "";
            char[] razdelen = new char[] { ' ' };
            if (radioButton1.Checked||radioButton3.Checked)
            {

                
                temp = textBox1.Text;
                
                char[] charSeparators = new char[] { ' ', '|' };
                char[] otk = new char[] { '{' };
                char[] zakr = new char[] { '}' };
                expr = "";
                string[] mass;
                mass = temp.Split(charSeparators);
                int vxozh = 0;
                if (mass.Length > 1)
                {
                    bool[] vibo = new bool[mass.Length - 1];
                    for (int j = 0; j < vibo.Length; j++)
                    {
                        vxozh = temp.IndexOfAny(charSeparators, vxozh + 1);
                        if (temp[vxozh] == ' ')
                        {
                            vibo[j] = true;
                        }
                        if (temp[vxozh] == '|')
                        {
                            vibo[j] = false;
                        }
                    }
                    expr = "(Pic_name LIKE '*" + mass[0] + "*'";
                    for (int i = 1; i < mass.Length; i++)
                    {
                        if (vibo[i - 1])
                        {
                            expr = expr + " AND Pic_name LIKE '*" + mass[i] + "*'";
                        }
                        else
                        {
                            expr = expr + " OR Pic_name LIKE '*" + mass[i] + "*'";
                        }
                    }
                    int otkr = expr.IndexOfAny(otk);
                    if (otkr != -1)
                    {
                        expr = expr.Remove(otkr, 1);
                        expr = expr.Insert(otkr - 16, "(");

                        int zak = expr.IndexOfAny(zakr);
                        expr = expr.Insert(zak + 3, ")");
                        expr = expr.Remove(zak, 1);
                    }



                }
                else
                {
                    expr = "(Pic_name LIKE '*" + mass[0] + "*'";
                }
                expr = expr + ")";
                if (textBox5.Text != "")
                {
                    string[] arr_iskl = textBox5.Text.Split(razdelen);
                    expr += " AND (";
                    for (int k = 0; k < arr_iskl.Length; k++)
                    {
                        expr += " Pic_name not LIKE '*" + arr_iskl[k] + "*'";
                        if (k == arr_iskl.Length - 1)
                        {
                            expr += ")";
                        }
                        else
                        {
                            expr += " AND ";
                        }
                    }
                }
                if (radioButton3.Checked)
                {
                    expr = expr + " AND (Pic_num>=" + textBox3.Text + " AND Pic_num<=" + textBox4.Text + ")";
                }
                // 



                //   MessageBox.Show(expr);
            }
            else
            {
                expr = "Pic_num>="+textBox3.Text+" AND Pic_num<="+textBox4.Text;
            }
                dr = pBDataSet.archive.Select(expr);
                str = new string[dr.Length];
                for (int i = 0; i < dr.Length; i++)
                {
                    listBox1.Items.Add(dr[i].ItemArray[2]);

                    str[i] = (string)dr[i].ItemArray[1];
                }
                label3.Text = "Найдено изображений:" + listBox1.Items.Count.ToString();

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int sel;
            sel = listBox1.SelectedIndex;

            Form2 prosm = new Form2();
            prosm.Owner = this;
            int secon = Convert.ToInt32(numericUpDown1.Value * 1000);
            prosm.timest = secon;
            prosm.sttr = str;
            prosm.nepovtor = new bool[str.Length];
            prosm.sel = sel;
            prosm.list = true;
            prosm.ShowDialog();

        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            DataRow[] row, row1;
            listBox2.Items.Clear();
            if (listBox1.SelectedItem.ToString() != "")
            {
                pictureBox1.Load(str[listBox1.SelectedIndex]);
                int pp = str[listBox1.SelectedIndex].LastIndexOf("'");
                if (pp == (-1))
                {
                    if (!DisableLoadTags)
                    {
                        row = pBDataSet.archive.Select("Pic_Path='" + str[listBox1.SelectedIndex] + "'");
                        if (row.Length != 0)
                        {
                            row1 = pBDataSet.tags.Select("Kod_Arc=" + row[0].ItemArray[0]);
                            for (int i = 0; i < row1.Length; i++)
                            {
                                listBox2.Items.Add(row1[i].ItemArray[2]);
                            }
                        }
                    }
                    else
                    {
                        string[] aot = GetArrayOfTags(Path.GetFileName(str[listBox1.SelectedIndex]));
                        for (int i = 0; i < aot.Length; i++)
                        {
                            listBox2.Items.Add(aot[i]);
                        }
                    }
                }
            }
        }
        private string[] GetArrayOfTags(string name)
        {
            // tagsTableAdapter.Delete();
            //  pBDataSet.tags.Rows.Clear();
            // tagsTableAdapter.Update(pBDataSet.tags);
            int prov;
            char[] charSeparators = new char[] { ' ' };
            string filen;
            int id_pic = 0;
            string[] ttt;
            DataTable table;
            string t1 = "Konachan.com";
            string t2 = "-";
            int prog;
            filen = name;
            List<string> aot = new List<string>();
            filen = filen.Remove(filen.Length - 4);
                //   while (posl != (-1))
                //  {
                //      posl = filen.LastIndexOf(" ", pos);
            ttt = filen.Split(charSeparators);

            for (int j = 0; j < ttt.Length; j++)
            {
                if ((ttt[j] != t1) && (ttt[j] != t2))
                {
                    try
                    {
                        prov = Convert.ToInt32(ttt[j]);
                    }
                    catch (FormatException)
                    {
                        aot.Add(ttt[j]);
                    }
                }
            }
            return aot.ToArray();
        }
        private void SetTags()
        {
           // tagsTableAdapter.Delete();
          //  pBDataSet.tags.Rows.Clear();
           // tagsTableAdapter.Update(pBDataSet.tags);
            int prov;
            char[] charSeparators = new char[] { ' ' };
            string filen;
            int id_pic = 0;
            string[] ttt;
            DataTable table;
            string t1 = "Konachan.com";
            string t2 = "-";
            DataRow row;
            int prog;
            prog = progressBar1.Value;
            table = (DataTable)archiveTableAdapter.GetData();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                filen = row["Pic_name"].ToString();
                id_pic =Convert.ToInt32(row["Kod"].ToString());
                filen = filen.Remove(filen.Length - 4);
                //   while (posl != (-1))
                //  {
                //      posl = filen.LastIndexOf(" ", pos);
                ttt = filen.Split(charSeparators);

                for (int j = 0; j < ttt.Length; j++)
                {
                    DataRow dr = pBDataSet.tags.NewRow();
                    if ((ttt[j] != t1) && (ttt[j] != t2))
                    {
                        try
                        {
                            prov = Convert.ToInt32(ttt[j]);
                        }
                        catch (FormatException)
                        {
                            dr["kod_arc"] = id_pic;
                            dr["tag"] = ttt[j];
                            pBDataSet.tags.Rows.Add(dr);

                        }
                    }
                }
                Progress_1_p2(i + prog);
            }
            tagsTableAdapter.Update(pBDataSet.tags);
            MessageBox.Show("Завершено");
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = (string)listBox2.SelectedItem;
            listBox1.Items.Clear();
            DataRow[] dr;
            string expr, stt, temp;
            temp = textBox1.Text;
            stt = "";
            int kk;
            kk = 0;
            expr = "";
            for (int i = 0; i < textBox1.TextLength; i++)
            {
                if (temp[i] != ' ')
                {
                    stt = stt + temp[i];
                }
                else
                {
                    if (kk == 0)
                    {
                        expr = "Pic_name LIKE '*" + stt + "*'";
                        kk += 1;
                        stt = "";
                    }
                    else
                    {
                        expr = expr + " AND Pic_name LIKE '*" + stt + "*'";
                        stt = "";
                    }

                }
                if (i == textBox1.TextLength - 1)
                {
                    if (kk != 0)
                        expr = expr + " AND Pic_name LIKE '*" + stt + "*'";
                    else
                        expr = expr + "Pic_name LIKE '*" + stt + "*'";
                    stt = "";
                }
            }

            //   MessageBox.Show(expr);
            dr = pBDataSet.archive.Select(expr);
            str = new string[dr.Length];
            for (int i = 0; i < dr.Length; i++)
            {
                listBox1.Items.Add(dr[i].ItemArray[2]);

                str[i] = (string)dr[i].ItemArray[1];
            }
            label3.Text = "Найдено изображений:" + listBox1.Items.Count.ToString();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem men = new ToolStripMenuItem();
            ToolStripMenuItem men1 = new ToolStripMenuItem();
            object obj, obj1;
            obj = (object)contextMenuStrip1.Items[0];
            obj1 = (object)contextMenuStrip1.Items[1];

            men1 = (ToolStripMenuItem)obj1;
            men = (ToolStripMenuItem)obj;
            if (FormWindowState.Minimized == WindowState)
            {
                men1.Enabled = false;
                men.Enabled = true;
            }
            else
            {
                men1.Enabled = true;
                men.Enabled = false;
            }

        }

        private void свернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Hide();

        }

        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            active = false;
            //int tt = ((int)numericUpDown1.Value * 1000) + 100;
            Thread.Sleep(1500);
            if (thr != null)
            {
                if (thr.ThreadState == System.Threading.ThreadState.Running)
                    thr.Abort();
            }
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
        }



        public Random nn1= new Random();
        public int st1 = 0;

        private void ochistka()
        {
            try
            {
                //pBDataSet.tags.Rows.Clear();
                //tagsTableAdapter.Delete();
                //string con_tag = tagsTableAdapter.Connection.ConnectionString;
                ExecQuery(Con_str(), "DELETE FROM tags");
                ExecQuery(Con_str(), "DELETE FROM archive");
                ExecQuery(Con_str(), "DBCC CHECKIDENT (tags, RESEED, 0)");
                //tagsTableAdapter.Update(pBDataSet.tags);
                //pBDataSet.archive.Rows.Clear();
                //archiveTableAdapter.Delete();
                ExecQuery(Con_str(), "DBCC CHECKIDENT (archive, RESEED, 0)");
                this.tagsTableAdapter.Fill(this.pBDataSet.tags);
                this.archiveTableAdapter.Fill(this.pBDataSet.archive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
            ExecQuery(archiveTableAdapter.Connection.ConnectionString, "dump transaction PB with no_log");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ExecQuery(string myConnectionString, string value)
        {
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            string myInsertQuery = value;
            SqlCommand myCommand = new SqlCommand(myInsertQuery);
            myCommand.Connection = myConnection;
            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myCommand.Connection.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox1.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox1.Enabled = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox1.Enabled = true;
            }
        }

        private void копироватьВToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  DataTable table;
            DataRow[] row;
            if (listBox1.SelectedItem.ToString() != "")
            {
              //  pictureBox1.Load(str[listBox1.SelectedIndex]);
                int pp = str[listBox1.SelectedIndex].LastIndexOf("'");
                if (pp == (-1))
                {
                    row = pBDataSet.archive.Select("Pic_Path='" + str[listBox1.SelectedIndex] + "'");
                    if (row.Length != 0)
                    {
                        saveFileDialog1.FileName = row[0][2].ToString();
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            File.Copy(str[listBox1.SelectedIndex], saveFileDialog1.FileName);
                        }
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form3 f_about = new Form3();
            f_about.ShowDialog();
        }

        private void настройкаСоединенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_connect f_con = new Form_connect();
            f_con.Owner = this;
            f_con.ShowDialog();
        }
        public int coll = 1000;
        private void списокТеговToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Form_spis_tags f_s = new Form_spis_tags();
            f_s.Owner = this;
            f_s.col = coll;
            f_s.ShowDialog();
        }
        Thread thr;
        private void button5_Click(object sender, EventArgs e)
        {
            thr = new Thread(SetImageToDesktop);
            interv = (int)numericUpDown1.Value;
            active = true;
            Hide();
            WindowState = FormWindowState.Minimized;
            thr.Start();

        }
        bool active = true;
        int interv;
        void SetImageToDesktop()
        {
            int val;
            Desktop dsk = new Desktop();
            string path = Application.StartupPath + "\\img.bmp";
            Image img,img2;
            while (active)
            {
                try
                {
                    if (str.Length == 0) break;
                    val = nn1.Next(0, str.Length);
                    img = Image.FromFile(str[val]);
                    img2 = resizeImage(img);
                    img2.Save(path, ImageFormat.Bmp);
                    img.Dispose();
                    img2.Dispose();
                    dsk.SetWallpaper(str[val], path, 0);
                    for (int i = 1; i <= interv; i++)
                    {
                        if (!active)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    notifyIcon1.ShowBalloonTip(3000, "Ошибка", ex.Message, ToolTipIcon.Error);
                    Thread.Sleep(3000);
                }
            }
        }
        public void SetCurrentImageToDesktop(int val)
        {
            //int val;
            Desktop dsk = new Desktop();
            string path = Application.StartupPath + "\\img.bmp";
            Image img, img2;
            try
            {
                if (str.Length != 0)
                {
                    //val = nn1.Next(0, str.Length);
                    img = Image.FromFile(str[val]);
                    img2 = resizeImage(img);
                    img2.Save(path, ImageFormat.Bmp);
                    img.Dispose();
                    img2.Dispose();
                    dsk.SetWallpaper(str[val], path, 0);
                }
            }
            catch (Exception ex)
            {
                notifyIcon1.ShowBalloonTip(3000, "Ошибка", ex.Message, ToolTipIcon.Error);
                Thread.Sleep(3000);
            }
        }

        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize)
        {
            int h, w;

            h = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            w = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;

            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float per = (float)w / (float)sourceWidth;

            if ((float)sourceHeight * per > (float)h)
            {
                per = (float)h / (float)sourceHeight;
            }

            int destWidth = (int)(sourceWidth * per);
            int destHeight = (int)(sourceHeight * per);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (System.Drawing.Image)b;
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void предпросмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_prev f_pr = new Form_prev();
            f_pr.Owner = this;
            f_pr.path = str;
            f_pr.ShowDialog();
            GC.Collect();
            
        }

        private void очисткаБазыДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(MessageBox.Show("Вы действительно хотите удалить все записи из базы данных?","Внимание",MessageBoxButtons.YesNo,MessageBoxIcon.Question)== System.Windows.Forms.DialogResult.Yes)
                ochistka();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            active = false;
            int tt = ((int)numericUpDown1.Value * 1000) + 100;
            Thread.Sleep(tt);
            if (thr != null)
            {
                if (thr.ThreadState == System.Threading.ThreadState.Running)
                    thr.Abort();
            }
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void webSearchKonachanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("WebSearchKonachan.exe");
        }

        private void общиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_options f_o = new Form_options();
            f_o.Owner = this;
            f_o.ShowDialog();
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg == null)
            {
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            }
            DisableLoadTags = Convert.ToBoolean(myreg.GetValue("DisableLoadTags", false));
            NoSaveCache = Convert.ToBoolean(myreg.GetValue("NoSaveCache", false));
            reloadDataFromDB();
        }
    

    
    
    

    }
}
