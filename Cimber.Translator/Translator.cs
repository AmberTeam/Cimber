using Cimber.Translator.Models;
using Cimber.Translator.Scrapers;
using ShellProgressBar;

namespace Cimber.Translator
{
    internal class Translator
    {
        private readonly Database _database;
        private readonly Language _language;

        public Translator(string path, string name, Language langauge)
        {
            _language = langauge;
            _database = new Database(path, name);
        }

        public void Start()
        {
            var films = _database.GetFilms();
            var options = new ProgressBarOptions { ProgressCharacter = '-' };

            using (var pbar = new ProgressBar(films.Count(), "Translating", options))
            {
                if (_language == Language.English)
                {
                    foreach (var film in films)
                    {
                        var imdbScraper = new ImdbScraper();
                        var enFilm = imdbScraper.GetEnglishFilm(film);

                        var enCountries = new List<string>();
                        foreach (var country in film.Countries)
                        {
                            enCountries.Add(
                                TranslationApi.Translate(
                                    Language.Russian,
                                    Language.English,
                                    country
                                )
                            );
                        }

                        var enGenres = new List<string>();
                        foreach (var genre in film.Genres)
                        {
                            enGenres.Add(
                                TranslationApi.Translate(Language.Russian, Language.English, genre)
                            );
                        }

                        var enDuration = TranslationApi.Translate(
                            Language.Russian,
                            Language.English,
                            film.Duration
                        );

                        if (enFilm != null)
                        {
                            enFilm.Countries = enCountries;
                            enFilm.Genres = enGenres;
                            enFilm.Duration = enDuration;
                            enFilm.Language = Language.English;

                            _database.AddEnglishFilm(enFilm);
                            pbar.Tick($"Last film: {enFilm.Name}({film.Id})");
                        }
                        else
                        {
                            var enName = TranslationApi.Translate(
                                Language.Russian,
                                Language.English,
                                film.Name
                            );
                            var description = TranslationApi.Translate(
                                Language.Russian,
                                Language.English,
                                film.Description
                            );

                            if (enName == null || description == null)
                                return;

                            _database.AddEnglishFilm(
                                new Film(
                                    enName,
                                    film.Year,
                                    description,
                                    enCountries,
                                    enDuration,
                                    enGenres,
                                    film.Poster,
                                    film.Players,
                                    film.Id,
                                    Language.English
                                )
                            );
                            pbar.Tick($"Last film: {enName ?? "Unknown"}({film.Id})");
                        }
                    }
                }
                else if (_language == Language.Ukrainian)
                {
                    foreach (var film in films)
                    {
                        var uaName = TranslationApi.Translate(
                            Language.Russian,
                            Language.Ukrainian,
                            film.Name
                        );
                        var uaDescription = TranslationApi.Translate(
                            Language.Russian,
                            Language.Ukrainian,
                            film.Description
                        );

                        var uaCountries = new List<string>();
                        foreach (var country in film.Countries)
                        {
                            uaCountries.Add(
                                TranslationApi.Translate(
                                    Language.Russian,
                                    Language.Ukrainian,
                                    country
                                )
                            );
                        }
                        var uaGenres = new List<string>();
                        foreach (var genre in film.Genres)
                        {
                            uaGenres.Add(
                                TranslationApi.Translate(
                                    Language.Russian,
                                    Language.Ukrainian,
                                    genre
                                )
                            );
                        }

                        var uaFilm = film;

                        if (uaFilm != null)
                        {
                            uaFilm.Countries = uaCountries;
                            uaFilm.Genres = uaGenres;
                            uaFilm.Name = uaName;
                            uaFilm.Description = uaDescription;
                            uaFilm.Language = Language.Ukrainian;

                            _database.AddUkrainianFilm(uaFilm);
                            pbar.Tick($"Last film: {uaFilm.Name}({film.Id})");
                        }
                        else
                        {
                            pbar.Tick("Didn't work");
                        }
                    }
                }
            }
        }
    }
}
