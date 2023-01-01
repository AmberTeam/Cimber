using Cimber.Scraper.Models;
using HtmlAgilityPack;
using ShellProgressBar;

namespace Cimber.Scraper.Scrapers
{
    internal class GidonlineScraper : BaseScraper
    {
        public override void Start()
        {
            try
            {
                try
                {
                    getFilms(Website.GIDONLINE);
                    var pagesCount = getPagesCount();
                    var options = new ProgressBarOptions { ProgressCharacter = '-' };

                    using (
                        var pbar = new ProgressBar(
                            pagesCount,
                            $"Scraping {Website.GIDONLINE}",
                            options
                        )
                    )
                    {
                        for (int i = 2; i <= pagesCount; i++)
                        {
                            var url = $"{Website.GIDONLINE}/page/{i}";
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
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        protected override void getFilms(string url)
        {
            try
            {
                var links = getLinks(url);

                foreach (var link in links!)
                {
                    try
                    {
                        var film = getFilm(link.Attributes["href"].Value);

                        if (film != null)
                            if (film.Players.Count > 0)
                                _database.AddFilm(film);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }
                _retryCounts = 0;
            }
            catch (Exception ex)
            {
                _retryCounts++;
                Logger.Error(ex.ToString());

                if (_retryCounts <= MAX_RETRY_COUNTS)
                {
                    Logger.Warning(
                        $"Trying to rescrape the url({url}) {_retryCounts} times of {MAX_RETRY_COUNTS}"
                    );
                    getFilms(url);
                }
                else
                {
                    _retryCounts = 0;
                }
            }
        }

        protected override int getPagesCount()
        {
            try
            {
                var document = getDocument(Website.GIDONLINE)?.DocumentNode;
                string lastLink = document!
                    .SelectSingleNode(
                        @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" wp-pagenavi "")]/child::*[last()]/self::a"
                    )
                    .Attributes["href"].Value.Split("/page/")[1]
                    .Replace("/", "")
                    .Trim();

                return int.Parse(lastLink);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return 0;
            }
        }

        protected override HtmlNodeCollection? getLinks(string url)
        {
            try
            {
                var document = getDocument(url)?.DocumentNode;
                var nodes = document?.SelectNodes(
                    ".//a[contains(concat(\" \",normalize-space(@class),\" \"),\" mainlink \")]"
                );
                return nodes;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }

        protected override Film? getFilm(string url)
        {
            try
            {
                var document = getDocument(url)?.DocumentNode;
                var name = document?.SelectSingleNode(".//h1[@itemprop=\"name\"]").InnerText;
                var year = document
                    ?.SelectSingleNode(".//div[@itemprop=\"dateCreated\"]")
                    .SelectSingleNode(".//a")
                    .InnerText;
                var countries = document
                    ?.SelectSingleNode(".//div[@itemprop=\"countryOfOrigin\"]")
                    .SelectNodes(".//a")
                    .Select(a => a.InnerText)
                    .ToList();
                var genres = document
                    ?.SelectSingleNode(".//div[@itemprop=\"genre\"]")
                    .SelectNodes(".//a")
                    .Select(a => a.InnerText)
                    .ToList();
                var duration = document
                    ?.SelectSingleNode(".//div[@itemprop=\"duration\"]")
                    .InnerText;
                var description = document
                    ?.SelectSingleNode(".//div[@itemprop=\"description\"]")
                    .SelectSingleNode(".//p")
                    .InnerText.Trim();
                var poster =
                    Website.GIDONLINE
                    + document
                        ?.SelectSingleNode(".//img[@itemprop=\"image\"]").Attributes["src"]
                        .Value;
                var players = document
                    ?.SelectNodes(".//iframe")
                    .Select(i => i.Attributes["src"].Value.Split("?partner")[0].Trim())
                    .Select(i => i.StartsWith("https") ? i : $"https{i}")
                    .ToList();
                players!.RemoveAll(i => i.Contains("youtube"));

                return new Film(
                    name ?? "",
                    int.Parse(year ?? "0"),
                    description ?? "",
                    countries ?? new List<string>(),
                    duration ?? "",
                    genres ?? new List<string>(),
                    poster ?? "",
                    players ?? new List<string>(),
                    language: Language.Russian
                );
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }
    }
}
