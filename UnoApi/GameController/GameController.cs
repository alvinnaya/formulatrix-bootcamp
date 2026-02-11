using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Serilog;
using Serilog.Formatting.Json;
using System.Security.Cryptography.X509Certificates;

namespace GameControllerNamespace;


 public class GameController
    {
        // ===== STATE =====
        public List<IPlayer>? Players { get; private set; }

        
        
        public IDeck Deck { get; private set; }
        public IDiscardPile DiscardPile { get; private set; }

        private Dictionary<IPlayer, List<ICard>> _playerCards;
        private ICard? _lastPlayedCard;
        private IPlayer? _lastPlayer;
        private IPlayer? _currentPlayer;
        private ILogger<GameController> _log;
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
        public bool IsGameStarted { get; private set; }

        // ===== CONSTRUCTOR =====
        //constructor dengan parameter list player, deck, dan discard pile.
        //sementara List player boleh kosong (null)
        public GameController( IDeck deck, IDiscardPile discardPile, ILogger<GameController> logger )
        {
           
            Players = new List<IPlayer>();
            Deck = deck;
            DiscardPile = discardPile;
            _log = logger;




             _playerCards = new Dictionary<IPlayer, List<ICard>>();
            if (Players != null)
            {
                foreach (IPlayer p in Players)
                    _playerCards[p] = new List<ICard>();
            }

            InitDeck(Deck);
        Shuffle(Deck.Cards);
           

            _currentPlayer = Players != null && Players.Count > 0 ? Players[0] : null;
            Direction = Direction.Clockwise;
            IsGameStarted = false;

            _log.LogInformation(
                "GameController created with {PlayerCount} players",
                Players?.Count ?? 0
            );

            
        }

        //mengganti daftar pemain dalam permainan dengan daftar pemain baru yang diberikan sebagai parameter.
        // sekaligus menginisialisasi dictionary untuk menyimpan kartu masing-masing pemain.
        public void ChangePlayers(List<IPlayer> players)
    {
        Players = players;
         _playerCards = new Dictionary<IPlayer, List<ICard>>();
        foreach (IPlayer p in Players)
            _playerCards[p] = new List<ICard>();

        _currentPlayer = players.Count > 1 ? players[0] : null;

        _log.LogInformation(
            "Players changed. New player count: {PlayerCount}",
            players.Count
        );
        _log.LogInformation(
            "Players changed to {@Players}",
            players.Select(p => p.Name)
        );
    }

 private void InitDeck(IDeck deck)
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
        _log.LogInformation(
            "Deck initialized. TotalCards={CardCount}",
            deck.Cards.Count
        );
    }

//mengacak urutan kartu di deck
 private void Shuffle<T>(Stack<T> stack)
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
            _log.LogInformation(
                "Deck shuffled. CardCount={CardCount}",
                stack.Count
            );

        }

    //membuat dictionary yang berisi nama pemain sebagai key
    // dan jumlah kartu yang dimiliki pemain tersebut sebagai value.
    public Dictionary<string, int> GetPlayerCardCounts()
{
    Dictionary<string, int> result = new Dictionary<string, int>();

    foreach (var kvp in _playerCards)
    {
         IPlayer player = kvp.Key;
         List<ICard> cards = kvp.Value;

        // Nama pemain sebagai key, jumlah kartu sebagai value
        result[player.Name] = cards.Count;
    }

    return result;
}

// memulai permainan dengan mengatur pemain pertama, kartu pertama di tumpukan buangan,
// dan menentukan warna saat ini berdasarkan kartu pertama.
    public void StartGame()
    {
        if (Players == null || Players.Count == 0)
            return;
            

         foreach (var p in Players)
                for (int i = 0; i < 7; i++)
                    DrawCard(p);


        GameStarted?.Invoke();
        _currentPlayer = Players[0];
        ICard first = Deck.Cards.Pop();
        DiscardPile.Cards.Push(first);
        _lastPlayedCard = first;
       if (first.Color.HasValue)
        {
            CurrentColor = first.Color.Value;
        }
        else
        {
            // Wild pertama: pilih warna default/random
            CardColor[] colors = Enum.GetValues<CardColor>();
            CurrentColor = colors[new Random().Next(colors.Length)];
        }
        IsGameStarted = true;

        _log.LogInformation(
            "Game started. FirstPlayer={PlayerName}, FirstCard={CardType}, Color={Color}",
            _currentPlayer.Name,
            first.Type,
            CurrentColor
        );
   
    }

//fungsi untuk menandai bahwa seorang pemain telah memanggil "UNO".
    public void CallUno(IPlayer player)
    {
        if (_playerCards[player].Count == 1)
        {
            player.HasCalledUno = true;
            _log.LogInformation(
            "UNO called by {Player}",
            player.Name
        );
            UnoCalled?.Invoke(player);
        }
    }

