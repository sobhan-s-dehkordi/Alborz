namespace Alborz.Application.Features.Settings;

public enum AppTheme { System, Light, Dark }

public sealed record AppearanceSettings(AppTheme Theme = AppTheme.System)
{
    public AppearanceSettings Normalize() => new(Enum.IsDefined(Theme) ? Theme : AppTheme.System);
}
