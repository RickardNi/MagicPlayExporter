using MagicPlayExporter.Models.Import;
using System.Text.Json;

namespace MagicPlayExporter.Services;

public class BgStatsImportService
{
    private const int MinimumNumberOfPlays = 5;

    public static async Task<BgStatsImportResult> ImportFromJsonAsync(string jsonContent)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var export = JsonSerializer.Deserialize<BgStatsExport>(jsonContent, options);
            
            if (export == null)
            {
                return BgStatsImportResult.Failure("Failed to deserialize JSON.");
            }

            var game = export.Games.FirstOrDefault(g => g.Id == 39 && g.BggId == 463);
            if (game == null)
            {
                return BgStatsImportResult.Failure("Game with id 39 and bggId 463 not found.");
            }

            GameMetaData? gameMetaData = null;
            if (!string.IsNullOrWhiteSpace(game.MetaData))
            {
                try
                {
                    gameMetaData = JsonSerializer.Deserialize<GameMetaData>(game.MetaData, options);
                }
                catch
                {
                    // MetaData parsing is optional
                }
            }

            var location = export.Locations.FirstOrDefault(l => l.Id == 16);
            if (location == null)
            {
                return BgStatsImportResult.Failure("Location with id 16 not found.");
            }

            var relevantPlays = export.Plays
                .Where(p => p.GameRefId == 39)
                .ToList();

            var playerPlayCounts = new Dictionary<int, int>();
            foreach (var play in relevantPlays)
            {
                foreach (var playerScore in play.PlayerScores)
                {
                    if (!playerPlayCounts.ContainsKey(playerScore.PlayerRefId))
                    {
                        playerPlayCounts[playerScore.PlayerRefId] = 0;
                    }
                    playerPlayCounts[playerScore.PlayerRefId]++;
                }
            }

            var activePlayers = export.Players
                .Where(p => playerPlayCounts.ContainsKey(p.Id) && playerPlayCounts[p.Id] >= MinimumNumberOfPlays)
                .Select(p => new PlayerWithPlayCount
                {
                    Player = p,
                    PlayCount = playerPlayCounts[p.Id]
                })
                .OrderByDescending(p => p.PlayCount)
                .ToList();

            return BgStatsImportResult.Success(game, gameMetaData, location, activePlayers, relevantPlays.Count);
        }
        catch (Exception ex)
        {
            return BgStatsImportResult.Failure($"Import error: {ex.Message}");
        }
    }
}

public class BgStatsImportResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public Game? Game { get; set; }
    public GameMetaData? GameMetaData { get; set; }
    public Location? Location { get; set; }
    public List<PlayerWithPlayCount> ActivePlayers { get; set; } = new();
    public int TotalPlaysCount { get; set; }

    public static BgStatsImportResult Success(Game game, GameMetaData? gameMetaData, Location location, 
        List<PlayerWithPlayCount> activePlayers, int totalPlaysCount)
    {
        return new BgStatsImportResult
        {
            IsSuccess = true,
            Game = game,
            GameMetaData = gameMetaData,
            Location = location,
            ActivePlayers = activePlayers,
            TotalPlaysCount = totalPlaysCount
        };
    }

    public static BgStatsImportResult Failure(string errorMessage)
    {
        return new BgStatsImportResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class PlayerWithPlayCount
{
    public Player Player { get; set; } = null!;
    public int PlayCount { get; set; }
}
