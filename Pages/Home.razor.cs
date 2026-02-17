using MagicPlayExporter.Components;
using MagicPlayExporter.Models;
using MagicPlayExporter.Models.Import;
using MagicPlayExporter.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MagicPlayExporter.Pages;

public partial class Home : IDisposable
{
    [Inject] private ISettingsStorage SettingsStorage { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private DataImportNotificationService NotificationService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private DateTime? playDate = DateTime.Today;
    private string selectedFormat = "Draft";
    private string selectedSet = "Magic Foundations";
    private string selectedDraftType = "Draft";

    private List<Player> playerList = new();
    private bool isFormDirty = false;
    private Dictionary<string, HashSet<string>> playerColorsMap = new();

    private List<GameplayRow> gameplayRows = new();

    private List<string> availableDraftTypes = new List<string>
    {
        "Draft",
        "Pick-Two Draft",
        "Sealed",
        "Winston Draft",
        "Winchester Draft",
        "Grid Draft",
        "Minneapolis Draft"
    };

    private List<string> availableSets = new List<string>
    {
        "Magic Foundations Cube"
    };

    private List<DeckInfo> availableDecks = new();

    private string ToFullDeckName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return displayName;

        var deck = availableDecks.FirstOrDefault(d => d.DisplayName == displayName);
        return deck?.FullName ?? displayName;
    }

    private string ToDisplayDeckName(string fullOrDisplayName)
    {
        if (string.IsNullOrWhiteSpace(fullOrDisplayName))
            return fullOrDisplayName;

        var deck = availableDecks.FirstOrDefault(d => d.FullName == fullOrDisplayName);
        if (deck != null)
            return deck.DisplayName;

        // If not found, check if it's already a display name
        deck = availableDecks.FirstOrDefault(d => d.DisplayName == fullOrDisplayName);
        return deck?.DisplayName ?? fullOrDisplayName;
    }

    private void OnStartTimeFocus(GameplayRow currentRow, int currentIndex)
    {
        if (currentRow.StartTime.HasValue)
        {
            return;
        }

        for (int i = currentIndex - 1; i >= 0; i--)
        {
            if (gameplayRows[i].StartTime.HasValue)
            {
                currentRow.StartTime = gameplayRows[i].StartTime;
                break;
            }
        }
    }

