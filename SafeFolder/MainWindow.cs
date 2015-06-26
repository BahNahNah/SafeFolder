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
using System.IO;

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

        private string EncryptString(string obf8_, string encBase, string salt)
        {
            byte[] obf14_ = System.Text.Encoding.UTF8.GetBytes(obf8_);
            using (Rijndael obf9_ = new RijndaelManaged())
            {
                PasswordDeriveBytes obf16_ = new PasswordDeriveBytes(encBase, System.Text.Encoding.UTF8.GetBytes(salt));
                obf9_.Key = obf16_.GetBytes(obf9_.KeySize / 8);
                obf9_.IV = obf16_.GetBytes(obf9_.BlockSize / 8);
                using (MemoryStream obf15_ = new MemoryStream())
                {
                    using (CryptoStream obf17_ = new CryptoStream(obf15_, obf9_.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        obf17_.Write(obf14_, 0, obf14_.Length);
                    }
                    return Convert.ToBase64String(obf15_.ToArray());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == string.Empty)
            {
                MessageBox.Show("Enter a password.");
                return;
            }
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

            string[] FuncEnc = new string[] { "obf_str_enc_func_" };
            foreach (string funcS in FuncEnc)
            {
                string gname = GenStr(10, varGen);
                while (!varnameVerify.Add(gname))
                    gname = GenStr(10, varGen);
                s = s.Replace(funcS, gname);
            }

            string DescFuncName = GenStr(10, varGen);
            while (!varnameVerify.Add(DescFuncName))
                DescFuncName = GenStr(10, varGen);
            s = s.Replace("obf_str_dec_func_", DescFuncName);

            Dictionary<string, string> StrEnc = new Dictionary<string, string>()
            {
                {"[PWPROMPT]", "Password: "},
                {"[WRONGPROMPT]", "Wrong."},
                {"[DIRDATA]", "dirdata"},
                {"[ENTERTODECRYPT]", "Press enter to decrypt..."},
                {"[ENTERTOENCRYPT]", "Press enter to encrypt..."},
                {"[DONE]", "Done."},
                {"[BLACKSLASHB]", "\\b"},
                {"[PWMASK]", "*"},
                {"[ENCRYPTEDOUTPUT]","Encrypted: {0}"},
                {"[DECRYPTEDOUTPUT]","Decrypted: {0}"},
                {"[ERRORPARAMS]", "({0}) {1}"},
                {"[TIMES2]", "x2"},
                {"[FAILEDDELETEDIRDATA]", "Failed to delete dirdata, please manualy delete."},
                {"[LOADEDFILENAME]", "Loaded file name: {0}"},
                {"[CREDIT]", "SafeFolder by BahNahNah"},
                {"[PWHASH]", ShaHash(textBox1.Text)}
            };

            foreach(var strDic in StrEnc)
            {
                string salt = GenStr(5, cMap);
                string enc = EncryptString(strDic.Value, salt, SeedLabel.Text);
                string replace = string.Format("{0}(\"{1}\", \"{2}\")", DescFuncName, enc, salt);
                string from = string.Format("\"{0}\"", strDic.Key);
                s = s.Replace(from, replace);
            }

            s = s.Replace("[SALT]", SeedLabel.Text);
            //s = s.Replace("[PWHASH]", ShaHash(textBox1.Text));


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
