namespace MagicPlayExporter;

public interface ISettingsStorage
{
    Task<T?> GetSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);
}
