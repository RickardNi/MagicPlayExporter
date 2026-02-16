namespace MagicPlayExporter.Models.Export;

public class Play
{
    public string Board { get; set; } = string.Empty;
    public int DurationMin { get; set; } = 0;
    public string EntryDate { get; set; } = string.Empty;
    public List<object> ExpansionPlays { get; set; } = new();
    public int GameRefId { get; set; }
    public bool Ignored { get; set; } = false;
    public int LocationRefId { get; set; }
    public bool ManualWinner { get; set; } = true;
    public string MetaData { get; set; } = string.Empty;
    public string ModificationDate { get; set; } = string.Empty;
    public string PlayDate { get; set; } = string.Empty;
    public List<PlayerScore> PlayerScores { get; set; } = new();
    public int Rounds { get; set; } = 0;
    public int ScoringSetting { get; set; } = 3;
    public bool UsesTeams { get; set; } = false;
    public string Uuid { get; set; } = string.Empty;
}
