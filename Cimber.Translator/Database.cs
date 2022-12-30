using Cimber.Translator.Models;
using Newtonsoft.Json;
using System.Data.SQLite;

namespace Cimber.Translator
{
    internal class Database
    {
        private readonly SQLiteConnection _readConnection;
        private readonly string _databaseName;

        public Database(string path, string name)
        {
            _databaseName = name;

            string readConnectionString = $"URI=file:{path}";

            _readConnection = new SQLiteConnection(readConnectionString);
            _readConnection.Open();
        }

        ~Database()
        {
            _readConnection.Close();
        }

        public IEnumerable<Film> GetFilms()
        {
            var getCommandString = $@"SELECT * FROM films;";
            var reader = new SQLiteCommand(getCommandString, _readConnection).ExecuteReader();

            while (reader.Read())
            {
                Film? film;
                try
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var year = reader.GetInt32(2);
                    var description = reader.GetString(3);
                    var countries = System.Text.Json.JsonSerializer.Deserialize<List<string>>(
                        (string)reader["countries"]
                    );
                    var duration = reader.GetString(5);
                    var genres = System.Text.Json.JsonSerializer.Deserialize<List<string>>(
                        (string)reader["genres"]
                    );
                    var poster = reader.GetString(7);
                    var language = (Language)reader.GetInt32(8);
                    var players = System.Text.Json.JsonSerializer.Deserialize<List<string>>(
                        (string)reader["players"]
                    );

                    film = new Film(
                        name: name,
                        year: year,
                        description: description,
                        countries: countries ?? new List<string>(),
                        duration: duration,
                        genres: genres ?? new List<string>(),
                        poster: poster,
                        players: players ?? new List<string>(),
                        id: id,
                        language: language
                    );
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    film = null;
                }

                if (film != null)
                    yield return film;
            }
        }

        public void AddEnglishFilm(Film? film)
        {
            using (var connection = new SQLiteConnection($"URI=file:{_databaseName}_EN.db"))
            {
                connection.Open();
                try
                {
                    var commandString =
                        "CREATE TABLE films ("
                        + "id INTEGER PRIMARY KEY,"
                        + "name TEXT  UNIQUE NOT NULL,"
                        + "year INTEGER,"
                        + "description TEXT NOT NULL,"
                        + "countries  JSON,"
                        + "duration  TEXT NOT NULL,"
                        + "genres JSON,"
                        + "poster  TEXT NOT NULL,"
                        + "language INTEGER,"
                        + "players JSON NOT NULL)";
                    var command = new SQLiteCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Warning(e.ToString());
                }

                if (film == null)
                {
                    return;
                }
                try
                {
                    film.Description = film.Description.Replace("'", "");
                    var commandString =
                        $"INSERT INTO films "
                        + $"(name,year,description,countries,duration,genres,poster,language,players)"
                        + $@"VALUES ('{film.Name}',{film.Year},'{film.Description}','{JsonConvert.SerializeObject(film.Countries)}','{film.Duration}',"
                        + $@"'{JsonConvert.SerializeObject(film.Genres)}','{film.Poster}',{(int)film.Language},'{JsonConvert.SerializeObject(film.Players)}');";
                    var command = new SQLiteCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e.Message.ToLower().Contains("unique"))
                    {
                        var getCommandString =
                            $@"SELECT id, players FROM films WHERE name='{film.Name}';";
                        var reader = new SQLiteCommand(
                            getCommandString,
                            connection
                        ).ExecuteReader();

                        while (reader.Read())
                        {
                            var existFilmId = reader.GetInt32(0);
                            var existFilmPlayers = System.Text.Json.JsonSerializer.Deserialize<
                                List<string>
                            >((string)reader["players"]);
                            foreach (var p in existFilmPlayers!)
                            {
                                if (film.Players.Contains(p) == false)
                                {
                                    film.Players.Add(p);
                                }
                            }
                            var newFilmPlayers = JsonConvert.SerializeObject(film.Players);

                            var updateCommand = new SQLiteCommand(
                                @$"UPDATE films SET players='{newFilmPlayers}' WHERE id={existFilmId};",
                                connection
                            );
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Logger.Warning(e.ToString());
                    }
                }
            }
        }

        public void AddUkrainianFilm(Film? film)
        {
            using (var connection = new SQLiteConnection($"URI=file:{_databaseName}_UA.db"))
            {
                connection.Open();
                try
                {
                    var commandString =
                        "CREATE TABLE films ("
                        + "id INTEGER PRIMARY KEY,"
                        + "name TEXT  UNIQUE NOT NULL,"
                        + "year INTEGER,"
                        + "description TEXT NOT NULL,"
                        + "countries  JSON,"
                        + "duration  TEXT NOT NULL,"
                        + "genres JSON,"
                        + "poster  TEXT NOT NULL,"
                        + "language INTEGER,"
                        + "players JSON NOT NULL)";
                    var command = new SQLiteCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Warning(e.ToString());
                }

                if (film == null)
                {
                    return;
                }
                try
                {
                    film.Description = film.Description.Replace("'", "");
                    var commandString =
                        $"INSERT INTO films "
                        + $"(name,year,description,countries,duration,genres,poster,language,players)"
                        + $@"VALUES ('{film.Name}',{film.Year},'{film.Description}','{JsonConvert.SerializeObject(film.Countries)}','{film.Duration}',"
                        + $@"'{JsonConvert.SerializeObject(film.Genres)}','{film.Poster}',{(int)film.Language},'{JsonConvert.SerializeObject(film.Players)}');";
                    var command = new SQLiteCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (e.Message.ToLower().Contains("unique"))
                    {
                        var getCommandString =
                            $@"SELECT id, players FROM films WHERE name='{film.Name}';";
                        var reader = new SQLiteCommand(
                            getCommandString,
                            connection
                        ).ExecuteReader();

                        while (reader.Read())
                        {
                            var existFilmId = reader.GetInt32(0);
                            var existFilmPlayers = System.Text.Json.JsonSerializer.Deserialize<
                                List<string>
                            >((string)reader["players"]);
                            foreach (var p in existFilmPlayers!)
                            {
                                if (film.Players.Contains(p) == false)
                                {
                                    film.Players.Append(p);
                                }
                            }
                            var newFilmPlayers = JsonConvert.SerializeObject(film.Players);

                            var updateCommand = new SQLiteCommand(
                                @$"UPDATE films SET players='{newFilmPlayers}' WHERE id={existFilmId};",
                                connection
                            );
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Logger.Warning(e.ToString());
                    }
                }
            }
        }
    }
}
