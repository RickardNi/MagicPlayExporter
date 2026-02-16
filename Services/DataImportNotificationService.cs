namespace MagicPlayExporter.Services;

public class DataImportNotificationService
{
    public event EventHandler? DataImported;

    public void NotifyDataImported()
    {
        DataImported?.Invoke(this, EventArgs.Empty);
    }
}