//fungsi untuk mengatur giliran berikutnya dalam permainan.
//ia akan mengecek apakah pemain mendpatkan hukuman uno atau tidak ketika kartunya sisa satu
    public void Nexturn()
    {
       
        if (IsGameOver) return;
        if (_currentPlayer == null)
        {
            return;
        }
         _log.LogInformation(
            "Turn started for {Player}",
            _currentPlayer.Name
        );
        //memanggil event TurnStarted sebelum giliran dimulai
        TurnStarted?.Invoke(_currentPlayer);
        CheckUno(_currentPlayer);

        // Placeholder: player _logic
        // PlayCard(_currentPlayer, someCard) or DrawCard(_currentPlayer);
        

        
        MoveToNextPlayer(1);
        //memanggil event TurnEnded setelah giliran berakhir
        TurnEnded?.Invoke(_currentPlayer);
        // if (!IsGameOver)
        //     PlayTurn();
        _log.LogInformation(
            "Turn ended. NextPlayer={NextPlayer}",
            _currentPlayer.Name
        );

    }

//fungsi untuk memainkan kartu oleh pemain tertentu.
    public void PlayCard(IPlayer player, ICard card)
    {
      //mengecek apakah kartu yang dimainkan bisa dimainkan berdasarkan kartu terakhir atau tidak
        if (IsCardValid(card))
        {
//jika bisa maka kartu tersebut dihapus dari tangan pemain
//kemudian kartu tersebut ditambahkan ke tumpukan buangan
//dan memperbarui kartu terakhir yang dimainkan serta pemain terakhir yang memainkan kartu tersebut
            _playerCards[player].Remove(card);
            _lastPlayedCard = card;
            _lastPlayer = player;
            DiscardPile.Cards.Push(card);

            //memanggil event CardPlayed setelah kartu dimainkan
            CardPlayed?.Invoke(player, card);
//setelah memainkan kartu, status HasCalledUno pemain direset menjadi false
            player.HasCalledUno = false;
//jika pemain kehabisan kartu setelah memainkan kartu, maka permainan berakhir
            _log.LogInformation(
                "Player {Player} plays card {CardType} {CardColor}",
                player.Name,
                card.Type,
                card.Color
            );
            if (_playerCards[player].Count == 0)
            {
                //memanggil fungsi EndGame untuk mengakhiri permainan
            EndGame(player);
            }


//memanggil fungsi untuk menyelesaikan efek dari kartu yang dimainkan
            ResolveCardEffect(card);


        }
        else
        {
             _log.LogWarning(
                "Invalid card play. Player={Player}, Card={CardType}, CurrentColor={CurrentColor}, LastCard={LastCard}",
                player.Name,
                card.Type,
                CurrentColor,
                _lastPlayedCard?.Type
            );
        }

        
        
    }
    public IPlayer? GetPlayerByName(string name)
    {
        if (Players == null) return null;
        return Players.FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }
    public void DrawCard(IPlayer player)
    {
        ICard card = DrawFromDeck();
        _playerCards[player].Add(card);
        CardDrawn?.Invoke(player, card);
        player.HasCalledUno = false;
        
        _log.LogInformation(
                "Player {Player} draws card {CardType}",
                player.Name,
                card.Type
            );

      
    }

    public ICard? GetLastPlayedCard() => _lastPlayedCard;
    public IPlayer? GetLastPlayer() => _lastPlayer;
    public IPlayer GetCurrentPlayer()
    {
        if (_currentPlayer == null)
            throw new InvalidOperationException("Players not initialized.");
        return _currentPlayer;
    }

    public IReadOnlyList<ICard> GetPlayerCards(IPlayer player) => _playerCards[player].AsReadOnly();


    public bool IsCardValid(ICard card)
    {
        // filter pengaman dengan mengecek kartu terakhir yang dimainkan
         if (_lastPlayedCard == null)
            {
                _log.LogWarning(
                    "IsCardValid called before any card was played. Card: {CardType}",
                    card.Type
                );
                return false;
            }
        // Wild selalu boleh
        if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
            return true;

        // Warna sama
        if (card.Color == CurrentColor)
            return true;

        // Angka / simbol sama
        if (card.Type == _lastPlayedCard.Type &&
            card.Type == _lastPlayedCard.Type)
            return true;

        return false;
    }

    public void SetCurrentColor(CardColor color)
    {
        
        CurrentColor = color;
        CurrentColorChanged?.Invoke(CurrentColor);
    }


    private void MoveToNextPlayer(int skip)
    {
        if (Players == null || _currentPlayer == null)
            throw new InvalidOperationException("Players not initialized.");

        int index = Players.IndexOf(_currentPlayer);
        int count = Players.Count;

        if (Direction == Direction.Clockwise)
            index = (index + skip) % count;
        else
            index = (index - skip + count) % count;

        _currentPlayer = Players[index];
    }

    private  IPlayer GetNextPlayer(int skip)
    {
        if (Players == null || _currentPlayer == null)
            throw new InvalidOperationException("Players not initialized.");

         int index = Players.IndexOf(_currentPlayer);
        int count = Players.Count;

        if (Direction == Direction.Clockwise)
            index = (index + skip) % count;
        else
            index = (index - skip + count) % count;

        return Players[index];
    }


    private void ReverseDirection()
{
    Direction = Direction == Direction.Clockwise
        ? Direction.CounterClockwise
        : Direction.Clockwise;

    DirectionChanged?.Invoke(Direction);
}

