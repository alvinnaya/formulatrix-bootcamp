using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Controller;
using Fleck;
using System.Text.Json;







        var rooms = new Dictionary<string, GameController>();
        List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        var server = new WebSocketServer("ws://0.0.0.0:5000");
    
        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Console.WriteLine("Client connected!");
                 allSockets.Add(socket);
                
            };

            socket.OnClose = () =>
            {
                Console.WriteLine("Client disconnected");
                allSockets.Remove(socket);
            };

            socket.OnMessage = message =>
            {
                Console.WriteLine("Received: " + message);
                HandleMessage(socket, message);
            };
        });

        Console.WriteLine("WebSocket server running on ws://localhost:5000");
        Console.ReadLine(); // biar server tetap jalan



        void HandleMessage(IWebSocketConnection socket, string message)
    {
        var parts = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            SendJson(socket, "error", new { message = "invalid command format" });
            return;
        }

        var command = parts[1].ToLower();
        switch (command)
        {
            case "init":
                Broadcast("Server got init: ");
                break;

            case "createroom":
                if (!RequireGamePrefix(socket, parts)) break;
                if (!TryGetRoomId(socket, parts, 2, out var newRoomId)) break;
                if (parts.Length < 4)
                {
                    SendJson(socket, "error", new { message = "missing roomId or maxPlayers" });
                    break;
                }
                if (rooms.ContainsKey(newRoomId))
                {
                    SendJson(socket, "error", new { message = "room already exists" });
                    break;
                }
                if (!int.TryParse(parts[3], out var maxPlayers) || maxPlayers <= 0)
                {
                    SendJson(socket, "error", new { message = "invalid maxPlayers" });
                    break;
                }
                CreateRoom(newRoomId, maxPlayers);
                SendJson(socket, "info", new { message = $"room {newRoomId} created" });
                break;

            case "closeroom":
                if (!RequireGamePrefix(socket, parts)) break;
                if (!TryGetRoomId(socket, parts, 2, out var closeRoomId)) break;
                if (!rooms.ContainsKey(closeRoomId))
                {
                    SendJson(socket, "error", new { message = "room not found" });
                    break;
                }
                rooms.Remove(closeRoomId);
                SendJson(socket, "info", new { message = $"room {closeRoomId} closed" });
                break;

            case "start":
                if (!RequireGamePrefix(socket, parts)) break;
                if (!TryGetRoomId(socket, parts, 2, out var startRoomId)) break;
                if (!TryGetGame(socket, startRoomId, requireStarted: false, out var startGame)) break;
                if (startGame.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game already started" });
                    break;
                }
                if (startGame.Players.Count == 0)
                {
                    SendJson(socket, "error", new { message = "no players in room" });
                    break;
                }
                foreach (var p in startGame.Players)
                for (int i = 0; i < 7; i++)
                {
                    startGame.DrawCard(p);
                }
                startGame.StartGame();
                BroadcastRoomJson(startRoomId, "gameState", new
                {
                    lastCard = startGame.GetLastPlayedCard().ToString(),
                    currentPlayer = startGame.GetCurrentPlayer().Name,
                    allPlayers = GetPlayersCardCount(startGame),
                    lastColor = startGame.CurrentColor.ToString()
                });
                break;

            case "getcurrentstate":
                if (!RequireGamePrefix(socket, parts)) break;
                if (!TryGetRoomId(socket, parts, 2, out var stateRoomId)) break;
                if (!TryGetGame(socket, stateRoomId, requireStarted: true, out var stateGame)) break;
                SendJson(socket, "gameState", new
                {
                    lastCard = stateGame.GetLastPlayedCard().ToString(),
                    currentPlayer = stateGame.GetCurrentPlayer().Name,
                    allPlayers = GetPlayersCardCount(stateGame),
                    lastColor = stateGame.CurrentColor.ToString()

                });
                break;

            case "listrooms":
                if (!RequireGamePrefix(socket, parts)) break;
                var roomList = rooms.Select(kvp => new
                {
                    roomId = kvp.Key,
                    players = kvp.Value.Players.Select(p => p.Name).ToList()
                }).ToList();
                SendJson(socket, "roomList", new { rooms = roomList });
                break;

            case "getcard":
                if (!TryGetRoomId(socket, parts, 2, out var cardRoomId)) break;
                if (!TryGetGame(socket, cardRoomId, requireStarted: true, out var cardGame)) break;
                var cardPlayer = cardGame.GetPlayerByName(parts[0]);
                if (cardPlayer == null)
                {
                    SendJson(socket, "error", new { message = "Player not found" });
                    break;
                }
                var cardHand = cardGame.GetPlayerCards(cardPlayer);
                SendJson(socket, "playerState", new
                {
                    lastCard = cardGame.GetLastPlayedCard().ToString(),
                    player = cardPlayer.Name,
                    hand = cardHand.Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                });
                break;

            case "draw":
                if (!TryGetRoomId(socket, parts, 2, out var drawRoomId)) break;
                if (!TryGetGame(socket, drawRoomId, requireStarted: true, out var drawGame)) break;
                var drawPlayer = drawGame.GetCurrentPlayer();
                if (!IsCurrentPlayer(socket, parts[0], drawPlayer)) break;
                drawGame.DrawCard(drawPlayer);
                drawGame.Nexturn();
                var nextAfterDraw = drawGame.GetCurrentPlayer();
                BroadcastRoomJson(drawRoomId, "gameState", new
                {
                    lastCard = drawGame.GetLastPlayedCard().ToString(),
                    currentPlayer = nextAfterDraw.Name,
                    allPlayers = GetPlayersCardCount(drawGame),
                    lastColor = drawGame.CurrentColor.ToString(),
                    action = "drawcard"
                });
                SendJson(socket, "playerState", new
                {
                    lastCard = drawGame.GetLastPlayedCard().ToString(),
                    player = drawPlayer.Name,
                    hand = drawGame.GetPlayerCards(drawPlayer).Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                });
                break;

            case "play":
                if (parts.Length < 5)
                {
                    SendJson(socket, "error", new { message = "missing roomId or cardIndex" });
                    break;
                }
                var playRoomId = parts[2];
                if (!TryGetGame(socket, playRoomId, requireStarted: true, out var playGame)) break;
                if (!int.TryParse(parts[3], out var cardIndex))
                {
                    SendJson(socket, "error", new { message = "invalid cardIndex" });
                    break;
                }
                var playPlayer = playGame.GetCurrentPlayer();
                if (!IsCurrentPlayer(socket, parts[0], playPlayer)) break;
                var playHand = playGame.GetPlayerCards(playPlayer);
                if (cardIndex < 0 || cardIndex >= playHand.Count)
                {
                    SendJson(socket, "error", new { message = "cardIndex out of range" });
                    break;
                }
                var playedCard = playHand[cardIndex];
                if (!playGame.IsCardValid(playedCard))
                {
                    SendJson(socket, "error", new { message = "invalid card" });
                    break;
                }
                playGame.PlayCard(playPlayer, playedCard);
                if(playedCard.Type == CardType.Wild || playedCard.Type == CardType.WildDrawFour)
                {
                    
                    var colorStr = parts[4];
                    if (!Enum.TryParse<CardColor>(colorStr, true, out var chosenColor))
                    {
                        SendJson(socket, "error", new { message = "invalid color choice" });
                        break;
                    }
                    playGame.SetCurrentColor(chosenColor);
                
                    
                    
                }
                if(playHand.Count == 1 )
                {
                    Thread.Sleep(2000);
                }
                playGame.Nexturn();
                var nextAfterPlay = playGame.GetCurrentPlayer();
                SendJson(socket, "playerState", new
                {
                    lastCard = playGame.GetLastPlayedCard().ToString(),
                    currentPlayer = playPlayer.Name,
                    hand = playGame.GetPlayerCards(playPlayer).Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                });
                BroadcastRoomJson(playRoomId, "gameState", new
                {
                    lastCard = playGame.GetLastPlayedCard().ToString(),
                    currentPlayer = nextAfterPlay.Name,
                    allPlayers = GetPlayersCardCount(playGame),
                    action = $"playcard {playedCard}",
                    lastColor = playGame.CurrentColor.ToString()
                });
                break;

            case "uno":
                if (!TryGetRoomId(socket, parts, 2, out var unoRoomId)) break;
                if (!TryGetGame(socket, unoRoomId, requireStarted: true, out var unoGame)) break;
                var unoPlayer = unoGame.GetCurrentPlayer();
                if (!IsCurrentPlayer(socket, parts[0], unoPlayer)) break;
                unoGame.CallUno(unoPlayer);
                BroadcastRoomJson(unoRoomId, "info", new { message = $"{unoPlayer.Name} called UNO!" });
                BroadcastRoomJson(unoRoomId, "UnoState", new
                {
                    currentPlayer = unoPlayer.Name,
                    UnoCalled = true,
                    action = "called uno"
                });
                break;

            default:
                SendJson(socket, "error", new { message = "Unknown command" });
                break;
        }
    }















