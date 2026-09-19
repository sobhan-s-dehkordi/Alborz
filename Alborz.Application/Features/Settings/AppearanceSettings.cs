namespace Alborz.Application.Features.Settings;

public enum AppTheme { System, Light, Dark }

public sealed record AppearanceSettings(AppTheme Theme = AppTheme.System, string FontFamily = "Segoe UI", double FontSize = 14)
{
    public AppearanceSettings Normalize() => new(
        Enum.IsDefined(Theme) ? Theme : AppTheme.System,
        string.IsNullOrWhiteSpace(FontFamily) || FontFamily.Length > 100 ? "Segoe UI" : FontFamily.Trim(),
        double.IsFinite(FontSize) && FontSize >= 12 && FontSize <= 24 ? FontSize : 14);
}
