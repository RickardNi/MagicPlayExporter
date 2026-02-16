namespace MagicPlayExporter.Models.Import;

public class Play
{
    public int GameRefId { get; set; }
    public List<PlayerScore> PlayerScores { get; set; } = new();
}

public class PlayerScore
{
    public int PlayerRefId { get; set; }
}
