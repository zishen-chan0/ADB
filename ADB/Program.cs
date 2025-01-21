using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using ADB.Utilities;

class Program
{
    static void Main(string[] args)
    {
        // Build configuration and Retrieve ADB commands
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var adbCommands = configuration.GetSection("AdbCommands").Get<Dictionary<string, string>>();

        // Create an instance of AdbCommandRunner
        AdbCommandRunner runner = new AdbCommandRunner(configuration);

        // Main loop
        Console.WriteLine("Press 'a' to toggle airplane mode, 'b' to list out connected devices, 'esc' to exit.");
        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.A)
            {
                string output = runner.RunAdbCommand("ToggleAirplaneMode");
                Console.WriteLine(output);
            }
            else if (key == ConsoleKey.B)
            {
                string output = runner.RunAdbCommand("ListDevices");
                Console.WriteLine(output);
            }
            else if (key == ConsoleKey.Escape)
            {
                Console.WriteLine("Exiting program.");
                break;
            }
        }
    }
}