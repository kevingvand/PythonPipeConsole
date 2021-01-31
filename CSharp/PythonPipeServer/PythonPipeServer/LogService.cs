using System;

namespace PythonPipeServer
{
    public static class LogService
    {
        public static void LogInfo(string message) => Log(message, ConsoleColor.White);
        public static void LogError(string message) => Log(message, ConsoleColor.Red);
        public static void LogWarning(string message) => Log(message, ConsoleColor.Yellow);

        private static void Log(string message, ConsoleColor color)
        {
            var prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]";
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"{prefix} {message}");
            Console.ForegroundColor = previousColor;
        }
    }
}
