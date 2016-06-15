using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SteamVR_HUDCenter
{
    public class UDebug
    {
        private enum LogType
        {
            Error = ConsoleColor.Red,
            Warning = ConsoleColor.Yellow,
            Info = ConsoleColor.Gray
        }

        public static void LogError(object Error)
        {
            string CallerName = Assembly.GetCallingAssembly().GetName().Name;
            if (Error is Exception)
                Error = ((Exception)Error).Message;
            PlotLog(LogType.Error, CallerName, Error.ToString());
        }

        public static void Log(object Error)
        {
            string CallerName = Assembly.GetCallingAssembly().GetName().Name;
            if (Error is Exception)
                Error = ((Exception)Error).Message;
            PlotLog(LogType.Info, CallerName, Error.ToString());
        }

        public static void LogWarning(object Error)
        {
            string CallerName = Assembly.GetCallingAssembly().GetName().Name;
            if (Error is Exception)
                Error = ((Exception)Error).Message;
            PlotLog(LogType.Warning, CallerName, Error.ToString());
        }

        private static void PlotLog(LogType type, string Name, string Message)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)type;
            Console.WriteLine("[{0}] {1}", Name, Message);
            Console.ForegroundColor = old;
        }
    }
}
