namespace MagicPlayExporter.Models.Export;

public class Player
{
    public int Id { get; set; }
    public bool IsAnonymous { get; set; }
    public string MetaData { get; set; } = "{\"isNpc\":0}";
    public string ModificationDate { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
}
