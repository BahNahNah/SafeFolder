using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace SafeFolder
{
    public partial class MainWindow : Form
    {
        Random r = new Random();
        string cMap = "1234567890!@#$%^&*()-=_+abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,./;'[]<>?:{}|";
        string varGen = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public MainWindow()
        {
            InitializeComponent();
        }

        private string GenStr(int len, string map)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
                sb.Append(map[r.Next(0, map.Length - 1)]);
            return sb.ToString();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            SeedLabel.Text = GenStr(30, cMap);
        }

        private void SeedLabel_MouseMove(object sender, MouseEventArgs e)
        {
            SeedLabel.Text = GenStr(30, cMap);
        }

        private string ShaHash(string strtohash)
        {
            using(SHA512 _sha = new SHA512Managed())
            {
                byte[] hashedBytes = _sha.ComputeHash(Encoding.UTF8.GetBytes(strtohash));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashedBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters parmiters = new CompilerParameters(new string[] { "mscorlib.dll", "System.Core.dll", "System.dll" });
            parmiters.GenerateExecutable = true;
            parmiters.CompilerOptions = "/target:exe /platform:x86";
            HashSet<string> varnameVerify = new HashSet<string>();

            string s = SafeFolder.Properties.Resources.SafeFolderModule;
            int i = 0;
            while (s.Contains(string.Format("obf{0}_", i)))
            {
                string gname = GenStr(10, varGen);
                while(!varnameVerify.Add(gname))
                    gname = GenStr(10, varGen);
                s = s.Replace(string.Format("obf{0}_", i), gname);
                i++;
            }

            s = s.Replace("[SALT]", SeedLabel.Text);
            s = s.Replace("[PWHASH]", ShaHash(textBox1.Text));

            using(SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Exe|*.exe";
                if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    parmiters.OutputAssembly = sfd.FileName;
                    CompilerResults r = compiler.CompileAssemblyFromSource(parmiters, s);
                    if (r.Errors.Count > 0)
                    {
                        foreach(CompilerError er in r.Errors)
                        {
                            Console.WriteLine("{0} {1}", er.Line, er.ErrorText);
                        }
                        MessageBox.Show("Failed");
                    }
                    else
                    {
                        MessageBox.Show("Built");
                    }
                }
            }

        }
    }
}
