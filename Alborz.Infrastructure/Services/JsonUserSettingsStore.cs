using Alborz.Application.Contracts;
using Alborz.Application.Features.Settings;
using System.Text.Json;

namespace Alborz.Infrastructure.Services;

public sealed class JsonUserSettingsStore : IUserSettingsStore
{
    private readonly string _path;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public JsonUserSettingsStore() : this(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlborzApp", "appearance.json")) { }

    public JsonUserSettingsStore(string path) => _path = Path.GetFullPath(path);

    public AppearanceSettings LoadAppearance()
    {
        try
        {
            return File.Exists(_path)
                ? (JsonSerializer.Deserialize<AppearanceSettings>(File.ReadAllText(_path)) ?? new()).Normalize()
                : new();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return new();
        }
    }

    public async Task SaveAppearanceAsync(AppearanceSettings settings, CancellationToken cancellationToken = default)
    {
        await _writeLock.WaitAsync(cancellationToken);
        var temporaryPath = _path + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            await File.WriteAllTextAsync(temporaryPath,
                JsonSerializer.Serialize(settings.Normalize(), new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
            File.Move(temporaryPath, _path, overwrite: true);
        }
        finally
        {
            try { if (File.Exists(temporaryPath)) File.Delete(temporaryPath); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            _writeLock.Release();
        }
    }
}
