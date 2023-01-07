namespace Cimber.Scraper.Scrapers
{
    public class UakinogoScraper : BaseScraper
    {
        public override void Start()
        {
            try
            {
                try
                {
                    getFilms(Website.UAKINOGO);
                    var pagesCount = getPagesCount();
                    var options = new ProgressBarOptions { ProgressCharacter = '-' };

                    using (
                        var pbar = new ProgressBar(
                            pagesCount,
                            $"Scraping {Website.UAKINOGO}",
                            options
                        )
                    )
                    {
                        for (int i = 2; i <= pagesCount; i++)
                        {
                            var url = $"{Website.UAKINOGO}/page/{i}";
                            pbar.Tick($"Scraping url({url}) page {i} of {pagesCount}");
                            getFilms(url);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Start();
                    Logger.Error(ex.ToString());
                }
            }
            catch (Exception)
            {
                Logger.Error(ex.ToString());
            }
        }

        protected override Film? getFilm(string url)
        {

        }

        protected override void getFilms(string url)
        {
            base.getFilms(url);
        }

        protected override HtmlNodeCollection? getLinks(string url)
        {
            return base.getLinks(url);
        }

        protected override int getPagesCount()
        {
            try
            {
                var document = getDocument(Website.UAKINOGO)?.DocumentNode;
                string lastLink = document!
                    .SelectSingleNode(
                        @"/html/body/div[1]/div/div[2]/main/section/div[2]/div/div/div[2]/a[10]"
                    )
                    .innerText
                    .Trim();

                return int.Parse(lastLink);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return 0;
            }
        }
    }
}