using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EasyTabs;
using Gecko;
using System.Xml;

namespace TestApp
{
    public partial class TabWindow : Form
    {
        protected TitleBarTabs ParentTabs
        {
            get
            {
                return (ParentForm as TitleBarTabs);
            }
        }

        public TabWindow()
        {
            InitializeComponent();
            //webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Text = webBrowser.DocumentTitle;

            if (webBrowser.Url.Scheme == "http")
            {
                try
                {
                    var url = "http://" + webBrowser.Url.Host + "/favicon.ico";
                    var path = Application.StartupPath + url.GetHashCode().ToString() + "favicon.ico";
                    Stream stream;
                    bool saveFile = false;
                    if (File.Exists(path))
                    {
                        stream = File.OpenRead(path);
                        saveFile = true;
                    }
                    else
                    {
                        WebRequest webRequest = WebRequest.Create(url);
                        WebResponse response = webRequest.GetResponse();
                        stream = response.GetResponseStream();
                    }

                    if (stream != null)
                    {
                        byte[] buffer = new byte[1024];

                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;

                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            stream.Close();
                            stream.Dispose();
                            ms.Seek(0, SeekOrigin.Begin);

                            Icon = new Icon(ms);
                            if (saveFile)
                            {
                                Icon.Save(File.OpenWrite(path));
                            }
                            ParentTabs.UpdateThumbnailPreviewIcon(ParentTabs.Tabs.Single(t => t.Content == this));
                            ParentTabs.RedrawTabs();
                        }
                    }
                }
                catch
                {
                    Icon = Resources.DefaultIcon;
                }
            }

            //Parent.Refresh();
        }

        void webBrowser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {


            Icon = Resources.DefaultIcon;
            Text = webBrowser.DocumentTitle;

            if (webBrowser.Url.Scheme == "http")
            {
                try
                {
                    var url = "http://" + webBrowser.Url.Host + "/favicon.ico";
                    var path = Application.StartupPath + url.GetHashCode().ToString() + "favicon.ico";
                    Stream stream;
                    bool saveFile = false;
                    if (File.Exists(path))
                    {
                        stream = File.OpenRead(path);
                        saveFile = true;
                    }
                    else
                    {
                        WebRequest webRequest = WebRequest.Create(url);
                        WebResponse response = webRequest.GetResponse();
                        stream = response.GetResponseStream();
                    }

                    if (stream != null)
                    {
                        byte[] buffer = new byte[1024];

                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;

                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            stream.Close();
                            stream.Dispose();
                            ms.Seek(0, SeekOrigin.Begin);

                            Icon = new Icon(ms);
                            if (saveFile)
                            {
                                Icon.Save(File.OpenWrite(path));
                            }
                            ParentTabs.UpdateThumbnailPreviewIcon(ParentTabs.Tabs.Single(t => t.Content == this));
                            ParentTabs.RedrawTabs();
                        }
                    }
                }
                catch
                {
                    Icon = Resources.DefaultIcon;
                }
            }

