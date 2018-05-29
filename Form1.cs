using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.IO;
using System.Reflection;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WebBrowser_HTML_File_CS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling alt tab

                if (objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags))
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }
        string curDir = Directory.GetCurrentDirectory();
        private void Form1_Load(object sender, EventArgs e)
        {
            
            int index = curDir.IndexOf("bin");
            if (index > 0)
                curDir = curDir.Substring(0, index);
            string str = String.Format("file:\\"+ curDir + "HTML.htm");
            webBrowser1.Navigate((new Uri(str)).AbsoluteUri);
            //uri - file:///your_directory/WebBrowser_HTML_File_CS/HTML.htm
            //   webBrowser1.Navigate((new Uri(String.Format("file:///{{0}}/WebBrowser_HTML_File_CS/HTML.htm",curDir))).AbsoluteUri);
            //  this.TopMost = true;
            //  this.FormBorderStyle = FormBorderStyle.None;
            //  this.WindowState = FormWindowState.Maximized;
            //string path = Path.GetPathRoot(Environment.SystemDirectory);
            //string strng = "";
            //System.IO.DriveInfo di = new System.IO.DriveInfo(@"C:\Windows\");
            //System.IO.DirectoryInfo dirInfo = di.RootDirectory;
            //Console.WriteLine(dirInfo.Attributes.ToString());

            //// Get the files in the directory and print out some information about them.
            //System.IO.FileInfo[] fileNames = dirInfo.GetFiles("*.*");
            //if (path.Contains("C"))
            //{
            //   // local = ;
            //    DirectoryInfo d = new DirectoryInfo(@"C:\Windows\");
            //    FileInfo[] Files = d.GetFiles("*-*"); //Getting Text files
                
            //    foreach (FileInfo file in Files)
            //    {
            //        strng = strng + ", " + file.Name;
            //    }

            //}
            //else
            //{
            //    DirectoryInfo d = new DirectoryInfo(@"D:\Windows\");
            //    FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
               
            //    foreach (FileInfo file in Files)
            //    {
            //        strng = strng + ", " + file.Name;
            //    }
            //}         
            //path = path.Remove(path.Length - 1);
            //   this.KeyPress += new KeyPressEventHandler(this.Form1_KeyPress);
            //  this.KeyDown += new KeyEventHandler(this.Form1_KeyDown);
            //Assuming Test is your Folder


            // string[] dirs = Directory.GetFiles(local, "-", SearchOption.AllDirectories);
            //  List<string> singleDirNames = dirs.Where(x => x.Count() == 4).Select(x => Path.GetDirectoryName(x)).Distinct().ToList();
            //var files = Directory.EnumerateDirectories(local);//EnumerateDirectories(local).SelectMany(
            //    directory => Directory.EnumerateFiles(directory));

            webBrowser1.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(LoadedDocument);
         
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
        
    }
        private void LoadedDocument(object sender, WebBrowserDocumentCompletedEventArgs e)
        {            
            //after page loaded select and change dom element
            string culturesList = "";
            HtmlElement tableElem = webBrowser1.Document.GetElementById("language_list");

            CultureInfo ci = CultureInfo.InstalledUICulture;
            culturesList = culturesList + "<li>" + ci.EnglishName + "</li>";
            tableElem.InnerHtml = culturesList;            
        }
       // Keypress only handles keys in the ascii range
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("KeyPress: " + (int)e.KeyChar);
        }

        // Keydown will work for all keys
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("KeyDown: " + e.KeyCode);
        }
        protected override void OnLoad(EventArgs e)
       {
            // base.OnLoad(e);
            // HtmlElement vid = webBrowser1.Document.GetElementById("video_id");
             string str = String.Format("file:\\" + curDir + "Media\\SampleVideo_1280x720_1mb.mp4");
            // var embed = "<iframe width=\"300\" src=\"{0}\"" +
            // "frameborder = \"0\" allow = \"autoplay; encrypted-media\" allowfullscreen></iframe>";
            //// webBrowser1.Navigate((new Uri(str)).AbsoluteUri);
            //// vid.InnerHtml= string.Format(embed, (new Uri(str)).AbsoluteUri);

            //  var url = "https://www.youtube.com/embed/L6ZgzJKfERM";
            // vid.InnerHtml = string.Format(embed, url);
            //  this.webBrowser1.DocumentText = string.Format(embed, url);
            base.OnLoad(e);
        var embed = "<html><head>" +
        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>" +
        "</head><body><ol id='language_list'></ol>" +
        "<iframe width=\"300\" src=\"{0}\"" +
        "frameborder = \"0\" allow = \"autoplay; encrypted-media\" allowfullscreen></iframe>" +
        "</body></html>";
       // var url = "https://www.youtube.com/embed/L6ZgzJKfERM";
            this.webBrowser1.DocumentText = string.Format(embed, new Uri(str));
        }
    }
}
