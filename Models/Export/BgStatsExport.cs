namespace MagicPlayExporter.Models.Export;

public class BgStatsExport
{
    public string About { get; set; } = "This is a Play file that can be read by Board Game Stats. If you see this text, try to use a share, export or open-in function to open it with Board Game Stats.";
    public List<Game> Games { get; set; } = new();
    public List<Location> Locations { get; set; } = new();
    public List<Player> Players { get; set; } = new();
    public List<Play> Plays { get; set; } = new();
    public UserInfo UserInfo { get; set; } = new() { MeRefId = 2 };
}

public class UserInfo
{
    public int MeRefId { get; set; }
}
