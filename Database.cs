using Cimber.Scraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Text.Json;

namespace Cimber.Scraper
{
    internal class Database
    {
        private readonly SQLiteConnection _connection;

        public Database()
        {
            string connectionString = "URI=file:CimberDB.db";

            _connection = new SQLiteConnection(connectionString);
            _connection.Open();

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
                var command = new SQLiteCommand(commandString, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Warning(e.ToString());
            }
        }

        ~Database()
        {
            _connection.Close();
        }

        public void AddFilm(Film? film)
        {
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
                var command = new SQLiteCommand(commandString, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("unique"))
                {
                    var getCommandString =
                        $@"SELECT id, players FROM films WHERE name='{film.Name}';";
                    var reader = new SQLiteCommand(getCommandString, _connection).ExecuteReader();

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
                            _connection
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
