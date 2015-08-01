using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

static class obf1_
{
    static string cMap = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    static Random r = new Random();
    static Dictionary<string, string> obf19_ = new Dictionary<string, string>();
    static PasswordDeriveBytes obf21_;
    static byte[] obf23_ = new byte[10000];
    static string obf29_ = "";
    static Rijndael obf13_;
    public static void Main(string[] args)
    {
        obf29_ = Assembly.GetExecutingAssembly().Location;
        Console.WriteLine("[CREDIT]");
        Console.WriteLine();
        Console.Write("[PWPROMPT]");
        string obf12_ = obf34_();
        if(obf2_(obf12_) != "[PWHASH]")
        {
            Console.WriteLine("[WRONGPROMPT]");
            Console.ReadLine();
            return;
        }
        obf21_ = new PasswordDeriveBytes(obf12_, System.Text.Encoding.UTF8.GetBytes("[SALT]"));
        Console.Clear();

        DirectoryInfo obf18_ = new DirectoryInfo(Environment.CurrentDirectory);
        obf13_ = new RijndaelManaged();
        obf13_.Key = obf21_.GetBytes(obf13_.KeySize / 8);
        obf13_.IV = obf21_.GetBytes(obf13_.BlockSize / 8);
        Console.WriteLine("Dir: {0}", obf18_.FullName);

        if (File.Exists("[DIRDATA]"))
        {
            Console.WriteLine("[ENTERTODECRYPT]");
            Console.ReadLine();
            using (StreamReader sr = new StreamReader("[DIRDATA]"))
            {
                string line = string.Empty;
                while((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        line = obf_str_dec_func_(line, obf12_);
                        string[] spl = line.Split(':');
                        obf19_[spl[0]] = spl[1];
                        Console.WriteLine("[LOADEDFILENAME]", line);
                    }
                    catch
                    {

                    }
                }
            }
            obf25_(obf18_, false);
			try
            {
                File.Delete("[DIRDATA]");
            }
			catch
            {
                Console.WriteLine("[FAILEDDELETEDIRDATA]");
            }
        }
        else
        {
            Console.WriteLine("[ENTERTOENCRYPT]");
            Console.ReadLine();
            obf26_(obf18_, false);
            using (StreamWriter sw = new StreamWriter("[DIRDATA]"))
            {
                foreach (var k in obf19_)
                {
                    sw.WriteLine(obf_str_enc_func_(string.Format("{0}:{1}", k.Key, k.Value), obf12_));
                    sw.Flush();
                }
                sw.Close();
            }
            FileInfo fi = new FileInfo("[DIRDATA]");
            fi.Attributes = FileAttributes.Hidden | FileAttributes.System;
        }
        Console.WriteLine("[DONE]");
        Console.ReadLine();
    }
    private static string obf0_(int len)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < len; i++)
            sb.Append(cMap[r.Next(0, cMap.Length - 1)]);
        return sb.ToString();
    }
    private static string obf34_()
    {
        ConsoleKeyInfo obf10_;
        string obf11_ = string.Empty;
        while((obf10_ = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (obf10_.Key == ConsoleKey.Backspace )
            {
                if (obf11_ != string.Empty && obf11_.Length > 0)
                {
                    obf11_ = obf11_.Substring(0, obf11_.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                Console.Write("[PWMASK]");
                obf11_ += obf10_.KeyChar;
            }
        }
        return obf11_;
    }

    private static void obf25_(DirectoryInfo obf20_, bool efile)
    {
        foreach (FileInfo obf28_ in obf20_.GetFiles())
        {
            try
            {
                if (obf28_.FullName == obf29_)
                    continue;
                string obf31_ = obf28_.Name;
                string obf32_ = Path.GetDirectoryName(obf28_.FullName);
                string fpath = string.Empty;
                if (obf19_.ContainsKey(obf31_))
                    fpath = Path.Combine(obf32_, obf19_[obf31_]);
                else
                    continue;
                FileStream obf22_ = obf28_.Open(FileMode.Open, FileAccess.ReadWrite);
                
                using (FileStream dec_fs = new FileStream(Path.Combine(obf32_, fpath), FileMode.Create))
                {
                    using (CryptoStream obf27_ = new CryptoStream(dec_fs, obf13_.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        int obf24_;
                        while ((obf24_ = obf22_.Read(obf23_, 0, obf23_.Length)) != 0)
                        {
                            obf27_.Write(obf23_, 0, obf24_);
                            obf27_.Flush();
                        }
                        obf27_.Close();
                    }
                }
                obf22_.Close();
                obf22_.Dispose();
                obf28_.Delete();
                Console.WriteLine("[DECRYPTEDOUTPUT]", fpath);
                string cName = obf20_.Name;
                if (obf19_.ContainsKey(cName) && efile)
                {
                    string fPathNew = Path.Combine(obf20_.FullName.Substring(0, obf20_.FullName.Length - obf20_.Name.Length), obf19_[cName]);
                    obf20_.MoveTo(fPathNew);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERRORPARAMS]", ex.Message, obf28_.FullName);
            }
        }
        foreach (DirectoryInfo di in obf20_.GetDirectories())
            obf25_(di, true);
    }

    private static void obf26_(DirectoryInfo obf20_, bool efile)
    {
        foreach (FileInfo obf28_ in obf20_.GetFiles())
        {
            try
            {
                if (obf28_.FullName == obf29_)
                    continue;
                string obf30_ = obf0_(10) + ".dat";
                string obf31_ = obf28_.Name;
                string obf32_ = Path.GetDirectoryName(obf28_.FullName);
                while (obf19_.ContainsKey(obf30_))
                    obf30_ = obf0_(10) + ".dat";
                obf19_[obf30_] = obf31_;
                FileStream obf22_ = obf28_.Open(FileMode.Open, FileAccess.ReadWrite);
                using (FileStream enc_fs = new FileStream(Path.Combine(obf32_, obf30_), FileMode.Create))
                {
                    using (CryptoStream obf27_ = new CryptoStream(enc_fs, obf13_.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        int obf24_ = 0;
                        while ((obf24_ = obf22_.Read(obf23_, 0, obf23_.Length)) != 0)
                        {
                            obf27_.Write(obf23_, 0, obf24_);
                            obf27_.Flush();
                        }
                    }
                    enc_fs.Close();
                }
                obf22_.Close();
                obf22_.Dispose();
                obf28_.Delete();
                Console.WriteLine("[ENCRYPTEDOUTPUT]", obf28_.FullName);
                if (efile)
                {
                    string obf33_ = obf0_(10) + "_dir";
                    while (obf19_.ContainsKey(obf33_))
                        obf33_ = obf0_(10) + "_dir";
                    obf19_[obf33_] = obf20_.Name;
                    string fPathNew = Path.Combine(obf20_.FullName.Substring(0, obf20_.FullName.Length - obf20_.Name.Length), obf33_);
                    obf20_.MoveTo(fPathNew);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        foreach (DirectoryInfo di in obf20_.GetDirectories())
            obf26_(di, true);
    }
    private static string obf2_(string obf5_)
    {
        using (SHA512 obf3_ = new SHA512Managed())
        {
            byte[] obf4_ = obf3_.ComputeHash(Encoding.UTF8.GetBytes(obf5_));
            StringBuilder obf6_ = new StringBuilder();
            foreach (byte obf7_ in obf4_)
                obf6_.Append(obf7_.ToString("[TIMES2]"));
            return obf6_.ToString();
        }
    }

    private static string obf_str_dec_func_(string obf8_, string encBase)
    {
        byte[] obf14_ = Convert.FromBase64String(obf8_);
        using (Rijndael obf9_ = new RijndaelManaged())
        {
            PasswordDeriveBytes obf16_ = new PasswordDeriveBytes(encBase, System.Text.Encoding.UTF8.GetBytes("[SALT]"));
            obf9_.Key = obf16_.GetBytes(obf9_.KeySize / 8);
            obf9_.IV = obf16_.GetBytes(obf9_.BlockSize / 8);
            using(MemoryStream obf15_ = new MemoryStream())
            {
                using (CryptoStream obf17_ = new CryptoStream(obf15_, obf9_.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    obf17_.Write(obf14_, 0, obf14_.Length);
                }
                return System.Text.Encoding.UTF8.GetString(obf15_.ToArray());
            }
        }
    }

    private static string obf_str_enc_func_(string obf8_, string encBase)
    {
        byte[] obf14_ = System.Text.Encoding.UTF8.GetBytes(obf8_);
        using (Rijndael obf9_ = new RijndaelManaged())
        {
            PasswordDeriveBytes obf16_ = new PasswordDeriveBytes(encBase, System.Text.Encoding.UTF8.GetBytes("[SALT]"));
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
}