bool RequireGamePrefix(IWebSocketConnection socket, string[] parts)
{
    if (parts[0] != "Game")
    {
        SendJson(socket, "error", new { message = "invalid input" });
        return false;
    }
    return true;
}

bool TryGetRoomId(IWebSocketConnection socket, string[] parts, int index, out string roomId)
{
    roomId = "";
    if (parts.Length <= index)
    {
        SendJson(socket, "error", new { message = "missing roomId" });
        return false;
    }
    roomId = parts[index];
    return true;
}

bool TryGetGame(IWebSocketConnection socket, string roomId, bool requireStarted, out GameController game)
{
    game = null!;
    if (!rooms.TryGetValue(roomId, out game))
    {
        SendJson(socket, "error", new { message = "room not found" });
        return false;
    }
    if (requireStarted && !game.IsGameStarted)
    {
        SendJson(socket, "error", new { message = "Game not started" });
        return false;
    }
    return true;
}

bool IsCurrentPlayer(IWebSocketConnection socket, string playerName, IPlayer currentPlayer)
{
    if (!string.Equals(currentPlayer.Name, playerName, StringComparison.OrdinalIgnoreCase))
    {
        SendJson(socket, "error", new { message = "not your turn" });
        return false;
    }
    return true;
}

void CreateRoom(string roomId, int maxPlayers)
{
    var deck = new Deck.Deck();
    InitDeck(deck);
    Shuffle(deck.Cards);
    var discardPile = new DiscardPile();
    var players = new List<IPlayer>();
    for (int i = 1; i <= maxPlayers; i++)
    {
        var playerName = $"Player{i}";
        players.Add(new Player(playerName));
    }
    var game = new GameController(players, deck, discardPile);
    rooms[roomId] = game;
}

