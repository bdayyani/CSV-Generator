using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Flir.Atlas.Image;
using System.Globalization;
using MetroFramework;
using CsvHelper;

namespace CSV_Generator
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {

        public class ZRL_Info
        {
            public string Image { get; set; }
            public int Zone { get; set; }
            public int Riser { get; set; }
            public int Level { get; set; }
            public string Notes { get; set; }
            public string Conclusion { get; set; }
            public string Energy { get; set; }
        }

        public List<ZRL_Info> ZRL_List = new List<ZRL_Info>();

        public string Source_Folder;
        public string OutputFileName = "ZRL_Skeleton";

        public Form1()
        {
            InitializeComponent();
        }

        private void button_browse_Click(object sender, EventArgs e)
        {

            // Open all the thermal images
            listBox1.Items.Clear();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Open the folder containing Thermal images";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Source_Folder = fbd.SelectedPath;
                string[] files = Directory.GetFiles(Source_Folder);
                foreach (string file in files)
                {
                    try
                    {

                        if (Flir.Atlas.Image.ThermalImageFile.IsThermalImage(file))
                        {
                            string name = file.Replace((Source_Folder + "\\"), "");
                            name = name.Replace(".jpg", "");
                            listBox1.Items.Add(name);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            listBox1.SelectedIndex = 0;
        }

        public void UpdateZRL()
        {

            if (textBox_Name.Text != null && textBox_Name.Text != "")
            {
                OutputFileName = textBox_Name.Text;
            }

            string ExportFilePath = Source_Folder + "\\" + OutputFileName + ".csv";

            try
            {
                if (File.Exists(ExportFilePath))
                    File.Delete(ExportFilePath);

                int N = listBox1.Items.Count;
                //string Final_Report = "";
                for (int index = 0; index < N; index++)
                {
                    string Name = listBox1.Items[index].ToString();
                    ZRL_Info temp = new ZRL_Info();
                    temp.Image = Name;
                    ZRL_List.Add(temp);
                    Console.WriteLine(index);
                }

                using (var writer = new StreamWriter(ExportFilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ZRL_List);
                }


                MessageBox.Show("Export is Done\r\nOpen Folder to find the report", "Export Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception EX)
            {
                string Error = EX.ToString();
                if (Error.Contains("is being used by another process"))
                {
                    MessageBox.Show("Error: \rTargt file is open\rClose the target file", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error:\r\b" + EX, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            UpdateZRL();
        }

        private void button_openfolder_Click(object sender, EventArgs e)
        {
            Process.Start(Source_Folder);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect();
            Application.Exit();
        }
    }
}
