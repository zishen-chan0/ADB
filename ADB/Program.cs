using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADB.Utilities;

class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = loggerFactory.CreateLogger("Program");

        var configuration;
        var adbCommands;

        try{
            // Build configuration and Retrieve ADB commands
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            adbCommands = configuration.GetSection("AdbCommands").Get<Dictionary<string, string>>();

            // Create an instance of AdbCommandRunner
            AdbCommandRunner runner = new AdbCommandRunner(configuration);

            // Main loop
            logger.LogInformation("Press 'a' to toggle airplane mode, 'b' to list out connected devices, 'esc' to exit.");
            while (true)
            {
                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.A)
                {
                    string output = runner.RunAdbCommand("ToggleAirplaneMode");
                    logger.LogInformation(output);
                }
                else if (key == ConsoleKey.B)
                {
                    string output = runner.RunAdbCommand("ListDevices");
                    logger.LogInformation(output);
                }
                else if (key == ConsoleKey.Escape)
                {
                    logger.LogInformation("Exiting Program...");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred");
        }
        finally
        {
            logger.LogInformation("Exiting Program...");
        }
    }
}