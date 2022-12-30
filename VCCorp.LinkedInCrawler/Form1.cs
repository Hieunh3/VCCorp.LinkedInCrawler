using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.LinkedInCrawler.Controller;

namespace VCCorp.LinkedInCrawler
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser _browser = null;
        public Form1()
        {
            InitializeComponent();
            InitBrowser("https://www.linkedin.com/");
        }
        public void InitBrowser(string urlBase)
        {
            if (_browser == null)
            {
                this.WindowState = FormWindowState.Maximized;
                CefSettings s = new CefSettings();

                Cef.Initialize(s);
                _browser = new ChromiumWebBrowser(urlBase);
                this.panel1.Controls.Add(_browser);
                _browser.Dock = DockStyle.Fill;
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ParserLinkedIn parserLinkedIn = new ParserLinkedIn(_browser);
                await parserLinkedIn.CrawlData();

            }
            catch (Exception)
            {
            }
            await Task.Delay(TimeSpan.FromHours(1));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ParserLinkedIn_Profile parserLinkedIn = new ParserLinkedIn_Profile(_browser);
                await parserLinkedIn.CrawlData();

            }
            catch (Exception)
            {
            }
            await Task.Delay(TimeSpan.FromHours(1));
        }
    }
}