//fungsi untuk menyelesaikan efek dari kartu yang dimainkan.
private void ResolveCardEffect(ICard card)
{
    // Update warna kartu non wild ke currentColor
    if (card.Color.HasValue)
    {
        CurrentColor = card.Color.Value;
        CurrentColorChanged?.Invoke(CurrentColor);
    }

//switch case untuk menentukan efek berdasarkan tipe kartu yang dimainkan.
    switch (card.Type)
    {
        //untuk tipe kartu skip maka akan melewati giliran satu pemain berikutnya
        case CardType.Skip:
            // skip 1 player
            MoveToNextPlayer(1);
            break;
        //untuk tipe kartu reverse maka akan membalik arah giliran permainan
        case CardType.Reverse:
            ReverseDirection();
            _log.LogInformation(
                "Direction reversed. NewDirection={Direction}",
                Direction
            );

            // aturan UNO: 2 player = skip
            if (Players != null && Players.Count == 2)
                MoveToNextPlayer(1);
            break;
        //untuk tipe kartu draw two maka pemain berikutnya harus mengambil dua kartu dari deck
        case CardType.DrawTwo:
            // pindah ke player target

            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            _log.LogInformation(
                "Player {TargetPlayer} draws {DrawCount} cards due to {CardType}",
                GetNextPlayer(1).Name,
                2,
                card.Type
            );

            break;
        //untuk tipe kartu wild draw four maka pemain berikutnya harus mengambil empat kartu dari deck
        case CardType.WildDrawFour:

            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));

            _log.LogInformation(
                "Player {TargetPlayer} draws {DrawCount} cards due to {CardType}",
                GetNextPlayer(1).Name,
                2,
                card.Type
            );

       
            break;

        //untuk tipe kartu wild dan tipe lainnya tidak memiliki efek tambahan 
        //karena efek sudah ditentukan di bagian request untuk merubah warna
        case CardType.Wild:

        default:
            // number card & wild tanpa efek tambahan
            break;
    }
}


//fungsi untuk mengambil kartu dari deck.
//jika deck kosong, maka deck akan diisi ulang dari Discard Pile terlebih dahulu.
private ICard DrawFromDeck()
    {

        if(Deck.Cards.Count == 0)
        {
            RefillDeckFromDiscard();
            
        }
        
        ICard card = Deck.Cards.Pop();
        return card;
    }

//fungsi untuk mengisi ulang deck dari tumpukan buangan.
// dengan memindahkan semua kartu dari tumpukan buangan ke deck. lewat push
private void RefillDeckFromDiscard()
    {
        Stack<ICard> cards = DiscardPile.Cards;
        foreach (ICard n in cards)
            {
                Deck.Cards.Push(n);
            }
        DiscardPile.Cards.Clear();
    }


//fungsi untuk memeriksa apakah pemain bisa mendapatkan hukuman UNO atau tidak.
private void CheckUno(IPlayer player)
{
    // Jika player sebelumnya punya 1 kartu dan TIDAK bilang UNO
    if (_lastPlayer != null &&
        _playerCards[_lastPlayer].Count == 1 &&
        !_lastPlayer.HasCalledUno)
    {
        ApplyUnoPenalty(_lastPlayer);
        _log.LogWarning(
            "UNO penalty applied to {Player}",
            player.Name
        );

    }
}


//fungsi untuk menerapkan hukuman UNO kepada pemain tertentu.
private void ApplyUnoPenalty(IPlayer player)
    {
        // Draw 2 cards as penalty
        DrawCard(player);
        DrawCard(player);
        UnoPenaltyApplied?.Invoke(player);
    }

//fungsi untuk mengakhiri permainan dengan menentukan pemenang. dan membuat IsGameOver menjadi true.
private void EndGame(IPlayer winner)
    {
        IsGameOver = true;
        _log.LogInformation(
            "Game ended. Winner={Winner}",
            winner.Name
        );

        //memanggil event GameEnded dengan pemain pemenang sebagai parameter
        GameEnded?.Invoke(winner);
    }


//fungsi untuk mereset status permainan ke kondisi awal.
// sehingga permainan dapat dimulai ulang dengan deck dan discard pile yang baru.
// seolah-olah game controller baru dibuat.
public void ResetGame(IDeck deck, IDiscardPile discardPile)
    {

        if(Deck == deck || DiscardPile == discardPile)
        {
            return;
        }
        Deck = deck;
        DiscardPile = discardPile;

        InitDeck(Deck);
        Shuffle(Deck.Cards);

        _log.LogInformation("Game reset");


        Players = null;
        _playerCards = new Dictionary<IPlayer, List<ICard>>();
        _lastPlayedCard = null;
        _lastPlayer = null;
        _currentPlayer = null;

        Direction = Direction.Clockwise;
        CurrentColor = default;
        IsGameOver = false;
        IsGameStarted = false;
    }


      
    }
