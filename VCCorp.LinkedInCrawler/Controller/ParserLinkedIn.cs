using CefSharp.WinForms;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.LinkedInCrawler.Common;
using VCCorp.LinkedInCrawler.DAO;
using VCCorp.LinkedInCrawler.Model;

namespace VCCorp.LinkedInCrawler.Controller
{
    public class ParserLinkedIn
    {
        private ChromiumWebBrowser _browser = null;
        private readonly HtmlAgilityPack.HtmlDocument _document = new HtmlAgilityPack.HtmlDocument();
        private const string _jsLoadMoreReplyCmt = @"document.getElementsByClassName('show-prev-replies')[0].click()";
        private const string _jsLoadMoreCmt = @"document.getElementsByClassName('comments-comments-list__load-more-comments-button')[0].click()";


        public ParserLinkedIn(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        public async Task CrawlData()
        {
            await Crawler_Comment("https://www.linkedin.com/posts/dam-thi-thu-trang_t%C3%A0i-li%E1%BB%87u-hay-th%E1%BA%BF-n%C3%A0y-m%C3%A0-%C4%91%E1%BB%8Dc-1-m%C3%ACnh-th%C3%AC-%C3%ADch-activity-7008726947186819072-PmBu/");
        }
        /// <summary>
        /// Thêm url vào tiktok_link table. (Bảng local)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<LinkedInCommentDTO>> Crawler_Comment(string url)
        {
            List<LinkedInCommentDTO> linkedInComments = new List<LinkedInCommentDTO>();
            ushort indexLastContent = 0;
            ushort indexLastContent1 = 0;
            try
            {
                await _browser.LoadUrlAsync(url);
                await Task.Delay(20_000);
                while (true)
                {
                    //check JS nút xem thêm
                    //await Utilities.EvaluateJavaScriptSync(_jsLoadMoreReplyCmt, _browser).ConfigureAwait(false);
                    //await Task.Delay(5_000);

                    string html = await Utilities.GetBrowserSource(_browser).ConfigureAwait(false);
                    _document.LoadHtml(html);
                    html = null;

                    HtmlNodeCollection divComment = _document.DocumentNode.SelectNodes($"//article[contains(@class,'comments-comments-list__comment-item')][position()>{indexLastContent}]");
                    if (divComment == null)
                    {
                        break;
                    }
                    if (divComment != null)
                    {
                        foreach (HtmlNode item in divComment)
                        {
                            string urlAuthor = item.SelectSingleNode(".//span[contains(@class,'comments-post-meta__name-text')]")?.Attributes["data-entity-hovercard-id"].Value;
                            string author_id = Regex.Match(urlAuthor, @"(?<=\:fs_miniProfile\:)[\w\W]+").Value; // lấy id_author

                            LinkedInCommentDTO content = new LinkedInCommentDTO();
                            content.author_id = author_id;
                            content.author_name = item.SelectSingleNode(".//span[contains(@class,'comments-post-meta__name-text')]//span/span[1]")?.InnerText;
                            content.job = item.SelectSingleNode(".//span[contains(@class,'comments-post-meta__headline')]")?.InnerText;
                            string comment = Utilities.RemoveSpecialCharacter(item.SelectSingleNode(".//div[contains(@class,'update-components-text')]//span[contains(@dir,'ltr')]")?.InnerText);
                            content.comment = Utilities.RemoveEmojiFromText(Utilities.RemoveSpecialSign(comment));
                            linkedInComments.Add(content);

                            LinkedInCommentDAO msql = new LinkedInCommentDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                            await msql.InsertCommentContent(content);
                            msql.Dispose();

                            indexLastContent++;

                            //HtmlNodeCollection divComment1 = _document.DocumentNode.SelectNodes($"//article[contains(@class,'comments-reply-item')][position()>{indexLastContent1}]");
                            //if (divComment1 == null)
                            //{
                            //    break;
                            //}
                            //if (divComment1 != null)
                            //{
                            //    foreach (HtmlNode item1 in divComment1)
                            //    {
                            //        string urlAuthor1 = item1.SelectSingleNode(".//span[contains(@class,'comments-post-meta__name-text')]")?.Attributes["data-entity-hovercard-id"].Value;
                            //        string author_id1 = Regex.Match(urlAuthor1, @"(?<=\:fs_miniProfile\:)[\w\W]+").Value; // lấy id_author

                            //        LinkedInCommentDTO content1 = new LinkedInCommentDTO();
                            //        content1.author_id = author_id1;
                            //        string comment = Utilities.RemoveSpecialCharacter(item1.SelectSingleNode(".//div[contains(@class,'update-components-text')]//span[contains(@dir,'ltr')]")?.InnerText);
                            //        content1.comment = Utilities.RemoveEmojiFromText(Utilities.RemoveSpecialSign(comment));
                            //        linkedInComments.Add(content1);

                            //        LinkedInCommentDAO msql1 = new LinkedInCommentDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                            //        await msql1.InsertCommentContent(content1);
                            //        msql1.Dispose();

                            //        indexLastContent1++;
                            //    }
                            //}
                        }
                    }
                    //check JS roll xuống cuối trang
                    string checkJs = await Common.Utilities.EvaluateJavaScriptSync(_jsLoadMoreCmt, _browser).ConfigureAwait(false);
                    if (checkJs == null)
                    {
                        break;
                    }
                    await Task.Delay(10_000);
                }
            }
            catch { }
            return linkedInComments;
        }

    }
}
