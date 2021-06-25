using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Re_Os_Email_Template
{
    public partial class Form1 : Form
    {
        private string _selectedFolder;
        private string _selectedTemplate;
        private bool _onlyBrand;

        private const string BaseDirectory = "C:/Templates/";

        private List<string> _allFolders;
        private List<string> _templates;

        public Form1()
        {
            InitializeComponent();

            _allFolders = GetSubDirectories(BaseDirectory);

            comboBox1.DataSource = _allFolders;

            _templates = GetSubDirectories($"{BaseDirectory}{_allFolders.First()}/");

            comboBox2.DataSource = _templates;

            _onlyBrand = checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _onlyBrand = checkBox1.Checked;
        }

        private List<string> GetSubDirectories(string path)
        {
            string[] subDirectoryEntries = Directory.GetDirectories(path);

            var foldersName = subDirectoryEntries.Select(subDirectory => subDirectory.Substring(path.Length)).ToList();

            return foldersName;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedFolder = comboBox1.SelectedItem.ToString();

            _templates = GetSubDirectories($"{BaseDirectory}{_selectedFolder}/");

            comboBox2.DataSource = _templates;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _selectedTemplate = comboBox2.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(_selectedFolder))
                _selectedFolder = comboBox1.SelectedItem.ToString();

            if (_onlyBrand)
                _allFolders = _allFolders.Where(s => s.Contains("Brand")).ToList();

            var fullPath = $"{BaseDirectory}{_selectedFolder}/{_selectedTemplate}/tr.liquid";

            foreach (var folder in _allFolders)
            {
                if (!Directory.Exists($"{BaseDirectory}{folder}/{_selectedTemplate}"))
                    Directory.CreateDirectory($"{BaseDirectory}{folder}/{_selectedTemplate}");

                try
                {
                    File.Copy(fullPath, $"{BaseDirectory}{folder}/{_selectedTemplate}/tr.liquid", true);
                }
                catch (IOException exception)
                {
                    if (exception.Message.Contains("in use"))
                    {
                        var process = new Process
                        {
                            StartInfo =
                            {
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                FileName = "cmd.exe",
                                Arguments = "/C copy \"" + fullPath + "\" \"" +
                                            $"{BaseDirectory}{folder}/{_selectedTemplate}/tr.liquid" + "\""
                            }
                        };
                        process.Start();
                        Console.WriteLine(process.StandardOutput.ReadToEnd());
                        process.WaitForExit();
                        process.Close();
                    }
                }
            }

            MessageBox.Show(@"Seçilen Template Hepsine Uygulandı.", @"Bitti", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
