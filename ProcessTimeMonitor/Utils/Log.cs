using ProcessTimeMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager.Utils
{
    public static class Log
    {
        static void WriteFgColorWithTime(ConsoleColor fgColor, string logLevel)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("{0} ", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff"));
            Console.ForegroundColor = fgColor;
            Console.Write($"[{logLevel}]");
            Console.ResetColor();
            Console.Write($" ");
        }
        static void WriteFgColor(ConsoleColor fgColor, string msg)
        {
            Console.ForegroundColor = fgColor;
            Console.Write(msg);
            Console.ResetColor();
        }
        static void WriteLineFgColor(ConsoleColor fgColor, string msg)
        {
            Console.ForegroundColor = fgColor;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        public static void Debug(string source, string msg)
        {
            if (Global.LogLevel >= LogLevel.DEBUG)
            {
                WriteFgColorWithTime(ConsoleColor.DarkGray, "DEBUG");
                WriteFgColor(ConsoleColor.DarkCyan, $"{source}: ");
                WriteLineFgColor(ConsoleColor.DarkGray, $"{msg}");
            }
        }
        public static void Info(string source, string msg)
        {
            if (Global.LogLevel >= LogLevel.INFO)
            {
                WriteFgColorWithTime(ConsoleColor.Green, "INFO");
                WriteFgColor(ConsoleColor.DarkCyan, $"{source}: ");
                WriteLineFgColor(ConsoleColor.DarkGray, $"{msg}");
            }
        }
        public static void Warn(string source, string msg)
        {
            if (Global.LogLevel >= LogLevel.WARN)
            {
                WriteFgColorWithTime(ConsoleColor.Yellow, "WARN");
                WriteFgColor(ConsoleColor.DarkCyan, $"{source}: ");
                WriteLineFgColor(ConsoleColor.DarkGray, $"{msg}");
            }
        }
        public static void Error(string source, string msg)
        {
            if (Global.LogLevel >= LogLevel.ERROR)
            {
                WriteFgColorWithTime(ConsoleColor.Red, "ERROR");
                WriteFgColor(ConsoleColor.DarkCyan, $"{source}: ");
                WriteLineFgColor(ConsoleColor.DarkGray, $"{msg}");
            }
        }
    }
}
