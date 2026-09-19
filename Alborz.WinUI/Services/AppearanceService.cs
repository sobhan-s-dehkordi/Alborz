using Alborz.Application.Contracts;
using Alborz.Application.Features.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alborz.WinUI.Services;

public sealed class AppearanceService : ObservableObject
{
    private readonly IUserSettingsStore _store;
    private AppearanceSettings _current;
    public IReadOnlyList<string> FontFamilies { get; } = FontCatalog.GetInstalledFamilies();
    public AppearanceSettings Current => _current;
    public FontFamily FontFamily => new(_current.FontFamily);
    public double FontSize => _current.FontSize;
    public ElementTheme RequestedTheme => _current.Theme switch
    {
        AppTheme.Light => ElementTheme.Light,
        AppTheme.Dark => ElementTheme.Dark,
        _ => ElementTheme.Default
    };

    public AppearanceService(IUserSettingsStore store)
    {
        _store = store;
        _current = store.LoadAppearance();
        if (!FontFamilies.Contains(_current.FontFamily)) _current = _current with { FontFamily = "Segoe UI" };
    }

    public async Task UpdateAsync(AppearanceSettings settings)
    {
        _current = settings.Normalize();
        OnPropertyChanged(nameof(Current));
        OnPropertyChanged(nameof(FontFamily));
        OnPropertyChanged(nameof(FontSize));
        OnPropertyChanged(nameof(RequestedTheme));
        // Default templates for newly created controls use the selected family, too.
        Microsoft.UI.Xaml.Application.Current.Resources["ContentControlThemeFontFamily"] = FontFamily;
        await _store.SaveAppearanceAsync(_current);
    }

    public void ApplyTo(FrameworkElement element)
    {
        element.SetBinding(FrameworkElement.RequestedThemeProperty, new Binding { Source = this, Path = new PropertyPath(nameof(RequestedTheme)) });
        if (element is Control control)
        {
            control.SetBinding(Control.FontFamilyProperty, new Binding { Source = this, Path = new PropertyPath(nameof(FontFamily)) });
            control.SetBinding(Control.FontSizeProperty, new Binding { Source = this, Path = new PropertyPath(nameof(FontSize)) });
        }
    }
}