void Broadcast(string message)
{
    if (allSockets == null)
    {
        return;
    }
    foreach (var s in allSockets.ToList()) // ToList() biar aman kalau ada disconnect
    {
        if (s == null)
        {
            continue;
        }
        s.Send(message);
    }
}

void SendJson(IWebSocketConnection socket, string type, object data)
{
    var payload = new { type, data };
    socket.Send(JsonSerializer.Serialize(payload));
}

void BroadcastJson(string type, object data)
{
    var payload = new { type, data };
    Broadcast(JsonSerializer.Serialize(payload));
}

void BroadcastRoomJson(string roomId, string type, object data)
{
    var payload = new { type, data };
    Broadcast(JsonSerializer.Serialize(payload));
}
    


List<object> GetPlayersCardCount(GameController game)
{
    // Ambil player counts dari fungsi yang sudah ada
    var playerCounts = game.GetPlayerCardCounts();

    // Buat list object, bukan JSON string
    var players = playerCounts.Select(kvp => new
    {
        name = kvp.Key,
        cardCount = kvp.Value
    }).ToList<object>(); // cast ke object supaya bisa digabung dengan object anonim lain

    return players;
}

   
            
        
   

        // game start and events
       

        // ===== EVENTS =====
        //game.GameStarted += () => Console.WriteLine("Game Started");
        // game.TurnStarted += p => Console.WriteLine($"\nTurn: {p}");
        // game.CardPlayed += (p, c) => Console.WriteLine($"{p} played {c}");
        // game.CardDrawn += (p, c) => Console.WriteLine($"{p} drew {c}");
        // game.GameEnded += p => Console.WriteLine($"\nWINNER: {p}");
        // foreach (var p in players)
        //     for (int i = 0; i < 7; i++)
        //     {
        //         game.DrawCard(p);
        //     }
      
        // game.StartGame();
        
        // Console.WriteLine($"First card: {game.GetLastPlayedCard()}");

        

     void InitDeck(Deck.Deck deck)
    {
        foreach (CardColor color in Enum.GetValues<CardColor>())
        {
            deck.Cards.Push(new Card(CardType.Number0, color));
            deck.Cards.Push(new Card(CardType.Number1, color));
            deck.Cards.Push(new Card(CardType.Number2, color));
            deck.Cards.Push(new Card(CardType.Number3, color));
            deck.Cards.Push(new Card(CardType.Number4, color));
            deck.Cards.Push(new Card(CardType.Number5, color));
            deck.Cards.Push(new Card(CardType.Number6, color));
            deck.Cards.Push(new Card(CardType.Number7, color));
            deck.Cards.Push(new Card(CardType.Number8, color));
            deck.Cards.Push(new Card(CardType.Number9, color));

            deck.Cards.Push(new Card(CardType.Skip, color));
            deck.Cards.Push(new Card(CardType.Reverse, color));
            deck.Cards.Push(new Card(CardType.DrawTwo, color));
        }

        deck.Cards.Push(new Card(CardType.Wild));
        deck.Cards.Push(new Card(CardType.WildDrawFour));
    }

    void Shuffle<T>(Stack<T> stack)
    {
        var list = new List<T>(stack);
        var rnd = new Random();
        stack.Clear();

        while (list.Count > 0)
        {
            int i = rnd.Next(list.Count);
            stack.Push(list[i]);
            list.RemoveAt(i);
        }
    }

    
