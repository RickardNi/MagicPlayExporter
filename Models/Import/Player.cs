namespace MagicPlayExporter.Models.Import;

public class Player
{
    public int Id { get; set; }
    public bool IsAnonymous { get; set; }
    public string MetaData { get; set; } = string.Empty;
    public string ModificationDate { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public List<PlayerTag>? Tags { get; set; }

    public int NumberOfPlays { get; set; }
}

public class PlayerTag
{
    public int TagRefId { get; set; }
}
