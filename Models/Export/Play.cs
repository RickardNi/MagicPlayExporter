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
    public string MetaData { get; set; } = "{\"playerRefId\":2,\"playGameBggVersion\":\"{\\\"versionId\\\":0,\\\"gameName\\\":\\\"\\\",\\\"versionName\\\":\\\"\\\",\\\"imageUrl\\\":\\\"https:\\\\\\/\\\\\\/cf.geekdo-images.com\\\\\\/CxJmNl4wR4InjqyNrMdBTw__thumb\\\\\\/img\\\\\\/TtlQgYxLTPyYQWJvruMHfwKPReE=\\\\\\/fit-in\\\\\\/200x150\\\\\\/filters:strip_icc()\\\\\\/pic163749.jpg\\\",\\\"thumbUrl\\\":\\\"https:\\\\\\/\\\\\\/cf.geekdo-images.com\\\\\\/CxJmNl4wR4InjqyNrMdBTw__thumb\\\\\\/img\\\\\\/TtlQgYxLTPyYQWJvruMHfwKPReE=\\\\\\/fit-in\\\\\\/200x150\\\\\\/filters:strip_icc()\\\\\\/pic163749.jpg\\\",\\\"yearPublished\\\":0}\",\"playUsedGameCopy\":2}";
    public string ModificationDate { get; set; } = string.Empty;
    public string PlayDate { get; set; } = string.Empty;
    public List<PlayerScore> PlayerScores { get; set; } = new();
    public int Rounds { get; set; } = 0;
    public int ScoringSetting { get; set; } = 3;
    public bool UsesTeams { get; set; } = false;
    public string Uuid { get; set; } = string.Empty;
}