            //Parent.Refresh();
        }
        public void GoTo(string url)
        {

            if (!Regex.IsMatch(url, "^[a-zA-Z0-9]+\\://"))
            {
                url = "http://" + url;
            }


            webBrowser.Navigate(url);
        }

        private void TabWindow_Load(object sender, EventArgs e)
        {
            //this.webBrowser.CreateWindow += webBrowser_CreateWindow;
            this.webBrowser.NewWindow += webBrowser_NewWindow;
            this.webBrowser.ScriptErrorsSuppressed = false;
            this.webBrowser.ContextMenu = null;
            this.webBrowser.ContextMenuStrip = null;
            this.LoadLink();
        }
        void LoadLink()
        {
            foreach (var folder in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Links")))
            {
                ToolStripDropDownButton f = new ToolStripDropDownButton(folder.Split('\\').LastOrDefault(), Image.FromFile("icon.gif"));
                foreach (var path in Directory.GetFiles(folder, "*.url"))
                {
                    IniFile iniFile = new IniFile(path);
                    var iconUrl = iniFile.IniReadValue("InternetShortcut", "IconFile");
                    if (string.IsNullOrEmpty(iconUrl))
                    {
                        iconUrl = iniFile.IniReadValue("InternetShortcut", "URL");
                    }
                    ToolStripButton b = new ToolStripButton(Path.GetFileNameWithoutExtension(path), Image.FromFile("icon.gif"), (object o, EventArgs e) =>
                        {
                            var sender = (ToolStripButton)o;
                            GoTo(sender.ToolTipText);
                        }
                         , iniFile.IniReadValue("InternetShortcut", "URL"));
                    b.Width = 200;
                    b.ToolTipText = iniFile.IniReadValue("InternetShortcut", "URL");
                    f.DropDownItems.Add(b);

                }
                linkBar.Items.Add(f);
            }
        }
        private void addLink(String url, string name)
        {
            var linksXml = Application.StartupPath + "\\links.xml";
            //open the xml file 
            XmlDocument myXml = new XmlDocument();
            //and a new element to the xml file
            XmlElement el = myXml.CreateElement("link");
            el.SetAttribute("url", url);
            el.InnerText = name;

            if (!File.Exists(linksXml))
            {
                XmlElement root = myXml.CreateElement("links");
                myXml.AppendChild(root);
                root.AppendChild(el);
            }
            else
            {
                myXml.Load(linksXml);
                myXml.DocumentElement.AppendChild(el);
            }
            //if the links bar is visible then 
            //you have to add a ToolStripButton
            if (this.linkBar.Visible == true)
            {
                //create a new ToolStripButton object with the favicon image,
                //website name the click eventhandler to 
                //navigate to the specific web site               
                ToolStripButton b =
                          new ToolStripButton(el.InnerText, favicon(url, "test.jpg"), (object o, EventArgs e) =>
                          {
                              var sender = (ToolStripButton)o;
                              GoTo(sender.ToolTipText);
                          }
                           , el.GetAttribute("url"));
                b.ToolTipText = el.GetAttribute("url");
                //the MouseUp event is used 
                //for showing the context menu of this button 
                //b.MouseUp += new MouseEventHandler(b_MouseUp);
                linkBar.Items.Add(b);
            }

            myXml.Save(linksXml);
        }
        public Image favicon(String u, string file)
        {
            Uri url = new Uri(u);
            String iconurl = "http://" + url.Host + "/favicon.ico";

            WebRequest request = WebRequest.Create(iconurl);
            try
            {
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();
                return Image.FromStream(s);
            }
            catch (Exception ex)
            {
                //return a default icon in case 
                //the web site doesn`t have a favicon
                return Image.FromFile(file);
            }
        }
        void webBrowser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Keep popup new window here!
            e.Cancel = true;

            var testApp = (TestApp)this.ParentForm;
            testApp.GoTo(this.webBrowser.Url.ToString());

            //e.WebBrowser.Navigate(e.Uri);

            // OR

            //GeckoWebBrowser wb1 = new GeckoWebBrowser();
            //wb1.Navigating += new EventHandler<GeckoNavigatingEventArgs>(wb1_Navigating);
            //wb1.Dock = DockStyle.Fill;
            //wb1.CreateControl();
            //TabPage tab1 = new TabPage("New WebBrowser");
            //tabBrowser.TabPages.Add(tab1);
            //tab1.Controls.Add(wb1);
            //wb1.Navigate(e.Uri);
        }

        void webBrowser_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {

            //Keep popup new window here!
            e.Cancel = true;

            var testApp = (TestApp)this.ParentForm;
            testApp.GoTo(e.Uri);

            //e.WebBrowser.Navigate(e.Uri);

            // OR

            //GeckoWebBrowser wb1 = new GeckoWebBrowser();
            //wb1.Navigating += new EventHandler<GeckoNavigatingEventArgs>(wb1_Navigating);
            //wb1.Dock = DockStyle.Fill;
            //wb1.CreateControl();
            //TabPage tab1 = new TabPage("New WebBrowser");
            //tabBrowser.TabPages.Add(tab1);
            //tab1.Controls.Add(wb1);
            //wb1.Navigate(e.Uri);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.addLink(this.webBrowser.Url.ToString(), this.webBrowser.DocumentTitle);
        }


    }
}