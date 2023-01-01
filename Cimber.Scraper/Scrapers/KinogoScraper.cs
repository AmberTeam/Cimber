using Cimber.Scraper.Models;
using HtmlAgilityPack;
using ShellProgressBar;

namespace Cimber.Scraper.Scrapers
{
    internal class KinogoScraper : BaseScraper
    {
        public override void Start()
        {
            try
            {
                try
                {
                    getFilms(Website.KINOGO);
                    var pagesCount = getPagesCount();
                    var options = new ProgressBarOptions { ProgressCharacter = '-' };

                    using (
                        var pbar = new ProgressBar(
                            pagesCount,
                            $"Scraping {Website.KINOGO}",
                            options
                        )
                    )
                    {
                        for (int i = 2; i <= pagesCount; i++)
                        {
                            var url = $"{Website.KINOGO}/page/{i}";
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
                var name = document
                    ?.SelectSingleNode(
                        ".//div[contains(concat(\" \",normalize-space(@class),\" \"),\" kino-title-full \")]/h1"
                    )
                    .InnerText.Trim();
                var year = document?.SelectSingleNode(
                    @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" kino-lines "")]/ul/li/div[contains(concat("" "",normalize-space(@class),"" ""),"" k-label "")]/span[contains(normalize-space(),""Год выпуска:"")]/parent::*/parent::*"
                ).InnerText.Split(":")[1].Trim();
                var countries = document
                    ?.SelectSingleNode(
                        @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" kino-lines "")]/ul/li/div[contains(concat("" "",normalize-space(@class),"" ""),"" k-label "")]/span[contains(normalize-space(),""Страна:"")]/parent::*/parent::*"
                    ).InnerText.Split(":")[1].Trim()
                    .Split(",")
                    .ToList();
                var genres = document
                    ?.SelectSingleNode(@".//span[@itemprop=""genre""]")
                    .InnerText.Split(",")
                    .ToList();
                var duration = document
                    ?.SelectSingleNode(
                        @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" kino-lines "")]/ul/li/div[contains(concat("" "",normalize-space(@class),"" ""),"" k-label "")]/span[contains(normalize-space(),""Продолжительность:"")]/parent::*/parent::*"
                    )
                    ?.InnerText?.Split(":")[1]?.Trim();
                var description = document
                    ?.SelectSingleNode(".//span[@itemprop=\"description\"]")
                    .InnerText.Trim();
                var poster = document
                    ?.SelectSingleNode(".//img[@itemprop=\"image\"]").Attributes["src"]
                    .Value;
                var players = document
                    ?.SelectNodes(".//iframe")
                    .Select(i => i.Attributes["src"].Value.Trim())
                    .Select(i => i.StartsWith("https") ? i : $"https{i}")
                    .ToList();

                players!.RemoveAt(players.Count - 1);
                players!.RemoveAll(i => i.Contains("youtube"));

                return new Film(
                    name ?? "",
                    int.Parse(year ?? "0"),
                    description ?? "",
                    countries ?? new List<string>(),
                    duration ?? "",
                    genres ?? new List<string>(),
                    poster!.StartsWith("https") == false
                        ? $"{Website.KINOGO}{poster}"
                        : poster ?? "",
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
                    @".//div[contains(concat("" "",normalize-space(@class),"" ""),"" kino-title "")]/a[contains(concat("" "",normalize-space(@class),"" ""),"" kino-h "")]"
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
                var document = getDocument(Website.KINOGO)?.DocumentNode;
                string lastLink = document!
                    .SelectSingleNode(
                        @".//span[contains(concat("" "",normalize-space(@class),"" ""),"" navigation "")]/child::*[last()]/self::a"
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
