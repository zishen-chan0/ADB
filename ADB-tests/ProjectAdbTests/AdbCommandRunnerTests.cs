using System;
using Xunit;
using ADB.Utilities;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjectAdbTests
{
    public class AdbCommandRunnerTests
    {
        private IConfiguration configuration;

        public AdbCommandRunnerTests()
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "ADB"));
            configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [Fact]
        public void TestRunAdbCommand()
        {
            // Arrange
            var runner = new AdbCommandRunner(configuration);
            string command = "ListDevices";

            // Act
            string output = runner.RunAdbCommand(command);

            // Assert
            Assert.NotNull(output);
            Assert.Contains("List of devices attached", output);
        }

        [Fact]
        public void TestToggleAirplaneMode()
        {
            // Arrange
            var runner = new AdbCommandRunner(configuration);
            string command = "GetAirplaneModeStatus";
            string command2 = "ToggleAirplaneMode";

            // Act
            string outputBefore = runner.RunAdbCommand(command);
            runner.RunAdbCommand(command2);
            string outputAfter = runner.RunAdbCommand(command);

            // Assert
            Assert.NotNull(outputBefore);
            Assert.NotNull(outputAfter);
            Assert.NotEqual(outputBefore, outputAfter);
        }
    }
}