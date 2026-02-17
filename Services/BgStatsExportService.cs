using MagicPlayExporter.Models;
using MagicPlayExporter.Models.Import;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExportModels = MagicPlayExporter.Models.Export;

namespace MagicPlayExporter.Services;

public class BgStatsExportService
{
    public static ExportModels.BgStatsExport CreateExport(
        DateTime playDate,
        List<GameplayRow> gameplayRows,
        Game game,
        Location location,
        List<Player> allPlayers,
        string format,
        string set,
        string draftType)
    {
        var export = new ExportModels.BgStatsExport();

        // Add the single game
        export.Games.Add(new ExportModels.Game
        {
            Id = game.Id,
            BggId = game.BggId,
            BggName = game.BggName,
            BggYear = game.BggYear,
            Cooperative = game.Cooperative,
            Designers = game.Designers,
            HighestWins = game.HighestWins,
            IsBaseGame = game.IsBaseGame,
            IsExpansion = game.IsExpansion,
            ModificationDate = game.ModificationDate,
            Name = game.Name,
            Uuid = game.Uuid
        });

        // Add the single location
        export.Locations.Add(new ExportModels.Location
        {
            Id = location.Id,
            ModificationDate = location.ModificationDate,
            Name = location.Name,
            Uuid = location.Uuid
        });

        // Get unique players that are actually used
        var usedPlayerIds = new HashSet<int>();
        foreach (var row in gameplayRows)
        {
            if (row.Player1Object != null)
                usedPlayerIds.Add(row.Player1Object.Id);
            if (row.Player2Object != null)
                usedPlayerIds.Add(row.Player2Object.Id);
        }

        // Add only the players that are used
        foreach (var playerId in usedPlayerIds)
        {
            var player = allPlayers.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                export.Players.Add(new ExportModels.Player
                {
                    Id = player.Id,
                    IsAnonymous = player.IsAnonymous,
                    ModificationDate = player.ModificationDate,
                    Name = player.Name,
                    Uuid = player.Uuid
                });
            }
        }

        // Create plays from gameplay rows
        foreach (var row in gameplayRows)
        {
            if (row.Player1Object == null || row.Player2Object == null)
                continue;

            // Calculate entry date string from playDate + StartTime
            var entryDateString = FormatEntryDate(playDate, row.StartTime);

            var play = new ExportModels.Play
            {
                Board = GetBoardString(format, set, draftType),
                DurationMin = row.GameTimeMinutes,
                EntryDate = entryDateString,
                ExpansionPlays = new List<object>(),
                GameRefId = game.Id,
                Ignored = false,
                LocationRefId = location.Id,
                ManualWinner = true,
                ModificationDate = entryDateString,
                PlayDate = entryDateString,
                PlayerScores = new List<ExportModels.PlayerScore>(),
                Rounds = 0,
                ScoringSetting = 3,
                UsesTeams = false,
                Uuid = Guid.NewGuid().ToString()
            };

            // Add player 1 score
            play.PlayerScores.Add(CreatePlayerScore(
                row.Player1Object,
                row.Winner == "P1",
                row.StartingPlayer == "P1",
                GetRoleForPlayer(row.Player1Deck, row.P1Colors, format)
            ));

            // Add player 2 score
            play.PlayerScores.Add(CreatePlayerScore(
                row.Player2Object,
                row.Winner == "P2",
                row.StartingPlayer == "P2",
                GetRoleForPlayer(row.Player2Deck, row.P2Colors, format)
            ));

            export.Plays.Add(play);
        }

        return export;
    }

    private static string FormatEntryDate(DateTime playDate, TimeSpan? startTime)
    {
        var dateTime = playDate.Date;
        if (startTime.HasValue)
        {
            dateTime = dateTime.Add(startTime.Value);
        }

        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private static string GetBoardString(string format, string set, string draftType)
    {
        if (format == "BattleDecks")
        {
            return "Battle Decks";
        }
        else // Draft
        {
            // Format: "Draft／[Set]／[Draft Type]"
            // But skip Draft Type if it's "Draft" since we already have "Draft" at the start
            if (draftType == "Draft")
            {
                return $"Draft／{set}";
            }
            else
            {
                return $"Draft／{set}／{draftType}";
            }
        }
    }

    private static ExportModels.PlayerScore CreatePlayerScore(
        Player player,
        bool isWinner,
        bool isStartPlayer,
        string? role)
    {
        return new ExportModels.PlayerScore
        {
            NewPlayer = false,
            PlayerRefId = player.Id,
            Rank = isWinner ? 1 : 2,
            Role = role,
            Score = null,
            SeatOrder = 0,
            StartPlayer = isStartPlayer,
            Winner = isWinner
        };
    }

    private static string? GetRoleForPlayer(string deck, HashSet<string> colors, string format)
    {
        if (format == "BattleDecks")
        {
            return !string.IsNullOrWhiteSpace(deck) ? deck : null;
        }

        if (colors.Count == 0)
            return null;

        var colorMap = new Dictionary<string, string>
        {
            { "W", "White" },
            { "U", "Blue" },
            { "B", "Black" },
            { "R", "Red" },
            { "G", "Green" }
        };

        var fullColorNames = colors
            .Select(c => colorMap.GetValueOrDefault(c, c))
            .OrderBy(c => c);

        return string.Join("／", fullColorNames);
    }

    public static string SerializeToJson(ExportModels.BgStatsExport export)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(export, options);
    }
}
