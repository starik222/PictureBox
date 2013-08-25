using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Win32;

namespace WebSearchKonachan
{
    public partial class Form_ws : Form
    {
        public Form_ws()
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

        delegate void SetTextCallback(string text);
        delegate void ProgressHandler(int chisl);
        public bool pin = false;
        Thread th;
        Thread th2;
        public bool vkl = true;



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
                    if (pass != null)
                    {
                        for (int i = 0; i < pass.Length; i++)
                        {
                            ppp = (int)pass[i] - i;
                            new_pass += (char)ppp;
                        }
                        c_str.User_pass = new_pass;
                    }
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

        bool[] pages;
        private void button1_Click(object sender, EventArgs e)
        {
            start_page_p1 = Convert.ToInt32(textBox4.Text);
            end_page_p1 = Convert.ToInt32(textBox5.Text);
            pages = new bool[end_page_p1 - start_page_p1+1];
            for (int i = 0; i < pages.Length; i++) pages[i] = false;
            path = textBox1.Text;
            path_p2 = textBox1.Text;
            th = new Thread(new ThreadStart(this.DownloadImage));
            th2 = new Thread(new ThreadStart(this.DownloadImage_p2));
            if (!pin)
            {
                vkl = true;
                th.Start();
                th2.Start();
                button1.Text = "Остановить";
                pin = !pin;
            }
            else
            {
                vkl = false;
                th.Abort();
                th2.Abort();
                button1.Text = "Начать";
                pin = !pin;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                if (File.Exists(folderBrowserDialog1.SelectedPath + "\\" + "page_num.txt"))
                {
                    StreamReader stre =  File.OpenText(folderBrowserDialog1.SelectedPath + "\\" + "page_num.txt");
                    string cur_str = stre.ReadLine();
                    textBox16.Text = cur_str;
                    stre.Close();

                }
                if (File.Exists(folderBrowserDialog1.SelectedPath + "\\" + "page_num_p2.txt"))
                {
                    StreamReader stre = File.OpenText(folderBrowserDialog1.SelectedPath + "\\" + "page_num_p2.txt");
                    string cur_str = stre.ReadLine();
                    textBox15.Text = cur_str;
                    stre.Close();

                }
            }
        }
        public bool downloaded = false;
        public bool downloaded_p2 = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "pBDataSet.archive". При необходимости она может быть перемещена или удалена.
            if (Con_str() != "error")
                Properties.Settings.Default["PBConnectionString"] = Con_str();
            this.archiveTableAdapter.Fill(this.pBDataSet.archive);
            label15.Text= "Текущее подключение: "+ Properties.Settings.Default["PBConnectionString"].ToString();
        }
        public int start_page_p1;
        public int end_page_p1;
        public string path;
        public string path_p2;
        private void DownloadImage()
        {

                int kol_sovp = 0;
                int kol_exis = 0;
                int all_row = 0;
                string url_kona;
                for (int kk = start_page_p1; kk <= end_page_p1; kk++)
                {
                    if (!vkl)
                    {
                        break;
                    }
                    if (pages[kk - start_page_p1])
                    {
                        continue;
                    }
                    else
                    {
                        pages[kk - start_page_p1] = true;
                    }
                    try{
                    url_kona = "http://konachan.com/post?page=" + kk.ToString();
                    HttpWebRequest webreque = (HttpWebRequest)WebRequest.Create(url_kona);
                    HttpWebResponse webrespon;
                    webrespon = (HttpWebResponse)webreque.GetResponse();
                    StreamReader str_read = new StreamReader(webrespon.GetResponseStream());
                    CurPage(Convert.ToString(kk));
                    if (File.Exists(path + "\\" + "page_num.txt"))
                    {
                        File.Delete(path + "\\" + "page_num.txt");
                    }
                    StreamWriter stw = File.CreateText(path+"\\"+"page_num.txt");
                    stw.Write(kk);
                    stw.Close();

                   // File.OpenWrite("Page_number.txt");
                    string str = "";
                    // while (!str_read.EndOfStream)
                    // {
                    str = str_read.ReadToEnd();
                    // richTextBox1.Text = str;
                    // }
                    string final_str = "";
                    int ii = 0;
                    int vhod = 0;
                    System.Collections.Generic.List<string> spisok_strok = new System.Collections.Generic.List<string>();
                    System.Collections.Generic.List<string> dlya_sravn = new System.Collections.Generic.List<string>();
                    bool poisk = true;
                    while (vhod != -1)
                    {
                        final_str = "";
                        poisk = true;
                        vhod = str.IndexOf("http://konachan.com/image/", ii);
                        if (vhod != -1)
                        {
                            while (poisk)
                            {
                                if (str[vhod] != '"')
                                {
                                    final_str += str[vhod];
                                    vhod++;
                                }
                                else
                                {
                                    poisk = false;
                                }
                            }
                            // richTextBox1.Text += final_str;
                            // richTextBox1.Text += '\n';
                            bool prov = true;
                            if (spisok_strok.Count == 0)
                            {
                                spisok_strok.Add(final_str);
                            }
                            else
                            {
                                for (int j = 0; j < spisok_strok.Count; j++)
                                {
                                    if (spisok_strok[j] == final_str)
                                    {
                                        prov = false;
                                        break;
                                    }
                                }
                                if (prov)
                                {
                                    spisok_strok.Add(final_str);
                                }
                            }
                            ii = vhod;
                        }
                    }

                    vhod = 0;
                    ii = 0;
                    while (vhod != -1)
                    {
                        final_str = "";
                        poisk = true;
                        vhod = str.IndexOf("http://konachan.com/jpeg/", ii);
                        if (vhod != -1)
                        {
                            while (poisk)
                            {
                                if (str[vhod] != '"')
                                {
                                    final_str += str[vhod];
                                    vhod++;
                                }
                                else
                                {
                                    poisk = false;
                                }
                            }
                            // richTextBox1.Text += final_str;
                            // richTextBox1.Text += '\n';
                            bool prov = true;
                            if (spisok_strok.Count == 0)
                            {
                                spisok_strok.Add(final_str);
                            }
                            else
                            {
                                for (int j = 0; j < spisok_strok.Count; j++)
                                {
                                    if (spisok_strok[j] == final_str)
                                    {
                                        prov = false;
                                        break;
                                    }
                                }
                                if (prov)
                                {
                                    spisok_strok.Add(final_str);
                                }
                            }
                            ii = vhod;
                        }





                    }
                    string temp1 = "";
                    for (int i = 0; i < spisok_strok.Count; i++)
                    {
                        temp1 = spisok_strok[i].Replace("%20", " ");
                        temp1 = temp1.Replace("%28", "(");
                        temp1 = temp1.Replace("%29", ")");
                        temp1 = temp1.Replace("%7E", "~");
                        temp1 = temp1.Replace("%21", "!");
                        temp1 = temp1.Replace("%3B", ";");
                        temp1 = temp1.Replace("%26", "&");
                        temp1 = temp1.Replace("%27", "'");
                        temp1 = temp1.Replace("%3A", ";");
                        temp1 = temp1.Replace("%40", "@");
                        temp1 = temp1.Replace("%2B", "+");
                        temp1 = temp1.Replace("%3F", "!");
                        temp1 = temp1.Replace("%C3%97", "×");
                        temp1 = temp1.Replace("%CE%B8", "θ");
                        temp1 = temp1.Replace("%3D", "=");
                        temp1 = temp1.Replace("%2A", "×");
                        temp1 = temp1.Replace("%C3%A8", "e");
                        temp1 = temp1.Replace("%C3%A7", "c");
                        temp1 = temp1.Replace("%C2%BD", "½");
                        temp1 = temp1.Replace("%E2%80%99", "’");
                        dlya_sravn.Add(temp1);
                    }
                    int vh = 0;
                    string substr = "";
                    string sub_temp = "";
                    WebClient wb = new WebClient();
                    DataRow[] dr;
                    string file_save = "";
                    string sel = "";
                    int col_err = 0;
                    int vhod_probel = 0;
                    for (int i = 0; i < dlya_sravn.Count; i++)
                    {
                        
                        vh = dlya_sravn[i].IndexOf("Konachan.com");
                        if (vh != -1)
                        {
                            sub_temp = dlya_sravn[i].Substring(vh + 15);
                            vhod_probel = sub_temp.IndexOf(' ');
                            substr = sub_temp.Substring(0, vhod_probel);

                            file_save = dlya_sravn[i].Substring(vh);
                            sel = "Pic_name LIKE '*" + substr + "*'";
                            dr = pBDataSet.archive.Select(sel);
                            if (dr.Length == 0)
                            {
                            IfErr:
                                try
                                {
                                
                                    if (!File.Exists(path + "\\" + file_save))
                                    {
                                    
                                        downloaded = false;
                                        wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                                        wb.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                                   //     wb.DownloadFile(spisok_strok[i], path + "\\" + file_save);
                                        wb.DownloadFileAsync(new Uri(spisok_strok[i]), path + "\\" + file_save);
                                        while (!downloaded)
                                        {
                                            Thread.Sleep(300);
                                        }
                                        AddTextToList(file_save);
                                    }
                                    else
                                    {
                                        kol_exis++;
                                        Col_Exist(Convert.ToString(kol_exis));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ErrMess(Convert.ToString(kk) + ":" + ex.Message);
                                    if (col_err < 8)
                                    {
                                        col_err++;
                                        goto IfErr;
                                        
                                    }
                                    else
                                    {
                                        col_err = 0;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                kol_sovp++;
                                KolSovpadenij(Convert.ToString(kol_sovp));
                            }
                        }
                        all_row++;
                        All_down(Convert.ToString(all_row));
                    }
                }
                    catch (Exception ex)
                    {
                        ErrMess(Convert.ToString(kk)+":" + ex.Message);
                    }
            }

        }
        private void AddTextToList(string text)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddTextToList);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
            }
        }
        private void KolSovpadenij(string text)
        {
            if (this.textBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(KolSovpadenij);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox2.Text = text;
            }
        }
        private void CurPage(string text)
        {
            if (this.textBox3.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(CurPage);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox3.Text = text;
            }
        }
        private void ErrMess(string text)
        {
            if (this.listBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ErrMess);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox2.Items.Add(text);
            }
        }
        private void KolSovpadenij_p2(string text)
        {
            if (this.textBox8.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(KolSovpadenij_p2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox8.Text = text;
            }
        }
        private void CurPage_p2(string text)
        {
            if (this.textBox9.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(CurPage_p2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox9.Text = text;
            }
        }
        private void Col_Exist(string text)
        {
            if (this.textBox10.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Col_Exist);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox10.Text = text;
            }
        }
        private void All_down(string text)
        {
            if (this.textBox11.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(All_down);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox11.Text = text;
            }
        }
        private void Col_Exist_p2(string text)
        {
            if (this.textBox12.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Col_Exist_p2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox12.Text = text;
            }
        }
        private void All_down_p2(string text)
        {
            if (this.textBox13.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(All_down_p2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox13.Text = text;
            }
        }
        private void Progress_1(int chisl)
        {
            if (this.progressBar1.InvokeRequired)
            {
                ProgressHandler d = new ProgressHandler(Progress_1);
                this.Invoke(d, new object[] { chisl });
            }
            else
            {
                this.progressBar1.Value = chisl;
            }
        }
        private void Return_Progress_1(int chisl)
        {
            if (this.progressBar1.InvokeRequired)
            {
                ProgressHandler d = new ProgressHandler(Return_Progress_1);
                this.Invoke(d, new object[] { chisl });
            }
            else
            {
                chisl = this.progressBar1.Value;
            }
        }

        private void Progress_1_p2(int chisl)
        {
            if (this.progressBar2.InvokeRequired)
            {
                ProgressHandler d = new ProgressHandler(Progress_1_p2);
                this.Invoke(d, new object[] { chisl });
            }
            else
            {
                this.progressBar2.Value = chisl;
            }
        }
        private void Return_Progress_1_p2(int chisl)
        {
            if (this.progressBar2.InvokeRequired)
            {
                ProgressHandler d = new ProgressHandler(Return_Progress_1_p2);
                this.Invoke(d, new object[] { chisl });
            }
            else
            {
                chisl = this.progressBar2.Value;
            }
        }



        private void DownloadImage_p2()
        {
            Thread.Sleep(7000);

            int kol_sovp = 0;
            int kol_exis = 0;
            int all_row = 0;
            string url_kona;
            for (int kk = start_page_p1; kk <= end_page_p1; kk++)
            {
                if (!vkl)
                {
                    break;
                }
                if (pages[kk - start_page_p1])
                {
                    continue;
                }
                else
                {
                    pages[kk - start_page_p1] = true;
                }
                try
                {
                    url_kona = "http://konachan.com/post?page=" + kk.ToString();
                    HttpWebRequest webreque = (HttpWebRequest)WebRequest.Create(url_kona);
                    HttpWebResponse webrespon;
                    webrespon = (HttpWebResponse)webreque.GetResponse();
                    StreamReader str_read = new StreamReader(webrespon.GetResponseStream());
                    CurPage_p2(Convert.ToString(kk));
                    if (File.Exists(path_p2 + "\\" + "page_num_p2.txt"))
                    {
                        File.Delete(path_p2 + "\\" + "page_num_p2.txt");
                    }
                    StreamWriter stw = File.CreateText(path_p2 + "\\" + "page_num_p2.txt");
                    stw.Write(kk);
                    stw.Close();

                    // File.OpenWrite("Page_number.txt");
                    string str = "";
                    // while (!str_read.EndOfStream)
                    // {
                    str = str_read.ReadToEnd();
                    // richTextBox1.Text = str;
                    // }
                    string final_str = "";
                    int ii = 0;
                    int vhod = 0;
                    System.Collections.Generic.List<string> spisok_strok = new System.Collections.Generic.List<string>();
                    System.Collections.Generic.List<string> dlya_sravn = new System.Collections.Generic.List<string>();
                    bool poisk = true;
                    while (vhod != -1)
                    {
                        final_str = "";
                        poisk = true;
                        vhod = str.IndexOf("http://konachan.com/image/", ii);
                        if (vhod != -1)
                        {
                            while (poisk)
                            {
                                if (str[vhod] != '"')
                                {
                                    final_str += str[vhod];
                                    vhod++;
                                }
                                else
                                {
                                    poisk = false;
                                }
                            }
                            // richTextBox1.Text += final_str;
                            // richTextBox1.Text += '\n';
                            bool prov = true;
                            if (spisok_strok.Count == 0)
                            {
                                spisok_strok.Add(final_str);
                            }
                            else
                            {
                                for (int j = 0; j < spisok_strok.Count; j++)
                                {
                                    if (spisok_strok[j] == final_str)
                                    {
                                        prov = false;
                                        break;
                                    }
                                }
                                if (prov)
                                {
                                    spisok_strok.Add(final_str);
                                }
                            }
                            ii = vhod;
                        }
                    }

                    vhod = 0;
                    ii = 0;
                    while (vhod != -1)
                    {
                        final_str = "";
                        poisk = true;
                        vhod = str.IndexOf("http://konachan.com/jpeg/", ii);
                        if (vhod != -1)
                        {
                            while (poisk)
                            {
                                if (str[vhod] != '"')
                                {
                                    final_str += str[vhod];
                                    vhod++;
                                }
                                else
                                {
                                    poisk = false;
                                }
                            }
                            // richTextBox1.Text += final_str;
                            // richTextBox1.Text += '\n';
                            bool prov = true;
                            if (spisok_strok.Count == 0)
                            {
                                spisok_strok.Add(final_str);
                            }
                            else
                            {
                                for (int j = 0; j < spisok_strok.Count; j++)
                                {
                                    if (spisok_strok[j] == final_str)
                                    {
                                        prov = false;
                                        break;
                                    }
                                }
                                if (prov)
                                {
                                    spisok_strok.Add(final_str);
                                }
                            }
                            ii = vhod;
                        }





                    }
                    string temp1 = "";
                    for (int i = 0; i < spisok_strok.Count; i++)
                    {
                        temp1 = spisok_strok[i].Replace("%20", " ");
                        temp1 = temp1.Replace("%28", "(");
                        temp1 = temp1.Replace("%29", ")");
                        temp1 = temp1.Replace("%7E", "~");
                        temp1 = temp1.Replace("%21", "!");
                        temp1 = temp1.Replace("%3B", ";");
                        temp1 = temp1.Replace("%26", "&");
                        temp1 = temp1.Replace("%27", "'");
                        temp1 = temp1.Replace("%3A", ";");
                        temp1 = temp1.Replace("%40", "@");
                        temp1 = temp1.Replace("%2B", "+");
                        temp1 = temp1.Replace("%3F", "!");
                        temp1 = temp1.Replace("%C3%97", "×");
                        temp1 = temp1.Replace("%CE%B8", "θ");
                        temp1 = temp1.Replace("%3D", "=");
                        temp1 = temp1.Replace("%2A", "×");
                        temp1 = temp1.Replace("%C3%A8", "e");
                        temp1 = temp1.Replace("%C3%A7", "c");
                        temp1 = temp1.Replace("%C2%BD", "½");
                        temp1 = temp1.Replace("%E2%80%99", "’");
                        dlya_sravn.Add(temp1);
                    }
                    int vh = 0;
                    string substr = "";
                    string sub_temp = "";
                    WebClient wb = new WebClient();
                    DataRow[] dr;
                    string file_save = "";
                    string sel = "";

                    int vhod_probel = 0;
                    for (int i = 0; i < dlya_sravn.Count; i++)
                    {
                        vh = dlya_sravn[i].IndexOf("Konachan.com");
                        if (vh != -1)
                        {
                            sub_temp = dlya_sravn[i].Substring(vh + 15);
                            vhod_probel = sub_temp.IndexOf(' ');
                            substr = sub_temp.Substring(0, vhod_probel);

                            file_save = dlya_sravn[i].Substring(vh);
                            sel = "Pic_name LIKE '*" + substr + "*'";
                            dr = pBDataSet.archive.Select(sel);
                            if (dr.Length == 0)
                            {
                                try
                                {
                                    if (!File.Exists(path_p2 + "\\" + file_save))
                                    {
                                        downloaded_p2 = false;
                                        wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged_p2);
                                        wb.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed_p2);
                                        //     wb.DownloadFile(spisok_strok[i], path + "\\" + file_save);
                                        wb.DownloadFileAsync(new Uri(spisok_strok[i]), path_p2 + "\\" + file_save);
                                        while (!downloaded_p2)
                                        {
                                            Thread.Sleep(300);
                                        }
                                        AddTextToList(file_save);
                                    }
                                    else
                                    {
                                        kol_exis++;
                                        Col_Exist_p2(Convert.ToString(kol_exis));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else
                            {
                                kol_sovp++;
                                KolSovpadenij_p2(Convert.ToString(kol_sovp));
                            }
                        }

                        all_row++;
                        All_down_p2(Convert.ToString(all_row));
                    }
                }
                catch (Exception ex)
                {
                    ErrMess(Convert.ToString(kk) + ":" + ex.Message);
                }
            }

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            vkl = false;
            try
            {
                th.Abort();
                th2.Abort();
                Application.Exit();
            }
            catch (Exception ex)
            {
                if (File.Exists("ErrorLog.txt"))
                {
                    StreamWriter str_wr = File.AppendText("ErrorLog.txt");
                    str_wr.Write(ex.Message);
                    str_wr.Write("\n");
                    str_wr.Close();
                }
                else
                {
                    StreamWriter str_wr = File.CreateText("ErrorLog.txt");
                    str_wr.Write(ex.Message);
                    str_wr.Write("\n");
                    str_wr.Close();
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress_1(e.ProgressPercentage);
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            downloaded = true;
        }
        private void ProgressChanged_p2(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress_1_p2(e.ProgressPercentage);
        }

        private void Completed_p2(object sender, AsyncCompletedEventArgs e)
        {
            downloaded_p2 = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            f_wait f_w = new f_wait();
            f_w.Owner = this;
            if(folderBrowserDialog3.ShowDialog() == DialogResult.OK)
            {
                f_w.Show();
                DirectoryInfo di = new DirectoryInfo(folderBrowserDialog3.SelectedPath);
                FileInfo[] file = di.GetFiles("*.*", SearchOption.AllDirectories);
                string temp1 = "";
                string path_name = "";
                for (int i = 0; i < file.Length; i++)
                {
                    temp1 = file[i].Name;
                    if (temp1.IndexOf('%') != -1)
                    {
                        path_name = file[i].DirectoryName;
                        temp1 = temp1.Replace("%20", " ");
                        temp1 = temp1.Replace("%28", "(");
                        temp1 = temp1.Replace("%29", ")");
                        temp1 = temp1.Replace("%7E", "~");
                        temp1 = temp1.Replace("%21", "!");
                        temp1 = temp1.Replace("%3B", ";");
                        temp1 = temp1.Replace("%26", "&");
                        temp1 = temp1.Replace("%27", "'");
                        temp1 = temp1.Replace("%3A", ";");
                        temp1 = temp1.Replace("%40", "@");
                        temp1 = temp1.Replace("%2B", "+");
                        temp1 = temp1.Replace("%3F", "!");
                        temp1 = temp1.Replace("%C3%97", "×");
                        temp1 = temp1.Replace("%CE%B8", "θ");
                        temp1 = temp1.Replace("%3D", "=");
                        temp1 = temp1.Replace("%2A", "×");
                        temp1 = temp1.Replace("%C3%A8", "e");
                        temp1 = temp1.Replace("%C3%A7", "c");
                        temp1 = temp1.Replace("%C2%BD", "½");
                        temp1 = temp1.Replace("%E2%80%99", "’");
                        path_name += "\\" + temp1;
                        file[i].MoveTo(path_name);
                    }
                    if (temp1.IndexOf('ç') != -1 || temp1.IndexOf('è') != -1 || temp1.IndexOf("") != -1 || temp1.IndexOf("%97") != -1 || temp1.IndexOf("×") != -1
                        || temp1.IndexOf("θ") != -1 || temp1.IndexOf("×") != -1 || temp1.IndexOf("’") != -1 || temp1.IndexOf("½") != -1)
                    {
                        path_name = file[i].DirectoryName;
                        temp1 = temp1.Replace('ç', 'c');
                        temp1 = temp1.Replace('è', 'e');
                        temp1 = temp1.Replace("", ".");
                        temp1 = temp1.Replace("×", "x");
                        temp1 = temp1.Replace("θ", "0");
                        temp1 = temp1.Replace("’", "'");
                        temp1 = temp1.Replace("½", "1-2");
                        path_name += "\\" + temp1;
                        file[i].MoveTo(path_name);
                    }

                }
                f_w.Close();
            }
        }
    }
}
