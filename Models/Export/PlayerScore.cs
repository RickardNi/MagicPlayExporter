using System.Text.Json.Serialization;

namespace MagicPlayExporter.Models.Export;

public class PlayerScore
{
    public bool NewPlayer { get; set; } = false;
    public int PlayerRefId { get; set; }
    public int Rank { get; set; }
    public string? Role { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public int? Score { get; set; } = null;
    public int SeatOrder { get; set; } = 0;
    public bool StartPlayer { get; set; } = false;
    public bool Winner { get; set; } = false;
}
