using Telegram.Bot.Types.ReplyMarkups;

namespace Cimber.Bot
{
    public static class Markups
    {
        public readonly static ReplyKeyboardMarkup MainMenuEngUser = new(new[] { new KeyboardButton[] { "Send a bug" } }) { ResizeKeyboard = true };
        public readonly static ReplyKeyboardMarkup MainMenuEngAdmin = new(new[] { new KeyboardButton[] { "Send a bug", "Bugs list", "Mark a bug as fixed" } }) { ResizeKeyboard = true };
    }
}
