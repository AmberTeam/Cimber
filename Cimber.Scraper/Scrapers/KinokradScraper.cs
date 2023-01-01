using Cimber.Scraper.Models;
using HtmlAgilityPack;
using ShellProgressBar;

namespace Cimber.Scraper.Scrapers
{
    internal class KinokradScraper : BaseScraper
    {
        public override void Start()
        {
            try
            {
                try
                {
                    getFilms(Website.KINOKRAD);
                    var pagesCount = getPagesCount();
                    var options = new ProgressBarOptions { ProgressCharacter = '-' };

                    using (
                        var pbar = new ProgressBar(
                            pagesCount,
                            $"Scraping {Website.KINOKRAD}",
                            options
                        )
                    )
                    {
                        for (int i = 2; i <= pagesCount; i++)
                        {
                            var url = $"{Website.KINOKRAD}/page/{i}";
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

        protected override Film? getFilm(string url)
        {
            try
            {
                var document = getDocument(url)?.DocumentNode;
                var name = document?.SelectSingleNode(".//h1[@itemprop=\"name\"]").InnerText.Split(
                    "("
                )[0].Trim();
                var year = document?.SelectSingleNode(
                    ".//span[contains(concat(\" \",normalize-space(@class),\" \"),\" orange \")][contains(normalize-space(),\"Год:\")]/parent::*"
                ).InnerText.Split(":")[1].Trim();
                var countries = document
                    ?.SelectSingleNode(
                        ".//span[contains(concat(\" \",normalize-space(@class),\" \"),\" orange \")][contains(normalize-space(),\"Страна:\")]/parent::*"
                    ).InnerText.Split(":")[1].Trim()
                    .Split(" ")
                    .ToList();
                var genres = document
                    ?.SelectSingleNode(
                        @"
.//span[contains(concat("" "",normalize-space(@class),"" ""),"" orange "")][contains(normalize-space(),""Жанр:"")]/parent::*"
                    )
                    .SelectNodes(".//a")
                    .Select(a => a.InnerText)
                    .ToList();
                var duration = document?.SelectSingleNode(
                    @".//span[contains(concat("" "",normalize-space(@class),"" ""),"" orange "")][contains(normalize-space(),""Продолжительность:"")]/parent::*"
                ).InnerText.Split(":")[1].Trim();
                var description = document
                    ?.SelectSingleNode(".//div[@itemprop=\"description\"]")
                    .InnerText.Trim();
                var poster = document
                    ?.SelectSingleNode(".//img[@itemprop=\"image\"]").Attributes["src"]
                    .Value;
                var players = document
                    ?.SelectNodes(".//iframe")
                    .Where(i => i.Id != "frame_youtube")
                    .Select(i => i.Attributes["src"].Value.Trim())
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

        protected override HtmlNodeCollection? getLinks(string url)
        {
            try
            {
                var document = getDocument(url)?.DocumentNode;
                var nodes = document?.SelectNodes(
                    ".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" postertitle \")]/h2/a"
                );
                return nodes;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }

        protected override int getPagesCount()
        {
            try
            {
                var document = getDocument(Website.KINOKRAD)?.DocumentNode;
                string lastLink = document!
                    .SelectSingleNode(
                        @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" navcent "")]/child::*[last()]/self::a"
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
    }
}
