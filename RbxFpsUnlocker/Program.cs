namespace RbxFpsUnlocker;

internal class Program
{
    private const string RobloxVersionsDirName = "Roblox/Versions";
    private const string ClientSettingsDirName = "ClientSettings";
    private const string RobloxPlayerBetaExe = "RobloxPlayerBeta.exe";

    private static void Main()
    {
        var versionsDir = GetRobloxVersionsDirectory();
        var clientSettingsPath = GetClientSettingsPath(versionsDir);

        Console.Title = "FPS Unlocker";

        if (string.IsNullOrEmpty(clientSettingsPath))
        {
            Console.WriteLine("Failed to locate Roblox path.");
            Console.ReadKey(true);
            return;
        }

        EnsureDirectoryExists(clientSettingsPath);

        var settingsFile = Path.Combine(clientSettingsPath, "ClientAppSettings.json");
        EnsureFileExists(settingsFile);

        var framerate = GetFramerateInput();

        WriteFramerateSettings(settingsFile, framerate);

        Console.WriteLine("Operation succeeded! Press any key to close the program.");
        Console.ReadKey(true);
    }

    private static string GetRobloxVersionsDirectory()
    {
        var programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var robloxVersionsDir = Path.Combine(programFilesDir, RobloxVersionsDirName);

        return Directory.Exists(robloxVersionsDir)
            ? robloxVersionsDir
            : Path.Combine(localAppDataDir, RobloxVersionsDirName);
    }

    private static string GetClientSettingsPath(string versionsDir)
    {
        var robloxPlayerBetaPaths = Directory.GetFiles(versionsDir, RobloxPlayerBetaExe, SearchOption.AllDirectories);
        return robloxPlayerBetaPaths.Length > 0
            ? Path.Combine(Path.GetDirectoryName(robloxPlayerBetaPaths[0]) ?? string.Empty, ClientSettingsDirName)
            : string.Empty;
    }

    private static void EnsureDirectoryExists(string directory)
    {
        Directory.CreateDirectory(directory);
    }

    private static void EnsureFileExists(string file)
    {
        if (!File.Exists(file)) File.WriteAllBytes(file, Array.Empty<byte>());
    }

    private static int GetFramerateInput()
    {
        while (true)
        {
            Console.WriteLine("Enter the desired framerate limit:");
            if (int.TryParse(Console.ReadLine(), out var framerate) && framerate > 0) return framerate;

            Console.WriteLine("Invalid input. Please enter a positive integer value.");
        }
    }

    private static void WriteFramerateSettings(string settingsFile, int framerate)
    {
        var jsonContent = $"{{\"DFIntTaskSchedulerTargetFps\": {framerate}}}";
        using var writer = new StreamWriter(settingsFile);
        writer.Write(jsonContent);
    }
}