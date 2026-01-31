using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Controller;



        
     

        var deck = new Deck.Deck();
        InitDeck(deck);
        Shuffle(deck.Cards);

        // ===== DISCARD =====
        var discardPile = new DiscardPile();

        // ===== PLAYERS =====
        var players = CreatePlayers(3);
            
        
        // ===== DECK =====
        

        // ===== CONTROLLER =====
        var game = new GameController(players, deck, discardPile);

        // ===== EVENTS =====
        game.GameStarted += () => Console.WriteLine("Game Started");
        // game.TurnStarted += p => Console.WriteLine($"\nTurn: {p}");
        // game.CardPlayed += (p, c) => Console.WriteLine($"{p} played {c}");
        // game.CardDrawn += (p, c) => Console.WriteLine($"{p} drew {c}");
        // game.GameEnded += p => Console.WriteLine($"\nWINNER: {p}");

        // ===== DEAL 7 CARDS =====
        foreach (var p in players)
            for (int i = 0; i < 7; i++)
            {
                game.DrawCard(p);
            }
               

        // ===== FIRST CARD =====
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

    List<IPlayer> CreatePlayers(int jumlah)
    {
        var players = new List<IPlayer>();
        for (int i = 1; i <= jumlah; i++)
        {
            players.Add(new Player($"Player {i}"));
        }
        return players;
    }