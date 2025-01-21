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
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            return process;
        }

        public string RunAdbCommand(string command)
        {
            // Console.WriteLine($"Running Command: {command}");
            string adbCommand = adbCommands[command];
            if (command.Contains("Toggle"))
            {
                // Console.WriteLine("Toggle Command Detected");
                string newState = RunToggleCommand(adbCommand);
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
            process.WaitForExit();
            // Console.WriteLine($"Command Executed: {adbCommand}");

            return output;
        }

        private string RunToggleCommand(string adbCommand)
        {
            // Read the current state and update
            string currentState = RunDefaultCommand(adbCommand).Trim();
            // Console.WriteLine($"Current State: {currentState}");
            string newState = currentState == "enabled" ? "disable" : "enable";
            string toggleCommand = adbCommand + " " + newState;
            RunDefaultCommand(toggleCommand);
            return newState;
        }

    }
}