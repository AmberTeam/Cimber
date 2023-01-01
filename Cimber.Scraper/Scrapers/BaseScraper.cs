using Cimber.Scraper.Models;
using HtmlAgilityPack;
using System.Text;

namespace Cimber.Scraper.Scrapers
{
    internal abstract class BaseScraper
    {
        protected int _retryCounts = 0;
        protected const int MAX_RETRY_COUNTS = 5;
        protected readonly Database _database;

        public BaseScraper()
        {
            _database = new Database();
        }

        protected HtmlDocument? getDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = web.Load(url);

            return doc;
        }

        protected virtual HtmlNodeCollection? getLinks(string url)
        {
            throw new NotImplementedException();
        }

        protected virtual int getPagesCount()
        {
            throw new NotImplementedException();
        }

        protected virtual void getFilms(string url)
        {
            throw new NotImplementedException();
        }

        protected virtual Film? getFilm(string url)
        {
            throw new NotImplementedException();
        }

        public virtual void Start() { }
    }
}
