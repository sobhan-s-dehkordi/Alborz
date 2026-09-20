using Alborz.Application.Features.Settings;
using Alborz.WinUI.Services;
using Alborz.WinUI.ViewModels.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly AppearanceService _appearance;
    private bool _initializing = true;
    private int _saveVersion;

    public IReadOnlyList<string> Themes { get; } = new[] { "System default", "Light", "Dark" };

    [ObservableProperty] public partial string AppearanceStatus { get; set; } = string.Empty;
    [ObservableProperty] public partial int SelectedThemeIndex { get; set; }

    public SettingsViewModel(AppearanceService appearance)
    {
        _appearance = appearance;
        SelectedThemeIndex = (int)appearance.Current.Theme;
        _initializing = false;
    }

    partial void OnSelectedThemeIndexChanged(int value) => QueueAppearanceSave();

    private void QueueAppearanceSave()
    {
        if (!_initializing && SelectedThemeIndex >= 0) _ = SaveAppearanceAsync();
    }

    private async Task SaveAppearanceAsync()
    {
        var version = ++_saveVersion;
        try
        {
            ErrorMessage = string.Empty;
            AppearanceStatus = "Saving appearance - ";
            await _appearance.UpdateAsync(new AppearanceSettings((AppTheme)SelectedThemeIndex));
            if (version == _saveVersion) AppearanceStatus = "Applied and saved for the next launch.";
        }
        catch (Exception)
        {
            if (version == _saveVersion)
            {
                AppearanceStatus = string.Empty;
                ErrorMessage = "Appearance is applied, but could not be saved. Check access to your local application data folder.";
            }
        }
    }

    [RelayCommand]
    public async Task ResetAppearanceAsync()
    {
        _initializing = true;
        SelectedThemeIndex = (int)AppTheme.System;
        _initializing = false;
        await SaveAppearanceAsync();
    }
}
