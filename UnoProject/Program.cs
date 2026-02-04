using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Controller;
using Fleck;
using System.Text.Json;







        
     

        var deck = new Deck.Deck();
        InitDeck(deck);
        Shuffle(deck.Cards);

       
        var discardPile = new DiscardPile();

        



    //initialize game

        var players = new List<IPlayer>();
        CreatePlayers(2,players);
        var game = new GameController(players, deck, discardPile);
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
        var parts = message.Split(" ");



        switch (parts[1].ToLower())
        {
            case "init":
            {
                 Broadcast("Server got init: ");
            
                break;
            }
                // contoh: "init 3" -> buat 3 pemain
               
            case "createplayer":
            {


                if(parts[0] != "Game" )
                {
                    SendJson(socket, "error", new { message = "invalid input" });
                    break;
                }

                 if (parts.Length < 3)
                {
                    SendJson(socket, "error", new { message = "missing number of players" });
                    break;
                }

                  if(game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game already started" });
                    break;
                }

                

                if (int.TryParse(parts[2], out int numOfPlayers))
                {
                    var newPlayerList =  new List<IPlayer>();
                    CreatePlayers(numOfPlayers, newPlayerList);
                    Console.WriteLine($"Created {numOfPlayers} players.");
                    Console.WriteLine(newPlayerList.Count);
                    newPlayerList.ForEach(p => Console.WriteLine(p.Name));
                    game.ChangePlayers(newPlayerList);
                    
                    SendJson(socket, "info", new { message = $"there was {numOfPlayers} Players" });
                }
                break;

                
            }

            case "start":{


                if(game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game already started" });
                    break;
                }

                if(parts[0] == "Game")
                {
                
                    //this is the main problem
                foreach (var p in game.Players)
                for (int i = 0; i < 7; i++)
                {
                    game.DrawCard(p);
                }
            
                game.StartGame();

                var currentPlayer = game.GetCurrentPlayer();
                //var hand = game.GetPlayerCards(player);
                var lastCard = game.GetLastPlayedCard();

                   var gameState = new
                {
                    lastCard = lastCard.ToString(),
                    currentPlayer = game.GetCurrentPlayer().Name,
                    allPlayers = GetPlayersCardCount(game)
                };

                    BroadcastJson("gameState", gameState);


               
             
                }
                   break;
            }

            case "getcurrentstate":
            {
                 if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }
                var lastCard = game.GetLastPlayedCard();

                 var gameState = new
                {
                    lastCard = lastCard.ToString(),
                    currentPlayer = game.GetCurrentPlayer().Name,
                    allPlayers = GetPlayersCardCount(game)
                };

                SendJson(socket, "gameState", gameState);
                break;
                
            }

            case "getcard":
            {
                if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }
                var player = game.GetPlayerByName(parts[0]);
                if(player == null)
                {
                    SendJson(socket, "error", new { message = "Player not found" });
                    break;
                }
                var hand = game.GetPlayerCards(player);
                var lastCard = game.GetLastPlayedCard();

                // Buat object JSON
                var state = new
                {
                    lastCard = lastCard.ToString(),
                    player = player.Name,
                    hand = hand.Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                };

                SendJson(socket, "playerState", state);
            

                break;
            }

             

            case "draw":
            {
                    if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }


                         // draw <playerIndex>
                    var currentPlayer = game.GetCurrentPlayer();

                    if(currentPlayer.Name == parts[0])
                    {
                        game.DrawCard(currentPlayer);
                        game.Nexturn();
                        var hand = game.GetPlayerCards(currentPlayer);
                        var lastCard = game.GetLastPlayedCard();
                        var nextPlayer = game.GetCurrentPlayer();

                

                    var gameState = new
                        {
                            lastCard = game.GetLastPlayedCard().ToString(),
                            currentPlayer = nextPlayer.Name,
                            allPlayers = GetPlayersCardCount(game),
                            action = $"drawcard"
                        };

                     var playerState = new
                    {
                        lastCard = lastCard.ToString(),
                        player = currentPlayer.Name,
                        hand = game.GetPlayerCards(currentPlayer).Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                    };

                    // Kirim ke client
                    BroadcastJson("gameState", gameState);
                    SendJson(socket, "playerState", playerState);

             
                
                    }


            }
                
           
                    

                   
                
                break;

            case "play":
            {
                    if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }
                // play <playerIndex> <cardIndex>
                    var currentPlayer = game.GetCurrentPlayer();
                    var hand = game.GetPlayerCards(currentPlayer);
                    var idx = int.Parse(parts[2]);
                    
                    var lastCard = game.GetLastPlayedCard();

                    if(currentPlayer.Name == parts[0])
                    {
                         var card = hand[idx];
                    if (game.IsCardValid(card))
                    {
                        game.PlayCard(currentPlayer, card);
                        if(card.Type == CardType.Wild || card.Type == CardType.WildDrawFour )
                        {
                            
                        }

                        
                        game.Nexturn();
                        var nextPlayer = game.GetCurrentPlayer();
                         var playerState = new
                        {
                            lastCard = game.GetLastPlayedCard().ToString(),
                            currentPlayer = currentPlayer.Name,
                            hand = game.GetPlayerCards(currentPlayer).Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                        };

                        var gameState = new
                        {
                            lastCard = game.GetLastPlayedCard().ToString(),
                            currentPlayer = nextPlayer.Name,
                            allPlayers = GetPlayersCardCount(game),
                            action = $"playcard {card.ToString()}"
                        };


                SendJson(socket, "playerState", playerState);
                BroadcastJson("gameState", gameState);
                            }
                    else
                    {
                        SendJson(socket, "error", new { message = "invalid card" });
                    }
                    }
                  
                    
                
                break;

                
            }
                    
            case "uno":
            {

                if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }

                    var currentPlayer = game.GetCurrentPlayer();

                    if(currentPlayer.Name == parts[0])
            {
                        game.CallUno(currentPlayer);
                        BroadcastJson("info", new { message = $"{currentPlayer.Name} called UNO!" } );
                         var gameState = new
                        {
                            
                            currentPlayer = currentPlayer.Name,
                            UnoCalled = true,
                            action = $"called uno"
                            
                        };
                        BroadcastJson("UnoState", gameState);
            }
                    
                    // uno <playerIndex>
                   
                   
                
                break;
                
            }

              

            default:
                SendJson(socket, "error", new { message = "Unknown command" });
                break;
        }
    }




