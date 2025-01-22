using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ADB.Utilities
{
    public class AdbCommandRunner
    {
        private Dictionary<string, string> adbCommands;
        
        public AdbCommandRunner(IConfiguration configuration)
        {
            adbCommands = configuration.GetSection("AdbCommands").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
        }

        private Process CreateProcess(string adbCommand)
        {   
            Process process = new Process();
            process.StartInfo.FileName = "bash";
            process.StartInfo.Arguments = $"-c \"{adbCommand}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            return process;
        }

        public string RunAdbCommand(string command)
        {
            // Console.WriteLine($"Running Command: {command}");

            if (!adbCommands.TryGetValue(command, out string adbCommand))
            {
                throw new ArgumentException($"Command '{command}' not found in configuration.");
            }

            if (command.Contains("Toggle"))
            {
                // Console.WriteLine("Toggle Command Detected");
                string newState = RunToggleCommand(adbCommand);
                if (newState == "no devices/emulators found")
                {
                    return newState;
                }
                string outputString = $"{command.Replace("Toggle", "")} is now {newState}d";
                return outputString;
            }
            else
            {
                // Console.WriteLine("Default Command Detected");
                return RunDefaultCommand(adbCommand);
            }
            
        }

        private string RunDefaultCommand(string adbCommand)
        {
            // Create a process with command injected
            Process process = CreateProcess(adbCommand);
            process.Start();

            // Read the output
            string output = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // Check if no devices are found
            if (string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(errorOutput))
            {
                return errorOutput;
            }

            return output;
        }

        private string RunToggleCommand(string adbCommand)
        {
            // Read the current state and update
            string currentState = RunDefaultCommand(adbCommand).Trim();
            if (currentState.Contains("no devices/emulators found"))
            {
                return "no devices/emulators found";
            }
            // Console.WriteLine($"Current State: {currentState}");
            string newState = currentState == "enabled" ? "disable" : "enable";
            string toggleCommand = adbCommand + " " + newState;
            RunDefaultCommand(toggleCommand);
            return newState;
        }

    }
}