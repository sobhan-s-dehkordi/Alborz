using Alborz.Application.Contracts;
using Alborz.Application.Features.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Threading.Tasks;

namespace Alborz.WinUI.Services;

public sealed class AppearanceService
{
    private readonly IUserSettingsStore _store;
    private AppearanceSettings _current;
    public AppearanceSettings Current => _current;
    public AppearanceState State { get; } = new();

    public AppearanceService(IUserSettingsStore store)
    {
        _store = store;
        _current = store.LoadAppearance();
        UpdateState();
    }

    public async Task UpdateAsync(AppearanceSettings settings)
    {
        _current = settings.Normalize();
        UpdateState();
        await _store.SaveAppearanceAsync(_current);
    }

    public void ApplyTo(FrameworkElement element)
    {
        element.SetBinding(FrameworkElement.RequestedThemeProperty, new Binding { Source = State, Path = new PropertyPath(nameof(AppearanceState.RequestedTheme)), Mode = BindingMode.OneWay });
    }

    private void UpdateState()
    {
        State.RequestedTheme = _current.Theme switch
        {
            AppTheme.Light => ElementTheme.Light,
            AppTheme.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default
        };
    }
}
