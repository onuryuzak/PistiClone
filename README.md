# Pisti Clone

A card game implementation based on the traditional Turkish card game "Pişti" built with Unity.

## Table of Contents
- [Introduction](#introduction)
- [Game Overview](#game-overview)
- [Project Structure](#project-structure)
- [Core Game Components](#core-game-components)
- [Game Mechanics](#game-mechanics)
- [Setup & Configuration](#setup--configuration)
- [AI Implementation](#ai-implementation)

## Introduction

This project is a digital implementation of the popular Turkish card game "Pişti" (pronounced "pish-tee"). The game supports both 2-player and 4-player modes, with AI opponents and betting functionality.

## Game Overview

Pişti is a traditional Turkish card game played with a standard 52-card deck. The goal is to collect cards by matching or creating special combinations. Players earn points for collecting cards, with special bonuses for specific combinations like a "pişti" (when a card exactly matches the top card of the pile).

## Project Structure

The project follows a modular architecture with clear separation of concerns:

```
Assets/
├── Arts/               # Game art assets, sprites, and visuals
├── Plugins/            # Third-party plugins and libraries (includes DOTween)
├── Prefabs/            # Reusable game object templates
├── Resources/          # Runtime-loadable assets
├── Scenes/             # Unity scenes (GameScene, TestScene)
├── Scripts/            # C# code organized by functionality
│   ├── AI/             # AI player logic and behavior
│   ├── Controllers/    # Player and game controllers
│   ├── Enums/          # Game state and type definitions
│   ├── GamePlay/       # Core game mechanics
│   ├── Managers/       # Singleton managers for game systems
│   ├── ScriptableObjects/ # Data containers and configurations
│   ├── UIScripts/      # UI-related functionality
│   └── Utilities/      # Helper functions and utilities
└── TextMesh Pro/       # Text rendering system
```

## Core Game Components

### Cards
The `Card` class represents individual cards with properties like:
- Card type (Hearts, Clubs, Diamonds, Spades)
- Card value (numeric value)
- Point value
- Visual representation (front and back sides)
- Movement and interaction logic

### Game Logic
The `GameLogic` class manages the core game rules, including:
- Turn management
- Card matching and collection
- Scoring mechanics
- Special combinations detection (like Pişti)
- Game state transitions

### Managers
The project uses the Singleton pattern for various managers:
- `CardManager`: Handles card creation, distribution, and management
- `EventManager`: Manages game events and communication between components
- `ExchangeManager`: Handles in-game currency and betting
- `GameManager`: Controls overall game flow and state
- `UIManager`: Manages UI elements and interactions
- `SaveSystem`: Handles player data persistence

## Game Mechanics

### Table Creation
Players can create game tables with customizable betting options:
- Select player count (2 or 4 players)
- Set minimum and maximum bet values
- Join existing tables or create new ones

### Card Distribution
Cards are dealt to players in rounds, with some cards placed face-up on the table to start the game.

### Turn System
Players take turns playing cards, with the current player indicated in the UI. The game follows a clockwise rotation of turns.

### Scoring
Points are awarded for:
- Collecting cards
- Creating special combinations (Pişti)
- Card values (Jacks, Aces, etc. have higher point values)

### Win Conditions
The player with the highest score at the end of the game wins the pot.

## Setup & Configuration

### Player Types
The game supports different player types:
- Human players
- AI opponents with different difficulty levels

### Currency System
The game includes a virtual currency system for betting with:
- Cash currency for betting
- Management of player winnings and losses

## AI Implementation

The game features AI opponents with different behavior levels. The AI system:
- Analyzes the game state to make decisions
- Remembers previously played cards
- Adjusts strategy based on the current game state
- Provides varying levels of challenge

---

Developed using Unity Engine. 
