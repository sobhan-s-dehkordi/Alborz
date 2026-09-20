using Microsoft.UI.Xaml;

namespace Alborz.WinUI.Services;

// Only this XAML-native object crosses the ResourceDictionary/WinRT boundary.
// Persistence and dependency injection remain in AppearanceService.
public sealed class AppearanceState : DependencyObject
{
    public static readonly DependencyProperty RequestedThemeProperty = DependencyProperty.Register(
        nameof(RequestedTheme), typeof(ElementTheme), typeof(AppearanceState), new PropertyMetadata(ElementTheme.Default));

    public ElementTheme RequestedTheme
    {
        get => (ElementTheme)GetValue(RequestedThemeProperty);
        set => SetValue(RequestedThemeProperty, value);
    }
}
