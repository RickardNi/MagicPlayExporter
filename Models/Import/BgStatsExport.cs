namespace MagicPlayExporter.Models.Import;

public class BgStatsExport
{
    public List<Game> Games { get; set; } = new();
    public List<Location> Locations { get; set; } = new();
    public List<Player> Players { get; set; } = new();
    public List<Play> Plays { get; set; } = new();
}
