namespace MagicPlayExporter.Models;

using MagicPlayExporter.Models.Import;

public class GameplayRow
{
    public Player? Player1Object { get; set; }
    public Player? Player2Object { get; set; }

    public string Player1 
    { 
        get => Player1Object?.Name ?? string.Empty;
        set { } // Keep setter for compatibility but don't use it
    }

    public string Player2 
    { 
        get => Player2Object?.Name ?? string.Empty;
        set { } // Keep setter for compatibility but don't use it
    }

    public string StartingPlayer { get; set; } = string.Empty;
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public int GameTimeMinutes
    {
        get
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                var diff = EndTime.Value - StartTime.Value;
                if (diff.Ticks < 0) diff = diff.Add(TimeSpan.FromDays(1));
                return (int)diff.TotalMinutes;
            }
            return 0;
        }
    }
    public string Player1Deck { get; set; } = string.Empty;
    public string Player2Deck { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public HashSet<string> P1Colors { get; set; } = new();
    public HashSet<string> P2Colors { get; set; } = new();
}
