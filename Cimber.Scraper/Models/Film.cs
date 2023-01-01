namespace Cimber.Scraper.Models
{
    internal class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public List<string> Countries { get; set; }
        public string Duration { get; set; }
        public List<string> Genres { get; set; }
        public string Poster { get; set; }
        public Language Language { get; set; }
        public List<string> Players { get; set; }

        public Film(
            string name,
            int year,
            string description,
            List<string> countries,
            string duration,
            List<string> genres,
            string poster,
            List<string> players,
            int id = 0,
            Language language = Language.Russian
        )
        {
            Id = id;
            Name = name;
            Year = year;
            Description = description;
            Countries = countries;
            Duration = duration;
            Genres = genres;
            Poster = poster;
            Language = language;
            Players = players;
        }
    }
}
