using Cimber.Scraper.Models;
using Cimber.Scraper.Scrapers;

namespace Cimber.Scraper
{
    internal class Scraper
    {
        private readonly string _website;

        public Scraper(string website)
        {
            _website = website;
        }

        public void Start()
        {
            if (_website == Website.GIDONLINE)
            {
                GidonlineScraper scraper = new GidonlineScraper();
                scraper.Start();
            }
            else if (_website == Website.KINOKRAD)
            {
                KinokradScraper scraper = new KinokradScraper();
                scraper.Start();
            }
            else if (_website == Website.KINOGO)
            {
                KinogoScraper scraper = new KinogoScraper();
                scraper.Start();
            }
            else if (_website == Website.KINOPROFI)
            {
                KinoprofiScraper scraper = new KinoprofiScraper();
                scraper.Start();
            }
        }
    }
}
