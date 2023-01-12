using Cimber.Bot;
using Cimber.Bot.Models;
using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

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
string BUG_PATH = Directory.GetCurrentDirectory() + (Environment.OSVersion.Platform == PlatformID.Win32NT ? @"\Materials\bug.jpg" : @"/Materials/bug.jpg");
string SCARED_PATH = Directory.GetCurrentDirectory() + (Environment.OSVersion.Platform == PlatformID.Win32NT ? @"\Materials\scared.webp" : @"/Materials/scared.webp");
string START_PATH = Directory.GetCurrentDirectory() + (Environment.OSVersion.Platform == PlatformID.Win32NT ? @"\Materials\start.jpg" : @"/Materials/start.jpg");
string WAIT_PATH = Directory.GetCurrentDirectory() + (Environment.OSVersion.Platform == PlatformID.Win32NT ? @"\Materials\wait.webp" : @"/Materials/wait.webp");

var bot = new TelegramBotClient(token);
bot.StartReceiving(Update, Error);

async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
{
    try
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
                            case " / start":
                                await bot.SendPhotoAsync(message.Chat.Id, "https://media2.giphy.com/media/kR7nX5EfyoxPy/giphy.gif?cid=790b76114d73d2c084c4ffdb46f9f0f174f249263ff8993b&rid=giphy.gif&ct=g", caption: "Hi, welcome to Cimber Bot🍿! There you can easily submit bugs to us. Choose an action below:", replyMarkup: Markups.MainMenuEngUser);

                                break;
                            case "Send a bug":
                                await SendABugMessage(message);
                                state = State.SendABug;

                                break;
                            default:
                                if (state == State.SendABug)
                                {
                                    Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Only Text |   {message!.Text}");
                                    state = State.Default;

                                    var bug = new Bug() { Description = message!.Text, Type = Cimber.Bot.Models.Type.Text, FromUser = message.Chat.Id, Path = null };
                                    database.AddBug(bug);

                                    await SendThanks(message);
                                    await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Text}");
                                }
                                break;
                        }

                        break;
                    case MessageType.Photo:
                        if (state == State.SendABug)
                        {
                            var fileId = message!.Photo!.Last().FileId;
                            var fileInfo = await client.GetFileAsync(fileId);

                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Photo);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message);
                            await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Caption}");
                        }
                        state = State.Default;
                        break;
                    case MessageType.Video:
                        if (state == State.SendABug)
                        {
                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Video);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message);
                            await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Caption}");
                        }

                        state = State.Default;
                        break;
                    case MessageType.Document:
                        if (state == State.SendABug)
                        {
                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Document);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message);
                            await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Caption}");
                        }
                        state = State.Default;

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
                                await StartMessage(message);

                                break;
                            case "Send a bug":
                                await SendABugMessage(message); state = State.SendABug;

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
                                    var bug = new Bug() { Description = message!.Text, Type = Cimber.Bot.Models.Type.Text, FromUser = message.Chat.Id, Path = null };
                                    database.AddBug(bug);

                                    await SendThanks(message, isAdmin: true);
                                    await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Text}");
                                }
                                else if (state == State.MarkAsFixed)
                                {
                                    var bug = database.DeleteBug(int.Parse(message!.Text!));
                                    string caption = $"<b>The bug you recently has been successfully fixed :D</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━<b>ID</b>    <b>TYPE</b>    <b>DESCRIPTION</b>   <b>FROM USER</b>\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n{bug.Id}    {bug.Type}    {bug.Description}    {bug.FromUser}";

                                    await bot.SendPhotoAsync(message.Chat.Id, "https://gfycat.com/discover/check-mark-gifs", caption: caption, replyMarkup: Markups.MainMenuEngUser, parseMode: ParseMode.Html);
                                    await bot.SendPhotoAsync(bug!.FromUser!, "https://gfycat.com/discover/check-mark-gifs", caption: caption, replyMarkup: Markups.MainMenuEngUser, parseMode: ParseMode.Html);
                                }

                                state = State.Default;
                                break;
                        }

                        break;
                    case MessageType.Photo:
                        if (state == State.SendABug)
                        {
                            Console.WriteLine($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) Photo + Text |   {message!.Caption}");

                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Photo);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message, isAdmin: true);
                            await SendToAdmins($"<b>New bug!</b>\n\n {message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {bug.Description}");
                        }
                        state = State.Default;
                        break;
                    case MessageType.Video:
                        if (state == State.SendABug)
                        {
                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Video);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message, isAdmin: true);
                            await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Caption}");
                        }
                        state = State.Default;
                        break;
                    case MessageType.Document:
                        if (state == State.SendABug)
                        {
                            var bug = await client.GetFilePath(message, Cimber.Bot.Models.Type.Document);
                            if (bug == null) return;
                            database.AddBug(bug);

                            await SendThanks(message, isAdmin: true);
                            await SendToAdmins($"{message!.From!.FirstName} {message!.From!.LastName} ({message!.From!.Username}) {bug.Type} |   {message!.Caption}");
                        }

                        state = State.Default;
                        break;
                    default: break;
                }
            }

            #endregion
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

async Task SendABugMessage(Message message)
{
    await using Stream stream = System.IO.File.OpenRead(BUG_PATH);

    await bot.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(content: stream), caption: "Please send a bug description with a photo or file(in one message) here:", replyMarkup: Markups.MainMenuEngAdmin);

}

async Task StartMessage(Message message)
{
    await using Stream stream = System.IO.File.OpenRead(START_PATH);

    await bot.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(content: stream), caption: "Hi, welcome to Cimber Bot🍿! There you can easily submit bugs to us. Choose an action below:", replyMarkup: Markups.MainMenuEngAdmin);
}

async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    Console.WriteLine(exception.ToString());
}

async Task SendToAdmins(string text)
{
    await using Stream stream = System.IO.File.OpenRead(SCARED_PATH);

    foreach (var admin in AdminList)
    {
        await bot!.SendPhotoAsync(admin, new InputOnlineFile(content: stream), caption: text, parseMode: ParseMode.Html);
    }
}

async Task SendThanks(Message message, bool isAdmin = false)
{
    await using Stream stream = System.IO.File.OpenRead(WAIT_PATH);

    if (isAdmin)
    {
        await bot!.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(content: stream), caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngAdmin);
    }
    else
    {
        await bot!.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(content: stream), caption: "Thanks :D Your message will be seen by our developers soon!", replyMarkup: Markups.MainMenuEngUser);
    }
}

Console.ReadLine();