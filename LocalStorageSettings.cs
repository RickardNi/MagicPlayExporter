using Blazored.LocalStorage;

namespace MagicPlayExporter;

public class LocalStorageSettings(ILocalStorageService localStorageService) : ISettingsStorage
{
    private readonly ILocalStorageService _localStorageService = localStorageService;

    public const string Players = "Players";
    public const string GameMetaData = "GameMetaData";
    public const string Game = "Game";
    public const string Location = "Location";

    public async Task<T?> GetSettingAsync<T>(string key)
    {
        try
        {
            return await _localStorageService.GetItemAsync<T>(key);
        }
        catch (Exception)
        {
            // Local storage might be disabled, full, or data might be corrupted
            return default;
        }
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        try
        {
            await _localStorageService.SetItemAsync(key, value);
        }
        catch (Exception)
        {
            // Local storage might be disabled, full, or in private browsing mode
        }
    }
}
