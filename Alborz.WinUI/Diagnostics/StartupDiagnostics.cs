using System;
using System.IO;

namespace Alborz.WinUI.Diagnostics;

internal static class StartupDiagnostics
{
    public static void Record(Exception exception)
    {
        try
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlborzApp");
            Directory.CreateDirectory(directory);
            File.AppendAllText(Path.Combine(directory, "startup-errors.log"),
                $"{DateTimeOffset.Now:O} {exception.GetType().FullName} HRESULT=0x{exception.HResult:X8}\n{exception.Message}\n{exception.StackTrace}\n{exception.InnerException}\n");
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