void Broadcast(string message)
{
    foreach (var s in allSockets.ToList()) // ToList() biar aman kalau ada disconnect
    {
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
    


        // game.AddPlayer(new Player($"Player4"));


        //add player

        

        


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

        

        // ===== GAME LOOP =====
        while (!game.IsGameOver)
        {

            var player = game.GetCurrentPlayer();
            var hand = game.GetPlayerCards(player);
            Console.WriteLine($"Last card: {game.GetLastPlayedCard()}");
            Console.WriteLine($"\n{player}'s turn");

            Console.WriteLine("\nHand:");
            for (int i = 0; i < hand.Count; i++)
                Console.WriteLine($"{i}. {hand[i]}");

            Console.Write("Index kartu / d (draw): ");
            var input = Console.ReadLine();

            if (input == "d")
            {
                DrawCard(player);
             
                
            }

            if (int.TryParse(input, out int idx) &&
                idx >= 0 && idx < hand.Count)
            {
                var card = hand[idx];
                if (game.IsCardValid(card))
                {
                    game.PlayCard(player, card);

                    if (hand.Count == 1)
                    {
                        Console.Write("UNO? (y/n): ");
                        if (Console.ReadLine() == "y")
                            game.CallUno(player);
                    }
                    game.Nexturn();
                }
                else
                {
                    Console.WriteLine("Invalid card");
                }
            }
        }
    

    // ===== HELPERS =====

    void DrawCard(IPlayer player)
{
        game.DrawCard(player);
        game.Nexturn();
}





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

   void CreatePlayers(int jumlah, List<IPlayer> players)
    {
      
        for (int i = 1; i <= jumlah; i++)
        {
            players.Add(new Player($"Player{i}"));
        }
       
    }




