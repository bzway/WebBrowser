using EasyTabs;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestApp
{
    public partial class TestApp : TitleBarTabs
    {
        public TestApp()
        {
            InitializeComponent();

            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
            Icon = Resources.DefaultIcon;
            MinimizeBox = false;
            MaximizeBox = false;
        }


        protected override void WndProc(ref Message m)
        {
            //if (m.Msg == Program.UserMessage)
            //{
            //    string str = Clipboard.GetText();
            //    foreach (var url in str.Split(';'))
            //    {
            //        GoTo(url);
            //    }
            //    return;
            //}
            base.WndProc(ref m);
        }
        public void GoTo(string url)
        {
            var win = new TabWindow();

            win.GoTo(url);
            var tab = new TitleBarTab(this)
            {
                Content = win
            };
            this.Tabs.Add(tab);
            this.SelectedTab = tab;
            ResizeTabContents();
        }
        public override TitleBarTab CreateTab()
        {
            var win = new TabWindow();

            win.GoTo("http://www.bing.com");
            return new TitleBarTab(this)
            {
                Content = win
            };
        }
    }
}
