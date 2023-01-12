using Cimber.Bot.Models;
using System.Data.SQLite;

namespace Cimber.Bot
{
    public class Database
    {
        private readonly SQLiteConnection _connection;

        public Database()
        {
            string connectionString = "URI=file:CimberBugs.db";

            _connection = new SQLiteConnection(connectionString);
            _connection.Open();

            try
            {
                var commandString =
                    "CREATE TABLE Bug ("
                    + "Id INTEGER PRIMARY KEY,"
                    + "Description TEXT    NOT NULL,"
                    + "Type INTEGER,"
                    + "FromUser INTEGER,"
                    + "Path TEXT)";
                var command = new SQLiteCommand(commandString, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        ~Database()
        {
            _connection.Close();
        }

        /// <summary>
        /// Adds bug to db
        /// </summary>
        /// <param name="bug"></param>
        /// <returns>Has bug added to db</returns>
        public bool AddBug(Bug bug)
        {
            try
            {
                string commandString;

                if (bug.Path == null)
                {
                    commandString = $"INSERT INTO Bug (Description, Type, FromUser) VALUES ('{bug.Description}', {(int)bug.Type}, {bug.FromUser});";
                }
                else
                {
                    commandString = $"INSERT INTO Bug (Description, Type, FromUser, Path) VALUES ('{bug.Description}', {(int)bug.Type}, {bug.FromUser}, '{bug.Path}');";
                }
                var command = new SQLiteCommand(commandString, _connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n{e.ToString()}\n");
                return false;
            }
        }

        public IEnumerable<Bug> GetBugs()
        {
            var getCommandString = $@"SELECT * FROM Bug;";
            var reader = new SQLiteCommand(getCommandString, _connection).ExecuteReader();

            while (reader.Read())
            {
                Bug? bug;

                try
                {
                    int id = reader.GetInt32(0);
                    string description = reader.GetString(1);
                    int? type = reader.GetInt32(2);
                    long? fromUser = reader.GetInt32(3);
                    
                    string? path;

                    try
                    {
                        path = reader.GetString(4);
                    }
                    catch
                    {
                        path = "NO PATH";
                    }
                    bug = new Bug() { Id = id, FromUser = fromUser, Description = description, Type = (Cimber.Bot.Models.Type)type, Path = path }; 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n{ex.ToString()}\n");
                    bug = null;
                }

                if (bug != null)
                {
                    yield return bug;
                }
            }
        }
        
        public Bug? DeleteBug(int id)
        {
            try
            {
                // Getting bug 
                var getCommandString = $@"SELECT * FROM Bug WHERE Id = {id};";
                var reader = new SQLiteCommand(getCommandString, _connection).ExecuteReader();


                Bug? bug = null;

                while (reader.Read())
                {
                    try
                    {
                        string description = reader.GetString(1);
                        int? type = reader.GetInt32(2);
                        long? fromUser = reader.GetInt32(3);
                        var path = reader.GetValue(4);

                        bug = new Bug() { Id = id, FromUser = fromUser, Description = description, Type = (Models.Type)type, Path = path.ToString() };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n{ex.ToString()}\n");
                        bug = null;
                    }
                }

                // Deleting bug
                var commandString = $"DELETE FROM Bug WHERE Id = {id}";
                var command = new SQLiteCommand(commandString, _connection);

                command.ExecuteNonQuery();

                return bug;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n{e.ToString()}\n");
                return null;
            }
        }
    }
}
