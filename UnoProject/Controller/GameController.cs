using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;

namespace Controller;


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
        public GameController(List<IPlayer>? players, IDeck deck, IDiscardPile discardPile)
        {
            Players = players;
            Deck = deck;
            DiscardPile = discardPile;

            _playerCards = new Dictionary<IPlayer, List<ICard>>();
            if (players != null)
            {
                foreach (IPlayer p in players)
                    _playerCards[p] = new List<ICard>();
            }

            _currentPlayer = players != null && players.Count > 0 ? players[0] : null;
            Direction = Direction.Clockwise;
            IsGameStarted = false;
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
   
    }

//fungsi untuk menandai bahwa seorang pemain telah memanggil "UNO".
    public void CallUno(IPlayer player)
    {
        if (_playerCards[player].Count == 1)
        {
            player.HasCalledUno = true;
            UnoCalled?.Invoke(player);
        }
    }

//fungsi untuk mengatur giliran berikutnya dalam permainan.
//ia akan mengecek apakah pemain mendpatkan hukuman uno atau tidak ketika kartunya sisa satu
    public void Nexturn()
    {
        if (IsGameOver) return;
        if (_currentPlayer == null) return;
        //memanggil event TurnStarted sebelum giliran dimulai
        TurnStarted?.Invoke(_currentPlayer);
        CheckUno(_currentPlayer);

        // Placeholder: player logic
        // PlayCard(_currentPlayer, someCard) or DrawCard(_currentPlayer);
        

        
        MoveToNextPlayer(1);
        //memanggil event TurnEnded setelah giliran berakhir
        TurnEnded?.Invoke(_currentPlayer);
        // if (!IsGameOver)
        //     PlayTurn();
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
            if (_playerCards[player].Count == 0)
            {
                //memanggil fungsi EndGame untuk mengakhiri permainan
            EndGame(player);
            }


//memanggil fungsi untuk menyelesaikan efek dari kartu yang dimainkan
            ResolveCardEffect(card);
          
            
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
            return true;

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

            // aturan UNO: 2 player = skip
            if (Players != null && Players.Count == 2)
                MoveToNextPlayer(1);
            break;
        //untuk tipe kartu draw two maka pemain berikutnya harus mengambil dua kartu dari deck
        case CardType.DrawTwo:
            // pindah ke player target

            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            break;
        //untuk tipe kartu wild draw four maka pemain berikutnya harus mengambil empat kartu dari deck
        case CardType.WildDrawFour:

            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
            DrawCard(GetNextPlayer(1));
       
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
        //memanggil event GameEnded dengan pemain pemenang sebagai parameter
        GameEnded?.Invoke(winner);
    }


//fungsi untuk mereset status permainan ke kondisi awal.
// sehingga permainan dapat dimulai ulang dengan deck dan discard pile yang baru.
// seolah-olah game controller baru dibuat.
    public void ResetGame(IDeck deck, IDiscardPile discardPile)
    {
        Deck = deck;
        DiscardPile = discardPile;

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
