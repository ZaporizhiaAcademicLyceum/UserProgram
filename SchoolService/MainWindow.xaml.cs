using HTMLConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SchoolService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            newsFlowDoc.Blocks.Clear();
            string xamlStr = HtmlToXamlConverter.ConvertHtmlToXaml("<div><div><b>12.05.2017</b></div><div>{{ news.text }}<hr></div></div>", false);
            StringReader stringReader = new StringReader(xamlStr);
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
            Section sec = XamlReader.Load(xmlReader) as Section;
            while (sec.Blocks.Count > 0)
            {
                var block = sec.Blocks.FirstBlock;
                sec.Blocks.Remove(block);
                newsFlowDoc.Blocks.Add(block);
            }
        }

        public static TimeSpan GetUpTime()
        {
            return TimeSpan.FromMilliseconds(GetTickCount64());
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TelemetryData.ip.Clear();
            String strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    TelemetryData.ip.Add(ipaddress.ToString());
                }
            }
            TelemetryData.hostName = Environment.MachineName;
            TelemetryData.user = Environment.UserName + "\n  " + counter;
            TelemetryData.uptime = GetUpTime().ToString("d'd 'h'h 'm'm 's's'");

            hostNameValue.Content = TelemetryData.hostName;
            userValue.Content = TelemetryData.user;
            uptimeValue.Content = TelemetryData.uptime;
            ipValue.Content = String.Join("\n", TelemetryData.ip);

            counter++;
            // HttpRequest();
        }

        private void HttpRequest()
        {
            string html = string.Empty;

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("https://api.master-stvo.com/");
            using (HttpWebResponse response = (HttpWebResponse)myReq.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            Console.WriteLine(html);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
    }
}
