using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;

namespace Controller;


 public class GameController
    {
        // ===== STATE =====
        public List<IPlayer> Players { get; private set; }
        public IDeck Deck { get; private set; }
        public IDiscardPile DiscardPile { get; private set; }

        private Dictionary<IPlayer, List<ICard>> _playerCards;
        private ICard _lastPlayedCard;
        private IPlayer _lastPlayer;
        private IPlayer _currentPlayer;

        public Direction Direction { get; private set; }
        public CardColor CurrentColor { get; private set; }
        public bool IsGameOver { get; private set; }

        // ===== EVENTS =====
        public event Action GameStarted;
        public event Action<IPlayer> TurnStarted;
        public event Action<IPlayer> TurnEnded;
        public event Action<IPlayer, ICard> CardPlayed;
        public event Action<IPlayer, ICard> CardDrawn;
        public event Action<CardColor> CurrentColorChanged;
        public event Action<Direction> DirectionChanged;
        public event Action<IPlayer> UnoCalled;
        public event Action<IPlayer> UnoPenaltyApplied;
        public event Action<IPlayer> GameEnded;

        // ===== CONSTRUCTOR =====
        public GameController(List<IPlayer> players, IDeck deck, IDiscardPile discardPile)
        {
            Players = players;
            Deck = deck;
            DiscardPile = discardPile;

            _playerCards = new Dictionary<IPlayer, List<ICard>>();
            foreach (var p in players)
                _playerCards[p] = new List<ICard>();

            _currentPlayer = players[0];
            Direction = Direction.Clockwise;
        }


         public void StartGame()
    {
        GameStarted?.Invoke();
        _currentPlayer = Players[0];
        PlayTurn();
    }



   public void PlayTurn()
    {
        if (IsGameOver) return;

        TurnStarted?.Invoke(_currentPlayer);

        // Placeholder: player logic
        // PlayCard(_currentPlayer, someCard) or DrawCard(_currentPlayer);

        TurnEnded?.Invoke(_currentPlayer);
        MoveToNextPlayer(1);
    }




    





        public void PlayCard(IPlayer player, ICard card)
    {
        _playerCards[player].Remove(card);
        _lastPlayedCard = card;
        _lastPlayer = player;
        DiscardPile.Cards.Push(card);
        CardPlayed?.Invoke(player, card);

        ResolveCardEffect(card);
        CheckUno(player);

        if (_playerCards[player].Count == 0)
            EndGame(player);
    }



     public void DrawCard(IPlayer player)
    {
        var card = DrawFromDeck();
        _playerCards[player].Add(card);
        CardDrawn?.Invoke(player, card);
    }

    public ICard GetLastPlayedCard() => _lastPlayedCard;
    public IPlayer GetLastPlayer() => _lastPlayer;
    public IPlayer GetCurrentPlayer() => _currentPlayer;

    public IReadOnlyList<ICard> GetPlayerCards(IPlayer player) => _playerCards[player].AsReadOnly();


     private void ResolveCardEffect(ICard card)
    {
        // Implement special card effects here (Skip, Reverse, Draw Two, Wild, etc.)
    }


      private void MoveToNextPlayer(int skip)
    {
        int index = Players.IndexOf(_currentPlayer);
        int count = Players.Count;

        if (Direction == Direction.Clockwise)
            index = (index + skip) % count;
        else
            index = (index - skip + count) % count;

        _currentPlayer = Players[index];
    }

    private void ReverseDirection()
    {
        Direction = Direction == Direction.Clockwise ? Direction.CounterClockwise : Direction.Clockwise;
        DirectionChanged?.Invoke(Direction);
    }

    





    private ICard DrawFromDeck()
    {

        if(Deck.Cards.Count == 0)
        {
            
        }
        
        var card = Deck.Cards.Pop();
        return card;
    }


     private void RefillDeckFromDiscard()
    {
        var cards = DiscardPile.Cards;
        foreach (var n in cards)
            {
                Deck.Cards.Push(n);
            }
    }


     private void CheckUno(IPlayer player)
    {
        if (_playerCards[player].Count == 1)
        {
            UnoCalled?.Invoke(player);
        }
        else if (_playerCards[player].Count == 0)
        {
            // player won
        }
    }


      private void ApplyUnoPenalty(IPlayer player)
    {
        // Draw 2 cards as penalty
        DrawCard(player);
        DrawCard(player);
        UnoPenaltyApplied?.Invoke(player);
    }


    private void EndGame(IPlayer winner)
    {
        IsGameOver = true;
        GameEnded?.Invoke(winner);
    }



      
    }