namespace MagicPlayExporter.Models;

public class Player
{
    public int Id { get; set; }
    public bool IsAnonymous { get; set; }
    public string MetaData { get; set; } = string.Empty;
    public string ModificationDate { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
}
