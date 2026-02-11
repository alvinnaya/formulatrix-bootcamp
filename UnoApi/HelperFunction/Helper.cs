using Players;
using Deck;
using Cards;
using Dto;
using System.Collections.Generic;
using GameControllerNamespace;
using Microsoft.AspNetCore.SignalR;
using Controllers;
using System.Text.Json;
using Serilog;
using Serilog.Formatting.Json;

namespace helperFunction;
static public class Helper
{
static public void CreatePlayers(int jumlah, List<IPlayer> players)
    {
      
        for (int i = 1; i <= jumlah; i++)
        {
            players.Add(new Player($"Player{i}"));
        }
       
    }


static public List<PlayerCardCountDTO> GetPlayerCardCounts(GameController game)
{
    // hitung jumlah kartu player dari fungsi yang sudah ada di game object
    var playerCounts = game.GetPlayerCardCounts();

    // Buat list object, bukan JSON string
    List<PlayerCardCountDTO> players = playerCounts.Select(kvp => new PlayerCardCountDTO
    {
        Name = kvp.Key,
        CardCount = kvp.Value
    }).ToList(); // cast ke object supaya bisa digabung dengan object anonim lain

    return players;
}



static public List<CardDTO> BuildHandState(IReadOnlyList<ICard> hand)
{
    return hand
        .Select((c, idx) => new CardDTO
        {
            Index = idx,
            Card = c.ToString(),
            CardColor = c.Color?.ToString() ?? string.Empty,
            CardType = c.Type.ToString()
        })
        .ToList ();
}


//menambahkan kartu-kartu ke deck
static public void InitDeck(IDeck deck)
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
        Log.Debug(
            "Deck initialized. TotalCards={CardCount}",
            deck.Cards.Count
        );
    }

//mengacak urutan kartu di deck
static public void Shuffle<T>(Stack<T> stack)
        {
            // memindahkan elemen Stack ke List untuk diacak
            List<T> list = new List<T>(stack);
            // membangun ulang Stack dengan urutan acak
            Random rnd = new Random();
            stack.Clear();

            while (list.Count > 0)
            {   
                //mendapatkan index acak dari jumlah elemen di list
                int i = rnd.Next(list.Count);
                //mmemindahkan elemen dari list ke stack
                stack.Push(list[i]);
                //menghapus elemen yang sudah dipindahkan dari list
                list.RemoveAt(i);
            }
            Log.Debug(
                "Deck shuffled. CardCount={CardCount}",
                stack.Count
            );

        }

public static Task BroadcastInfo(object payload, IHubContext<GameHub> hub)
{
     Log.Debug(
        "Broadcast info sent. PayloadType={PayloadType}",
        payload.GetType().Name
    );
    return hub.Clients.All.SendAsync("info", payload);
}


public static Task Broadcast(string message, IHubContext<GameHub> hub)
{
    return hub.Clients.All.SendAsync("message", message);
}



static public void BroadcastJson(string type, object data,IHubContext<GameHub> hub)
{
    Log.Debug(
        "Broadcast JSON. Type={Type}, DataType={DataType}",
        type,
        data.GetType().Name
    );
    
     hub.Clients.All.SendAsync(type, data);
}




static public GameStateDTO BroadcastGameState(GameController game, string action)
{

    var lastCard = game.GetLastPlayedCard();

    GameStateDTO gameState = new GameStateDTO
    {
        LastCard = lastCard?.ToString() ?? "",
        CurrentPlayer = game.GetCurrentPlayer().Name,
        AllPlayers = GetPlayerCardCounts(game),
        Action = action,
        CurrentColor = game.CurrentColor.ToString(),
        GameEnd = game.IsGameOver,
      
        
    };

    

    return gameState;

   
}



}