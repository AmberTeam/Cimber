using log4net;
using log4net.Config;

namespace Cimber.Translator
{
    internal class Logger
    {
        private static ILog log = LogManager.GetLogger("LOGGER");

        public static ILog Log
        {
            get { return log; }
        }

        internal static void InitLogger()
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        internal static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log.Info(text);
        }

        internal static void Warning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.Warn(text);
        }

        internal static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log.Error(text);
        }
    }
}
