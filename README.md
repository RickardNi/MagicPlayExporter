# MagicPlayExporter

A Blazor WebAssembly application for creating an import file for BG Stats with Magic: The Gathering gameplay sessions.

**🌐 [Demo](https://rickardni.github.io/MagicPlayExporter/)**

## What This Tool Does

This is a **data entry and file generation tool** for BG Stats. I track my Magic: The Gathering games on paper during play sessions, then use this tool to digitize that data and generate a properly formatted .bgsplay file that can be imported into the [BG Stats](https://www.bgstatsapp.com/) mobile app.

The tool handles the BG Stats JSON format, validates the data, and ensures all player IDs, game references, and metadata are correctly structured for a seamless import.

More information about importing files into BG Stats can be found [here](https://www.bgstatsapp.com/explanations/importing-own-files-into-board-game-stats/).

## ⚠️ Important Notice

**This is a highly specialized tool built for my personal use.** It contains many hard-coded values, specific game IDs, player configurations, and workflows tailored to my exact needs. While the code is open source for reference and inspiration, it is **not intended as a general-purpose application** and will likely not work out-of-the-box for other users without significant modifications.

Feel free to explore the code, learn from it, and adapt it to your own needs!

I have vibe-coded this app with different tools, mainly Gemini CLI and GitHub Copilot (using Sonnet-4.5 mostly).

## Features

### 📋 Game Session Tracking
- **Time Tracking**: Record start time, end time, and automatically calculate game duration
	- If there are previous start times entered, it will automatically begin from that time
	- After entering start time, it will auto-fill an end time 15 minutes later, which is the average time for our games
- **Smart Player Selection**: Can only select a player once per game, and players already from the session take priority in the dropdown list (since they are likely to occur in future games because they are attending the event)
- **Visual Indicators**: Clear visual feedback for winners and starting players
- **Auto-Save**: All form data automatically saves to browser local storage so you don't lose data by mistake!

### 🎨 Draft-Specific Features
- **Color Tracking**: Track which Magic colors each player used in their draft deck
- **Visual Color Selection**: Easy-to-use color picker for the 5 Magic colors (WURBG) (exported as roles with the full color name)
- **Per-Player Color Tracking**: Colors are automatically tracked and associated with players
	- During draft, a player usually have the same colors each game, and they are synced for each player so you only need to enter it once per player

### 🎮 Format Support
- **Draft Format**: 
  - Select from available sets defined in BG Stats app (e.g., Magic Foundations Cube)
  - Choose draft type (Draft, Pick-Two Draft, Sealed, Winston, Winchester, Grid, Minneapolis)
- **Battle Decks Format**: 
  - Select from pre-configured battle decks from BG Stats app (roles)
  - Deck selection with auto-complete

### 📊 BG Stats Integration
- **Import from BG Stats**: 
  - Upload BG Stats JSON export files (drag and drop for file upload)
  - Automatically extract player lists and deck configurations
  - Automatically filter out archived players and low-activity players (< 5 plays)
  - Filter and organize player data
- **Export to BG Stats**:
  - Generate properly formatted BG Stats file (.bgsplay)
  - Include all game metadata (players, scores, duration, date and time)
  - Preserve player IDs and references for seamless integration
  - Custom metadata fields for format, set, and draft type information

### ✅ Data Validation
- **Pre-Export Validation**: Validates all data before export
- **Missing Data Detection**: Identifies incomplete entries (missing players, times, winners)
- **Interactive Dialog**: Review and fix issues before exporting
- **Color Validation**: Warns if players have no colors assigned (in Draft format)

## Technology Stack

- **Framework**: .NET 10.0 / C# 14.0
- **UI Framework**: Blazor WebAssembly
- **Component Library**: [MudBlazor 8.15.0](https://mudblazor.com/)
- **Storage**: [Blazored.LocalStorage 4.5.0](https://github.com/Blazored/LocalStorage)

## Hard-Coded Elements

This application includes several hard-coded values specific to my setup:

- **Game ID**: 39 (Magic: The Gathering)
- **BGG ID**: 463
- **Location ID**: 16
- **Default Sets**: Magic Foundations Cube
- **Draft Types**: Specific list of draft formats I use
- **Player Filtering**: Minimum 5 plays to be considered "active"
- **Archived Tag ID**: 3
- **Battle Deck Configuration**: Specific deck list from my collection
- **Metadata Structure**: Custom metadata format for my BG Stats setup
