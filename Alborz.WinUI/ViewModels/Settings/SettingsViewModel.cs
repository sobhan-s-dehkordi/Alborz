using Alborz.Application.Contracts;
using Alborz.Application.Features.Settings;
using Alborz.WinUI.Services;
using Alborz.WinUI.ViewModels.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AppearanceService _appearance;
    private bool _initializing = true;
    private int _saveVersion;
    public string ServerInfo { get; }
    public IReadOnlyList<string> FontFamilies => _appearance.FontFamilies;
    public IReadOnlyList<string> Themes { get; } = new[] { "System default", "Light", "Dark" };
    [ObservableProperty] public partial string Status { get; set; } = string.Empty;
    [ObservableProperty] public partial string AppearanceStatus { get; set; } = string.Empty;
    [ObservableProperty] public partial int SelectedThemeIndex { get; set; }
    [ObservableProperty] public partial string SelectedFont { get; set; } = "Segoe UI";
    [ObservableProperty] public partial double SelectedFontSize { get; set; } = 14;

    public SettingsViewModel(IServiceScopeFactory scopeFactory, AppearanceService appearance)
    {
        _scopeFactory = scopeFactory;
        _appearance = appearance;
        SelectedThemeIndex = (int)appearance.Current.Theme;
        SelectedFont = appearance.Current.FontFamily;
        SelectedFontSize = appearance.Current.FontSize;
        _initializing = false;
        using var scope = scopeFactory.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IDatabaseStatusService>();
        ServerInfo = $"Server: {database.Server}\nDatabase: {database.Database}";
    }

    partial void OnSelectedThemeIndexChanged(int value) => QueueAppearanceSave();
    partial void OnSelectedFontChanged(string value) => QueueAppearanceSave();
    partial void OnSelectedFontSizeChanged(double value) => QueueAppearanceSave();

    private void QueueAppearanceSave()
    {
        if (!_initializing) _ = SaveAppearanceAsync();
    }

    private async Task SaveAppearanceAsync()
    {
        var version = ++_saveVersion;
        try
        {
            ErrorMessage = string.Empty;
            AppearanceStatus = "Saving appearance…";
            await _appearance.UpdateAsync(new AppearanceSettings((AppTheme)SelectedThemeIndex, SelectedFont, SelectedFontSize));
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
        SelectedFont = "Segoe UI";
        SelectedFontSize = 14;
        _initializing = false;
        await SaveAppearanceAsync();
    }

    [RelayCommand]
    public async Task TestConnectionAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            Status = await scope.ServiceProvider.GetRequiredService<IDatabaseStatusService>().CanConnectAsync()
                ? "Connection successful." : "Connection failed. Check the configuration and server.";
        }
        catch (Exception) { Status = "Connection failed. Check the configuration and server."; }
    }
}
