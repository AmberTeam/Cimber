using Cimber.Bot;
using Cimber.Bot.Models;
using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

Directory.CreateDirectory("static");
Env.Load();
string token = Env.GetString("TOKEN");
State state = State.Default;
List<long> AdminList = new List<long>()
{
    930727649,
    1495480119
};
var database = new Database();

var bot = new TelegramBotClient(token);
bot.StartReceiving(Update, Error);

async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
{
    var message = update.Message;

    if (message != null)
    {
        #region User

        if (AdminList.Contains(message.Chat.Id) == false)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    switch (message.Text)
                    {
                        case "/start":
                            await bot.SendPhotoAsync(message.Chat.Id, "https://media2.giphy.com/media/kR7nX5EfyoxPy/giphy.gif?cid=790b76114d73d2c084c4ffdb46f9f0f174f249263ff8993b&rid=giphy.gif&ct=g", caption: "Hi, welcome to Cimber Bot🍿! There you can easily submit bugs to us. Choose an action below:", replyMarkup: Markups.MainMenuEngUser);

                            break;
                        case "Send a bug":
                            await bot.SendPhotoAsync(message.Chat.Id, "https://www.reactiongifs.us/wp-content/uploads/2018/05/giphy-1-2.gif", caption: "Please send a bug description with a photo(or not) here:", replyMarkup: Markups.MainMenuEngUser);
                            state = State.SendABug;

                            break;
                        default:
                            if (state == State.SendABug)
                            {
                                Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Only Text |   {message!.Text}");
                                state = State.Default;

                                var bug = new Bug() { Description = message!.Text, Type = Cimber.Bot.Models.Type.Text, FromUser = message.Chat.Id, Path = null };
                                database.AddBug(bug);

                                await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngUser);
                            }
                            break;
                    }

                    break;
                case MessageType.Photo:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Photo + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Photo!.Last().FileId;
                        var fileInfo = await client.GetFileAsync(fileId);

                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION" ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Photo, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngUser);
                    }
                    break;
                case MessageType.Video:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Video + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Video!.FileId;
                        var fileInfo = await client.GetFileAsync(fileId);
                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Video, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngUser);
                    }
                    break;
                case MessageType.Document:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Document + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Document!.FileId;
                        var fileInfo = await client.GetFileAsync(fileId);
                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Document, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngUser);
                    }
                    break;
                default: break;
            }
        }

        #endregion

        #region Admin

        if (AdminList.Contains(message.Chat.Id))
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    switch (message.Text)
                    {
                        case "/start":
                            await bot.SendPhotoAsync(message.Chat.Id, "https://media2.giphy.com/media/kR7nX5EfyoxPy/giphy.gif?cid=790b76114d73d2c084c4ffdb46f9f0f174f249263ff8993b&rid=giphy.gif&ct=g", caption: "Hi, welcome to Cimber Bot🍿! There you can easily submit bugs to us. Choose an action below:", replyMarkup: Markups.MainMenuEngAdmin);

                            break;
                        case "Send a bug":
                            await bot.SendPhotoAsync(message.Chat.Id, "https://www.reactiongifs.us/wp-content/uploads/2018/05/giphy-1-2.gif", caption: "Please send a bug description with a photo(or not) here:", replyMarkup: Markups.MainMenuEngAdmin);
                            state = State.SendABug;

                            break;
                        case "Bugs list":
                            var bugs = database.GetBugs().ToList();

                            string text = "<b>Bugs</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━<b>ID</b>    <b>TYPE</b>    <b>DESCRIPTION</b>    <b>PATH</b>    <b>FROM USER</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";

                            foreach (var bug in bugs)
                            {
                                text += $"\n<b>{bug.Id}</b>    {bug.Type}    {bug.Description}    {bug.Path}    {bug.FromUser}\n\n";
                            }

                            await bot.SendTextMessageAsync(message.Chat.Id, text, parseMode: ParseMode.Html);

                            break;
                        case "Mark a bug as fixed":
                            await bot.SendTextMessageAsync(message.Chat.Id, "Please send the id of bug u want to mark as fixed");
                            state = State.MarkAsFixed;

                            break;
                        default:
                            if (state == State.SendABug)
                            {
                                Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Only Text |   {message!.Text}");
                                state = State.Default;

                                var bug = new Bug() { Description = message!.Text, Type = Cimber.Bot.Models.Type.Text, FromUser = message.Chat.Id, Path = null };
                                database.AddBug(bug);

                                await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngAdmin);
                            }
                            else if (state == State.MarkAsFixed)
                            {
                                var bug = database.DeleteBug(int.Parse(message!.Text!));
                                string caption = $"<b>The bug you recently has been successfully fixed :D</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━<b>ID</b>    <b>TYPE</b>    <b>DESCRIPTION</b>    <b>PATH</b>   <b>FROM USER</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n{bug.Id}    {bug.Type}    {bug.Description}    {bug.Path}    {bug.FromUser}";

                                await bot.SendPhotoAsync(message.Chat.Id, "https://gfycat.com/discover/check-mark-gifs", caption: caption, replyMarkup: Markups.MainMenuEngUser, parseMode: ParseMode.Html);
                                await bot.SendPhotoAsync(bug!.FromUser!, "https://gfycat.com/discover/check-mark-gifs", caption: caption, replyMarkup: Markups.MainMenuEngUser, parseMode: ParseMode.Html);

                                state = State.Default;
                            }
                            break;
                    }

                    break;
                case MessageType.Photo:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Photo + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Photo!.Last().FileId;
                        var fileInfo = await client.GetFileAsync(fileId);

                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Photo, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngAdmin);
                    }
                    break;
                case MessageType.Video:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Video + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Video!.FileId;
                        var fileInfo = await client.GetFileAsync(fileId);
                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Video, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngAdmin);
                    }
                    break;
                case MessageType.Document:
                    if (state == State.SendABug)
                    {
                        Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Document + Text |   {message!.Caption}");
                        state = State.Default;

                        var fileId = message!.Document!.FileId;
                        var fileInfo = await client.GetFileAsync(fileId);
                        var bug = new Bug() { Description = message!.Caption ?? "NO DESCRIPTION", Type = Cimber.Bot.Models.Type.Document, FromUser = message.Chat.Id, Path = $"https://api.telegram.org/file/bot{Env.GetString("TOKEN")}/{fileInfo.FilePath}" };
                        database.AddBug(bug);

                        await bot.SendPhotoAsync(message.Chat.Id, "https://www.sellerlift.com/wp-content/uploads/2017/04/gif-icons-menu-transition-animations-send-mail-400x300.gif", caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngAdmin);
                    }
                    break;
                default: break;
            }
        }

        #endregion
    }
}

async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    Console.WriteLine(exception.ToString());
}

Console.ReadLine();