    private void OnStartTimeBlur(GameplayRow row)
    {
        if (row.StartTime.HasValue && !row.EndTime.HasValue)
        {
            var end = row.StartTime.Value.Add(TimeSpan.FromMinutes(15));
            if (end.TotalDays >= 1) end = end.Subtract(TimeSpan.FromDays(1));
            row.EndTime = end;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        NotificationService.DataImported += OnDataImported;

        await LoadPlayersFromStorageAsync();
        await LoadGameMetaDataFromStorageAsync();
        await LoadFormDataFromStorageAsync();
    }

    private async void OnDataImported(object? sender, EventArgs e)
    {
        await LoadPlayersFromStorageAsync();
        await LoadGameMetaDataFromStorageAsync();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        NotificationService.DataImported -= OnDataImported;
    }

    private async Task LoadPlayersFromStorageAsync()
    {
        var savedPlayers = await SettingsStorage.GetSettingAsync<List<Player>>(LocalStorageSettings.Players);
        if (savedPlayers != null && savedPlayers.Count > 0)
        {
            playerList = savedPlayers;
        }
        else
        {
            var hasImportedData = await SettingsStorage.GetSettingAsync<bool>(LocalStorageSettings.HasImportedData);
            if (!hasImportedData)
            {
                playerList = CreateSamplePlayers();
            }
        }
    }

    private List<Player> CreateSamplePlayers()
    {
        return
        [
            new Player { Id = -1, Name = "Anna Lundgren", IsAnonymous = false, Uuid = Guid.NewGuid().ToString(), NumberOfPlays = 0 },
            new Player { Id = -2, Name = "Björn Ekström", IsAnonymous = false, Uuid = Guid.NewGuid().ToString(), NumberOfPlays = 0 },
            new Player { Id = -3, Name = "Carina Holm", IsAnonymous = false, Uuid = Guid.NewGuid().ToString(), NumberOfPlays = 0 },
            new Player { Id = -4, Name = "Daniel Berg", IsAnonymous = false, Uuid = Guid.NewGuid().ToString(), NumberOfPlays = 0 }
        ];
    }

    private async Task LoadGameMetaDataFromStorageAsync()
    {
        var gameMetaData = await SettingsStorage.GetSettingAsync<GameMetaData>(LocalStorageSettings.GameMetaData);

        if (gameMetaData?.GameAddedBoards != null && gameMetaData.GameAddedBoards.Count > 0)
        {
            var predefinedDraftTypes = new List<string>
            {
                "Draft",
                "Pick-Two Draft",
                "Sealed",
                "Winston Draft",
                "Winchester Draft",
                "Grid Draft",
                "Minneapolis Draft"
            };

            var predefinedSets = new List<string>
            {
                "Magic Foundations Cube"
            };

            var importedDraftTypes = gameMetaData.GameAddedBoards
                .Where(board => predefinedDraftTypes.Contains(board))
                .ToList();

            var importedSets = gameMetaData.GameAddedBoards
                .Where(board => predefinedSets.Contains(board))
                .ToList();

            if (importedDraftTypes.Count > 0)
            {
                availableDraftTypes = predefinedDraftTypes
                    .Where(type => importedDraftTypes.Contains(type))
                    .ToList();

                if (availableDraftTypes.Count > 0)
                {
                    selectedDraftType = availableDraftTypes[0];
                }
            }

            if (importedSets.Count > 0)
            {
                availableSets = predefinedSets
                    .Where(set => importedSets.Contains(set))
                    .ToList();

                if (availableSets.Count > 0)
                {
                    selectedSet = availableSets[0];
                }
            }
        }

        if (gameMetaData?.GameAddedRoles != null && gameMetaData.GameAddedRoles.Count > 0)
        {
            var importedDecks = gameMetaData.GameAddedRoles
                .Where(role => role.StartsWith("[Battle Deck]"))
                .Select(role => new DeckInfo
                {
                    FullName = role,
                    DisplayName = role.Substring("[Battle Deck]".Length).Trim()
                })
                .ToList();

            if (importedDecks.Count > 0)
            {
                availableDecks = importedDecks;
            }
        }
    }

    private async Task LoadFormDataFromStorageAsync()
    {
        var savedFormData = await SettingsStorage.GetSettingAsync<FormData>(LocalStorageSettings.FormData);

        if (savedFormData != null)
        {
            playDate = savedFormData.PlayDate;
            selectedFormat = savedFormData.SelectedFormat;
            selectedSet = savedFormData.SelectedSet;
            selectedDraftType = savedFormData.SelectedDraftType;
            playerColorsMap = savedFormData.PlayerColorsMap ?? new();

            gameplayRows.Clear();
            foreach (var rowData in savedFormData.GameplayRows)
            {
                var row = new GameplayRow
                {
                    Player1Object = rowData.Player1Id.HasValue 
                        ? playerList.FirstOrDefault(p => p.Id == rowData.Player1Id.Value) 
                        : null,
                    Player2Object = rowData.Player2Id.HasValue 
                        ? playerList.FirstOrDefault(p => p.Id == rowData.Player2Id.Value) 
                        : null,
                    StartingPlayer = rowData.StartingPlayer,
                    StartTime = rowData.StartTime,
                    EndTime = rowData.EndTime,
                    Player1Deck = rowData.Player1Deck,
                    Player2Deck = rowData.Player2Deck,
                    Winner = rowData.Winner,
                    P1Colors = rowData.P1Colors,
                    P2Colors = rowData.P2Colors
                };
                gameplayRows.Add(row);
            }
        }

        if (gameplayRows.Count == 0)
        {
            for (int i = 0; i < 12; i++)
            {
                gameplayRows.Add(new GameplayRow());
            }
        }
    }

    private async Task SaveFormDataToStorageAsync()
    {
        var formData = new FormData
        {
            PlayDate = playDate,
            SelectedFormat = selectedFormat,
            SelectedSet = selectedSet,
            SelectedDraftType = selectedDraftType,
            PlayerColorsMap = playerColorsMap,
            GameplayRows = gameplayRows.Select(row => new GameplayRowData
            {
                Player1Id = row.Player1Object?.Id,
                Player2Id = row.Player2Object?.Id,
                StartingPlayer = row.StartingPlayer,
                StartTime = row.StartTime,
                EndTime = row.EndTime,
                Player1Deck = row.Player1Deck,
                Player2Deck = row.Player2Deck,
                Winner = row.Winner,
                P1Colors = row.P1Colors,
                P2Colors = row.P2Colors
            }).ToList()
        };

        await SettingsStorage.SaveSettingAsync(LocalStorageSettings.FormData, formData);
    }

    private async Task AddRowAsync()
    {
        gameplayRows.Add(new GameplayRow());
        await SaveFormDataToStorageAsync();
    }

    private async Task RemoveRow(GameplayRow row)
    {
        if (HasRowData(row))
        {
            bool? result = await DialogService.ShowMessageBox(
                "Remove Row",
                "This row contains data. Are you sure you want to remove it?",
                yesText: "Remove",
                cancelText: "Cancel");

            if (result != true)
            {
                return;
            }
        }

        gameplayRows.Remove(row);
        await SaveFormDataToStorageAsync();
    }

    private bool HasRowData(GameplayRow row)
    {
        return !string.IsNullOrWhiteSpace(row.Player1)
            || !string.IsNullOrWhiteSpace(row.Player2)
            || row.StartTime.HasValue
            || row.EndTime.HasValue
            || !string.IsNullOrWhiteSpace(row.Player1Deck)
            || !string.IsNullOrWhiteSpace(row.Player2Deck)
            || !string.IsNullOrWhiteSpace(row.Winner)
            || !string.IsNullOrWhiteSpace(row.StartingPlayer)
            || row.P1Colors.Count > 0
            || row.P2Colors.Count > 0;
    }

    private class ValidationResult
    {
        public bool HasBlockingErrors { get; init; }
        public bool HasWarnings { get; init; }
        public List<string> ErrorMessages { get; init; } = new();
        public List<string> WarningMessages { get; init; } = new();
    }

    private async Task ExportData()
    {
        var validation = ValidateDataForExport();

        if (validation.HasBlockingErrors)
        {
            var parameters = new DialogParameters
            {
                ["HeaderMessage"] = "Cannot export data due to the following errors:",
                ["Messages"] = validation.ErrorMessages,
                ["FooterMessage"] = "Please fix these issues before exporting.",
                ["ShowProceedButton"] = false,
                ["CloseText"] = "OK"
            };

            await DialogService.ShowAsync<ValidationDialog>("Cannot Export", parameters);
            return;
        }

        if (validation.HasWarnings)
        {
            var parameters = new DialogParameters
            {
                ["HeaderMessage"] = "",
                ["Messages"] = validation.WarningMessages,
                ["FooterMessage"] = "Do you want to proceed with the export anyway?",
                ["ShowProceedButton"] = true,
                ["ProceedText"] = "Proceed Anyway",
                ["CancelText"] = "Cancel"
            };

            var dialog = await DialogService.ShowAsync<ValidationDialog>("Export Warning", parameters);
            var result = await dialog.Result;

            if (result is null || result.Canceled)
            {
                return;
            }
        }

        // Load game and location from storage
        var game = await SettingsStorage.GetSettingAsync<Game>(LocalStorageSettings.Game);
        var location = await SettingsStorage.GetSettingAsync<Location>(LocalStorageSettings.Location);

        if (game == null || location == null)
        {
            var parameters = new DialogParameters
            {
                ["HeaderMessage"] = "Missing required data:",
                ["Messages"] = new List<string> { "Please import a Board Game Stats file first to set up the game and location." },
                ["FooterMessage"] = "",
                ["ShowProceedButton"] = false,
                ["CloseText"] = "OK"
            };

            await DialogService.ShowAsync<ValidationDialog>("Cannot Export", parameters);
            return;
        }

        if (!playDate.HasValue)
        {
            var parameters = new DialogParameters
            {
                ["HeaderMessage"] = "Missing play date:",
                ["Messages"] = new List<string> { "Please select a date for the plays." },
                ["FooterMessage"] = "",
                ["ShowProceedButton"] = false,
                ["CloseText"] = "OK"
            };

            await DialogService.ShowAsync<ValidationDialog>("Cannot Export", parameters);
            return;
        }

        // Create the export
        var export = BgStatsExportService.CreateExport(
            playDate.Value,
            gameplayRows,
            game,
            location,
            playerList,
            selectedFormat,
            selectedSet,
            selectedDraftType);

        // Serialize to JSON
        var jsonContent = BgStatsExportService.SerializeToJson(export);

        // Create filename with selected date
        var filename = $"{playDate.Value:yyyy-MM-dd}.bgsplay";

        // Trigger download
        await DownloadFileAsync(filename, jsonContent);
    }

    private ValidationResult ValidateDataForExport()
    {
        var missingPlayerRows = new List<int>();
        var invalidWinnerRows = new List<int>();
        var duplicatePlayerRows = new List<int>();
        var missingStartPlayerRows = new List<int>();

        for (int i = 0; i < gameplayRows.Count; ++i)
        {
            var row = gameplayRows[i];
            var rowNumber = i + 1;

            var hasPlayer1 = !string.IsNullOrWhiteSpace(row.Player1);
            var hasPlayer2 = !string.IsNullOrWhiteSpace(row.Player2);

            if (!hasPlayer1 || !hasPlayer2)
            {
                missingPlayerRows.Add(rowNumber);
            }
            else if (row.Player1.Equals(row.Player2, StringComparison.OrdinalIgnoreCase))
            {
                duplicatePlayerRows.Add(rowNumber);
            }

            if (string.IsNullOrWhiteSpace(row.Winner) || (row.Winner != "P1" && row.Winner != "P2"))
            {
                invalidWinnerRows.Add(rowNumber);
            }

            if (string.IsNullOrWhiteSpace(row.StartingPlayer) || (row.StartingPlayer != "P1" && row.StartingPlayer != "P2"))
            {
                missingStartPlayerRows.Add(rowNumber);
            }
        }

        var errorMessages = new List<string>();

        if (missingPlayerRows.Count > 0)
        {
            errorMessages.Add($"Missing player(s) in row(s): {string.Join(", ", missingPlayerRows)}");
        }

        if (duplicatePlayerRows.Count > 0)
        {
            errorMessages.Add($"Same player listed twice in row(s): {string.Join(", ", duplicatePlayerRows)}");
        }

        if (invalidWinnerRows.Count > 0)
        {
            errorMessages.Add($"Missing or invalid winner in row(s): {string.Join(", ", invalidWinnerRows)}");
        }

        if (errorMessages.Count > 0)
        {
            return new ValidationResult
            {
                HasBlockingErrors = true,
                ErrorMessages = errorMessages
            };
        }

        if (missingStartPlayerRows.Count > 0)
        {
            return new ValidationResult
            {
                HasWarnings = true,
                WarningMessages = new List<string> { $"The following row(s) are missing a start player: {string.Join(", ", missingStartPlayerRows)}" }
            };
        }

        return new ValidationResult();
    }

    private void OnPlayerNameChanged(GameplayRow row, bool isP1, string newPlayerName)
    {
        var player = playerList.FirstOrDefault(p => p.Name == newPlayerName);

        if (isP1)
        {
            row.Player1Object = player;
        }
        else
        {
            row.Player2Object = player;
        }

        if (selectedFormat == "Draft" && !string.IsNullOrWhiteSpace(newPlayerName))
        {
            var targetColors = isP1 ? row.P1Colors : row.P2Colors;
            targetColors.Clear();

            if (playerColorsMap.TryGetValue(newPlayerName, out var savedColors))
            {
                foreach (var color in savedColors)
                {
                    targetColors.Add(color);
                }
            }
        }
    }

    private async Task ToggleColorForPlayer(string playerName, string color)
    {
        if (string.IsNullOrWhiteSpace(playerName) || selectedFormat != "Draft")
            return;

        if (!playerColorsMap.TryGetValue(playerName, out var colors))
        {
            colors = new HashSet<string>();
            playerColorsMap[playerName] = colors;
        }

        if (!colors.Remove(color))
        {
            colors.Add(color);
        }

        foreach (var row in gameplayRows)
        {
            if (row.Player1 == playerName)
            {
                row.P1Colors.Clear();
                if (playerColorsMap.TryGetValue(playerName, out var playerColors))
                {
                    foreach (var c in playerColors)
                    {
                        row.P1Colors.Add(c);
                    }
                }
            }
            if (row.Player2 == playerName)
            {
                row.P2Colors.Clear();
                if (playerColorsMap.TryGetValue(playerName, out var playerColors))
                {
                    foreach (var c in playerColors)
                    {
                        row.P2Colors.Add(c);
                    }
                }
            }
        }

        StateHasChanged();
        await SaveFormDataToStorageAsync();
    }

    private void OnPlayersImported(List<Player> players)
    {
        playerList = players;
        StateHasChanged();
    }

    private async Task<IEnumerable<string>> SearchPlayers(string value, CancellationToken token)
    {
        await Task.Delay(5, token);
        var playerNames = playerList.Select(p => p.Name).ToList();
        return string.IsNullOrEmpty(value) ? playerNames : playerNames.Where(p => p.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<string>> SearchPlayersForRow(string value, CancellationToken token, string otherPlayerName)
    {
        await Task.Delay(5, token);

        var usedPlayerNames = gameplayRows
            .SelectMany(r => new[] { r.Player1, r.Player2 })
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allPlayerNames = playerList.Select(p => p.Name).ToList();

        var usedPlayers = allPlayerNames
            .Where(name => usedPlayerNames.Contains(name))
            .OrderBy(name => name)
            .ToList();

        var unusedPlayers = allPlayerNames
            .Where(name => !usedPlayerNames.Contains(name))
            .OrderBy(name => name)
            .ToList();

        var orderedPlayerNames = usedPlayers.Concat(unusedPlayers).ToList();

        var filteredPlayers = orderedPlayerNames
            .Where(name => !name.Equals(otherPlayerName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (string.IsNullOrEmpty(value))
        {
            return filteredPlayers;
        }

        return filteredPlayers.Where(p => p.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<string>> SearchDecks(string value, CancellationToken token)
    {
        await Task.Delay(5, token);
        var deckNames = availableDecks
            .Select(d => d.DisplayName)
            .OrderBy(d => d)
            .ToList();
        return string.IsNullOrEmpty(value) ? deckNames : deckNames.Where(d => d.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task OnFormatChanged(string newFormat)
    {
        if (newFormat == selectedFormat)
            return;

        if (gameplayRows.Any(HasRowData))
        {
            bool? result = await DialogService.ShowMessageBox(
                "Clear Data Required",
                "You must clear all data before switching formats. Would you like to clear all data now?",
                yesText: "Clear & Switch",
                cancelText: "Cancel");

            if (result != true)
            {
                StateHasChanged();
                return;
            }

            gameplayRows.Clear();
            playerColorsMap.Clear();
            for (int i = 0; i < 12; i++)
            {
                gameplayRows.Add(new GameplayRow());
            }
        }

        selectedFormat = newFormat;
        await SaveFormDataToStorageAsync();
        StateHasChanged();
    }

    private async Task ClearAllData()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Clear All Data",
            "Are you sure you want to clear all data? This will remove all entries and cannot be undone.",
            yesText: "Clear",
            cancelText: "Cancel");

        if (result == true)
        {
            gameplayRows.Clear();
            playerColorsMap.Clear();
            for (int i = 0; i < 12; i++)
            {
                gameplayRows.Add(new GameplayRow());
            }
            await SaveFormDataToStorageAsync();
            StateHasChanged();
        }
    }

    private async Task DownloadFileAsync(string filename, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(bytes);

        await JSRuntime.InvokeVoidAsync("downloadFile", filename, base64);
    }
}
