using CefSharp.WinForms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VCCorp.LinkedInCrawler.Common;
using VCCorp.LinkedInCrawler.DAO;
using VCCorp.LinkedInCrawler.Model;
using static System.Windows.Forms.LinkLabel;

namespace VCCorp.LinkedInCrawler.Controller
{
    public class ParserLinkedIn_Profile
    {
        private ChromiumWebBrowser _browser = null;
        private readonly HtmlAgilityPack.HtmlDocument _document = new HtmlAgilityPack.HtmlDocument();
        private const string _jsLoadImg = @"document.getElementsByClassName('pv-top-card-profile-picture')[0].click()";
        private const string _jsLoadMoreCmt = @"document.getElementsByClassName('comments-comments-list__load-more-comments-button')[0].click()";


        public ParserLinkedIn_Profile(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        public async Task CrawlData()
        {
            await Crawler_Comment();
        }
        /// <summary>
        /// Thêm url vào tiktok_link table. (Bảng local)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<LinkedInPorfileCommentDTO>> Crawler_Comment()
        {
            List<LinkedInPorfileCommentDTO> linkedInComments = new List<LinkedInPorfileCommentDTO>();
            try
            {
                //lấy url trong db
                LinkedInCommentDAO contentDAO = new LinkedInCommentDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                List<LinkedInCommentDTO> dataUrl = contentDAO.GetLinkByDomain();
                contentDAO.Dispose();
                for (int i = 0; i < dataUrl.Count; i++)
                {
                    int status_link = dataUrl[i].status;
                    if (status_link ==0 )// check xem đã bóc hay chưa?
                    {
                        string url = "https://www.linkedin.com/in/" + dataUrl[i].author_id;
                        string author_id = dataUrl[i].author_id;
                        await _browser.LoadUrlAsync(url);
                        await Task.Delay(10_000);

                        string html = await Utilities.GetBrowserSource(_browser).ConfigureAwait(false);
                        _document.LoadHtml(html);
                        html = null;

                        LinkedInPorfileCommentDTO content = new LinkedInPorfileCommentDTO();
                        content.author_name = _document.DocumentNode.SelectSingleNode("//div[contains(@class,'pv-text-details__left-panel')]//h1")?.InnerText;
                        content.profile_pic = _document.DocumentNode.SelectSingleNode("//div[contains(@class,'pv-top-card__non-self-photo-wrapper')]//img")?.Attributes["src"].Value ?? "";
                        string rawfollowers = _document.DocumentNode.SelectSingleNode("//ul[contains(@class,'pv-top-card--list')]/li/span[contains(@class,'t-bold')]")?.InnerText ?? null;
                        if (rawfollowers == null)
                        {
                            content.followers = 0;
                        }
                        else
                        {
                            string numFollowers = Regex.Match(rawfollowers, @".*\d+").Value;
                            string getOnlyNum = numFollowers.Replace(",", string.Empty).ToLower();//loại bỏ dấu phẩy chỉ lấy số (ví dụ 21.9)
                            content.followers = int.Parse(getOnlyNum);
                        }
                        content.country = _document.DocumentNode.SelectSingleNode("//div[contains(@class,'pv-text-details__left-panel')]/span[1]")?.InnerText ?? "";
                        content.experiences = _document.DocumentNode.SelectSingleNode("//section[contains(@class,'artdeco-card')]//span[contains(@class,'mr1')]/span[1]")?.InnerText ?? "";

                        linkedInComments.Add(content);

                        LinkedInCommentDAO msql = new LinkedInCommentDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                        await msql.InsertCommentProfile(content);
                        msql.Dispose();

                        LinkedInCommentDAO msql1 = new LinkedInCommentDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                        await msql1.UpdateStatus(author_id);
                        msql1.Dispose();
                    }
                    string checkJs = await Common.Utilities.EvaluateJavaScriptSync(_jsLoadImg, _browser).ConfigureAwait(false);
                    if (checkJs == null)
                    {
                        break;
                    }
                }
            }
            catch { }
            return linkedInComments;
        }
    }
}
