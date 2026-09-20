using Alborz.Application.Common;
using Alborz.Application.Features.Settings;
using Alborz.Infrastructure.Services;
using Xunit;

namespace Alborz.Tests;

public sealed class AppearanceAndLineTests
{
    [Theory]
    [InlineData(AppTheme.System)]
    [InlineData(AppTheme.Dark)]
    [InlineData(AppTheme.Light)]
    public async Task Appearance_survives_new_store_instance(AppTheme theme)
    {
        var path = Path.Combine(Path.GetTempPath(), "Alborz_" + Guid.NewGuid() + ".json");
        try
        {
            var expected = new AppearanceSettings(theme);
            await new JsonUserSettingsStore(path).SaveAppearanceAsync(expected);
            Assert.Equal(expected, new JsonUserSettingsStore(path).LoadAppearance());
            Assert.False(File.Exists(path + ".tmp"));
        }
        finally { File.Delete(path); }
    }

    [Theory]
    [InlineData("{broken")]
    [InlineData("null")]
    [InlineData("{\"Theme\":999}")]
    public void Invalid_settings_recover_to_defaults(string json)
    {
        var path = Path.Combine(Path.GetTempPath(), "Alborz_" + Guid.NewGuid() + ".json");
        try
        {
            File.WriteAllText(path, json);
            Assert.Equal(new AppearanceSettings(), new JsonUserSettingsStore(path).LoadAppearance());
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task Rapid_updates_leave_complete_latest_settings()
    {
        var path = Path.Combine(Path.GetTempPath(), "Alborz_" + Guid.NewGuid() + ".json");
        try
        {
            var store = new JsonUserSettingsStore(path);
            await Task.WhenAll(Enumerable.Range(0, 20).Select(i => store.SaveAppearanceAsync(new(AppTheme.Dark))));
            var final = new AppearanceSettings(AppTheme.Light);
            await store.SaveAppearanceAsync(final);
            Assert.Equal(final, new JsonUserSettingsStore(path).LoadAppearance());
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Duplicate_addition_preserves_price_and_discount_rate()
    {
        var line = DocumentLineEditor.Increase(1, 2, 100, 20, 3);
        Assert.Equal(new DocumentLine(5, 100, 50), line);
    }

    [Fact]
    public void Duplicate_addition_rejects_quantity_overflow() =>
        Assert.Throws<OverflowException>(() => DocumentLineEditor.Increase(1, int.MaxValue, 1, 0, 1));

    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(-1, 100, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(1, 100, 101)]
    [InlineData(1, 100, -1)]
    public void Editing_rejects_invalid_line(int quantity, decimal price, decimal discount) =>
        Assert.ThrowsAny<ArgumentException>(() => DocumentLineEditor.Validate(1, quantity, price, discount));

    [Fact]
    public void Editing_accepts_decimal_prices_and_discount() =>
        Assert.Equal(new DocumentLine(3, 120.25m, 10.75m), DocumentLineEditor.Validate(1, 3, 120.25m, 10.75m));
}
