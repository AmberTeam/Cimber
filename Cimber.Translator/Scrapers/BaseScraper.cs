using HtmlAgilityPack;
using System.Text;

namespace Cimber.Translator.Scrapers
{
    internal class BaseScraper
    {
        protected HtmlDocument? getDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = web.Load(url);

            return doc;
        }
    }
}
