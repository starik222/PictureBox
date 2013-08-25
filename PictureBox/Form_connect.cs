using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace PictureBox
{
    public partial class Form_connect : Form
    {
        public Form_connect()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                SqlDataSourceEnumerator MyServers = SqlDataSourceEnumerator.Instance;
                DataTable ServerTable = MyServers.GetDataSources();
                foreach (DataRow row in ServerTable.Rows)
                {
                    if (row[1].ToString() == "")
                    {
                        comboBox3.Items.Add(row[0].ToString());
                    }
                    else
                    {
                        comboBox3.Items.Add(row[0].ToString() + "\\" + row[1].ToString());
                    }
                }
            }
            else
            {
                if ((MessageBox.Show("Дальнейшие действия приведут к потери данных о текущем сервере, хотите продолжить?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) == DialogResult.Yes)
                {
                    SqlDataSourceEnumerator MyServers = SqlDataSourceEnumerator.Instance;
                    DataTable ServerTable = MyServers.GetDataSources();
                    foreach (DataRow row in ServerTable.Rows)
                    {
                        if (row[1].ToString() == "")
                        {
                            comboBox3.Items.Add(row[0].ToString());
                        }
                        else
                        {
                            comboBox3.Items.Add(row[0].ToString() + "\\" + row[1].ToString());
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string first = "Data Source=" + comboBox3.Text + ";";
            if (comboBox1.Text == "Да")
            {
                first = first + "AttachDbFilename=" + textBox2.Text + ";";
            }
            //first = first + "Database=master;";
            if (comboBox2.Text != "Аутентификация Windows")
            {
                first = first + "Integrated Security=false;";
                first = first + "User ID=" + textBox4.Text + ";";
                first = first + "Password=" + textBox5.Text + ";";
            }
            else
            {
                first = first + "Integrated Security=true;";
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text != "")
            {
                // Create a new connection to the selected server name
                string con = "Data Source=" + comboBox3.Text + ";";
                // ServerConnection srvConn = new ServerConnection(comboBox3.Text);
                // Log in using SQL authentication instead of Windows authentication
                if (comboBox2.Text == "Аутентификация Windows")
                {
                    con += "Integrated Security=true;";
                }
                else
                {
                    con += "Integrated Security=false;";
                    con += "User ID=" + textBox4.Text + ";";
                    con += "Password=" + textBox5.Text + ";";
                }
                // Give the login username
                //  srvConn.Login = txtUsername.Text;
                // Give the login password
                // srvConn.Password = txtPassword.Text;
                // Create a new SQL Server object using the connection we created
                // Loop through the databases list
                comboBox4.Items.Clear();
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(con))
                    {
                        sqlConn.Open();
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = "sp_helpdb";
                        using (SqlDataReader sqlDR = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            int col_name = sqlDR.GetOrdinal("name");
                            while (sqlDR.Read())
                            {
                                comboBox4.Items.Add(sqlDR.GetString(col_name));
                            }
                        }
                    }









                    /*  foreach (Database dbServer in srvSql.Databases)
                      {
                          // Add database to combobox
                          comboBox4.Items.Add(dbServer.Name);
                      }*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // A server was not selected, show an error message
                MessageBox.Show("Please select a server first", "Server Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Form_connect_Load(object sender, EventArgs e)
        {
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg != null)
            {
                comboBox3.Text = (string)myreg.GetValue("Server");
                comboBox1.SelectedIndex = comboBox1.FindStringExact((string)myreg.GetValue("dbf"));
                /*  if ((string)myreg.GetValue("dbf") == "Да")
                  {
                      button1.Enabled = true;
                      textBox2.Enabled = true;
                  }
                  else
                  {
                      button1.Enabled = false;
                      textBox2.Enabled = false;
                  }*/

                textBox2.Text = (string)myreg.GetValue("path_dbf");
                if ((string)myreg.GetValue("dbf") != "Да")
                {
                    comboBox4.Text = (string)myreg.GetValue("database");
                }

                comboBox2.SelectedIndex = comboBox2.FindStringExact((string)myreg.GetValue("ayth"));
                /*if ((string)myreg.GetValue("ayth") == "Аутентификация Windows")
                {
                    textBox4.Enabled = false;
                    textBox5.Enabled = false;
                }
                else
                {
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                }*/
                textBox4.Text = (string)myreg.GetValue("login");
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
                    textBox5.Text = new_pass;
                }

            }
            else
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            myreg.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistryKey myreg;
            myreg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureBox\\Setting", true);
            if (myreg == null)
            {
                myreg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\PictureBox\\Setting");
            }
            myreg.SetValue("Server", comboBox3.Text);
            myreg.SetValue("dbf", comboBox1.Text);
            myreg.SetValue("path_dbf", textBox2.Text);
            myreg.SetValue("database", comboBox4.Text);
            myreg.SetValue("ayth", comboBox2.Text);
            myreg.SetValue("login", textBox4.Text);
            string pass = "";
            string new_pass = "";
            pass = textBox5.Text;
            int ppp;
            for (int i = 0; i < pass.Length; i++)
            {
                ppp = (int)pass[i] + i;
                new_pass += (char)ppp;
            }
            myreg.SetValue("pass", new_pass);
            myreg.Close();
            //  if (f_closed)
            //{
            //     Close();
            //  }
            Application.Restart();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.ComboBox)(sender)).SelectedIndex == 0)
            {
                textBox1.Enabled = false;
                button1.Enabled = true;
                textBox2.Enabled = true;
                button7.Enabled = false;
                comboBox4.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                button7.Enabled = true;
                button1.Enabled = false;
                textBox2.Enabled = false;
                textBox1.Enabled = true;
                comboBox4.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.ComboBox)(sender)).SelectedIndex == 0)
            {
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }
            else
            {
                textBox4.Enabled = true;
                textBox5.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                PBDataSetTableAdapters.archiveTableAdapter at = new PBDataSetTableAdapters.archiveTableAdapter();
                at.GetCount();
                MessageBox.Show("соединение успешно установлено");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
