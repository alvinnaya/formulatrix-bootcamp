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
                
            };

            socket.OnClose = () =>
            {
                Console.WriteLine("Client disconnected");
               
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

        switch (parts[0].ToLower())
        {
            case "init":
                // contoh: "init 3" -> buat 3 pemain
                Broadcast("Server got init: ");
            
                break;

            case "start":{

                foreach (var p in players)
                for (int i = 0; i < 7; i++)
                {
                    game.DrawCard(p);
                }
            
                game.StartGame();

                var player = game.GetCurrentPlayer();
                var hand = game.GetPlayerCards(player);
                var lastCard = game.GetLastPlayedCard();

                // Buat object JSON
                var state = new
                {
                    lastCard = lastCard.ToString(),
                    currentPlayer = player.Name,
                    hand = hand.Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                };

                // Serialize object ke JSON
                string json = JsonSerializer.Serialize(state);

                // Kirim ke client lewat socket
                Broadcast(json);

               
                break;
            }

            case "getcard":

                //String playerIndex = parts[1];
                Broadcast($"player Index: {parts[1]} " );


                break;

            case "draw":
            {
                         // draw <playerIndex>
                    var player = game.GetCurrentPlayer();

                    if(player.Name == parts[1])
                    {
                        DrawCard(player);
                        var hand = game.GetPlayerCards(player);
                        var lastCard = game.GetLastPlayedCard();

                // Buat object JSON
                var state = new
                {
                    lastCard = lastCard.ToString(),
                    currentPlayer = player.Name,
                    hand = hand.Select((c, idx) => new { index = idx, card = c.ToString() }).ToList()
                };

                // Serialize object ke JSON
                string json = JsonSerializer.Serialize(state);

                // Kirim ke client lewat socket
                Broadcast(json);
                    }


            }
                
           
                    

                   
                
                break;

            case "play":
                
                    // play <playerIndex> <cardIndex>
                  socket.Send($"play: " );
                    
                
                break;

            case "uno":
                
                    // uno <playerIndex>
                    socket.Send($"uno " );
                   
                
                break;

            default:
                socket.Send("Unknown command");
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

    


        game.AddPlayer(new Player($"Player4"));


        //add player

        

        



   

   
            
        
   

        // game start and events
       

        // ===== EVENTS =====
        game.GameStarted += () => Console.WriteLine("Game Started");
        // game.TurnStarted += p => Console.WriteLine($"\nTurn: {p}");
        // game.CardPlayed += (p, c) => Console.WriteLine($"{p} played {c}");
        // game.CardDrawn += (p, c) => Console.WriteLine($"{p} drew {c}");
        // game.GameEnded += p => Console.WriteLine($"\nWINNER: {p}");
        foreach (var p in players)
            for (int i = 0; i < 7; i++)
            {
                game.DrawCard(p);
            }
      
        game.StartGame();
        
        Console.WriteLine($"First card: {game.GetLastPlayedCard()}");

        

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




