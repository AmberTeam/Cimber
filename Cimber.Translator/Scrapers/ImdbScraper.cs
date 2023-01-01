using Cimber.Translator.Models;

namespace Cimber.Translator.Scrapers
{
    internal class ImdbScraper : BaseScraper
    {
        private const string SEARCH_URL = "https://www.imdb.com/find?q=";

        public string? GetEnglishName(string name)
        {
            try
            {
                var document = getDocument($"{SEARCH_URL}{name}")?.DocumentNode;
                var englishName = document
                    ?.SelectSingleNode(
                        "/html/body/div[2]/main/div[2]/div[3]/section/div/div[1]/section[2]/div[2]/ul/li[1]/div[2]/div[1]/a"
                    )
                    ?.InnerText;
                return englishName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }

        public string? GetEnglishDescription(string name)
        {
            try
            {
                string link = getLink(name)!;
                var document = getDocument(link)?.DocumentNode;
                var description = document!
                    .SelectSingleNode(".//span[@data-testid=\"plot-xl\"]")
                    .InnerText;
                return description;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }

        private string? getLink(string name)
        {
            try
            {
                var document = getDocument($"{SEARCH_URL}{name}")?.DocumentNode;
                var link = document!
                    .SelectSingleNode(
                        "/html/body/div[2]/main/div[2]/div[3]/section/div/div[1]/section[2]/div[2]/ul/li[1]/div[2]/div[1]/a"
                    )
                    .Attributes["href"].Value;
                return $"https://imdb.com/{link}";
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }

        public Film? GetEnglishFilm(Film film)
        {
            try
            {
                var document = getDocument($"{SEARCH_URL}{film.Name}")?.DocumentNode;
                var link =
                    "https://imdb.com"
                    + document!
                        .SelectSingleNode(
                            "/html/body/div[2]/main/div[2]/div[3]/section/div/div[1]/section[2]/div[2]/ul/li[1]/div[2]/div[1]/a"
                        )
                        .Attributes["href"].Value;
                var name = document!.SelectSingleNode(
                    "/html/body/div[2]/main/div[2]/div[3]/section/div/div[1]/section[2]/div[2]/ul/li[1]/div[2]/div[1]/a"
                );

                var subdocument = getDocument(link)?.DocumentNode;
                var description = subdocument!
                    .SelectSingleNode(
                        "/html/body/div[2]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[1]/p/span[1]"
                    )
                    .InnerText;

                film.Description = description;
                film.Name = name.InnerText;

                return film;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return null;
            }
        }
    }
}
