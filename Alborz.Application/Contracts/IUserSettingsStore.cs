using Alborz.Application.Features.Settings;

namespace Alborz.Application.Contracts;

public interface IUserSettingsStore
{
    AppearanceSettings LoadAppearance();
    Task SaveAppearanceAsync(AppearanceSettings settings, CancellationToken cancellationToken = default);
}
