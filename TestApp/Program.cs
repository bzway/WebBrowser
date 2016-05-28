using System;
using System.Windows.Forms;
using EasyTabs;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
namespace TestApp
{
    public class IniFile
    {
        public string companyName, module, softname, version, sqlConstr;
        public string path;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
        string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
        int size, string filePath);
        public IniFile(string INIPath)
        {

            path = INIPath;

        }
        //该函数取得一切基础配置参数  
        //----------------------------------------------------------------------------------------------  
        public void getIni()
        {
            companyName = IniReadValue("setting", "companyName").ToString();
            module = IniReadValue("setting", "module").ToString();
            softname = IniReadValue("setting", "softname").ToString();
            version = IniReadValue("setting", "version").ToString();
            sqlConstr = "initial catalog=***;Server=***;User ID=***;Password=***;Connect Timeout=30";
        }
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }
        public string IniReadValue(string Section, string Key)
        {

            StringBuilder temp = new StringBuilder(1024);

            int i = GetPrivateProfileString(Section, Key, "", temp, 1024, this.path);

            return temp.ToString();

        }


    }  
    public static class Program
    {
        #region msg
        public const int USER = 0x0400;
        public const int UserMessage = USER + 1;
        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            //var ProcessList = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            //var ProcessList = Process.GetProcesses().Where(mbox => mbox.ProcessName.Contains("Test")).ToList();
            //MessageBox.Show(ProcessList.Count.ToString());
            //if (ProcessList.Count > 1)
            //{
            //    if (args != null)
            //    {
            //        var p = ProcessList.OrderBy(m => m.TotalProcessorTime).LastOrDefault();
            //        IntPtr hWnd = p.MainWindowHandle;

            //        string str = string.Join(";", args);
            //        Clipboard.SetText(str);
            //        SendMessage(hWnd, UserMessage, IntPtr.Zero, IntPtr.Zero);
            //    }
            //    return;
            //}
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TestApp testApp = new TestApp();
            testApp.GoTo("http://www.bing.com");
            TitleBarTabsApplicationContext applicationContext = new TitleBarTabsApplicationContext();
            applicationContext.Start(testApp);


            //testApp.Tabs.Add(
            //    new TitleBarTab(testApp)
            //        {
            //            Content = new TabWindow
            //                          {
            //                              Text = "New Tab"
            //                          }
            //        });
            //testApp.SelectedTabIndex = 0;


            Application.Run(applicationContext);
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    }
}
