using CefSharp.WinForms;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace VCCorp.LinkedInCrawler.Common
{
    public class Utilities
    {
        public static async Task<string> GetBrowserSource(ChromiumWebBrowser browser)
        {
            return await browser.GetMainFrame().GetSourceAsync();
        }
        public static readonly JsonSerializerOptions opt = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };
        public static string RemoveSpecialCharacter(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "";
            }

            return Regex.Replace(text, @"\t|\n|\r", " ");
        }
        public static async Task<string> EvaluateJavaScriptSync(string jsScript, ChromiumWebBrowser browser)
        {
            string jsonFromJS = null;
            if (browser.CanExecuteJavascriptInMainFrame && !String.IsNullOrEmpty(jsScript))
            {
                JavascriptResponse result = await browser.EvaluateScriptAsync(jsScript);

                if (result.Success)
                {
                    jsonFromJS = "";

                    if (result.Result != null)
                    {
                        jsonFromJS = result.Result.ToString();
                    }
                }
            }
            return jsonFromJS;
        }
        public static string RemoveEmojiFromText(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return Regex.Replace(value, @"\p{Cs}", ""); ;
            }

            return string.Empty;
        }
        public static string RemoveSpecialSign(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return Regex.Replace(value, @"\&nbsp;|\&amp;|\&lt;3", " "); ;
            }

            return string.Empty;
        }
    }
}
