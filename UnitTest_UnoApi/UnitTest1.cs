using Cards;
using Deck;
using GameControllerNamespace;
using Players;
using helperFunction;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace UnitTest_UnoApi;



public class Tests
{
    private GameController _game;
    private IDeck _deck;
    private IDiscardPile _discardPile;
    private List<IPlayer> _playerList;

    [SetUp]
    public void Setup()
    {
        // _deck = new Deck.Deck();
        // _discardPile = new DiscardPile();
        // _playerList = new List<IPlayer>();
        // Helper.InitDeck(_deck);
        // _game = new GameController( _playerList ,_deck, _discardPile);
    var deck = new Deck.Deck();
    var discardPile = new DiscardPile();

    var loggerMock = new Mock<ILogger<GameController>>();
    // Buat GameController manual
     _game = new GameController(deck, discardPile, loggerMock.Object);
    
    }

//ChangePlayers
    [Test]
    public void ChangePlayers_MoreThanOnePlayer_SetCurrentPlayerToFirst()
{
    // Arrange
    var players = new List<IPlayer>
    {
        new Player($"Player1"),
        new Player($"Player2")
    };

    // Act
    _game.ChangePlayers(players);

    // Assert
    Assert.That(_game.Players, Is.EqualTo(players));
    Assert.That(_game.GetCurrentPlayer().Name, Is.EqualTo("Player1"));
}

   [Test]
    public void ChangePlayers_EmptyPlayerList_CurrentPlayerIsNull()
{
    // Arrange
    var players = new List<IPlayer>();

    // Act
    _game.ChangePlayers(players);

    // Assert
    Assert.That(_game.Players.Count, Is.EqualTo(0));
    Assert.That(
        () => _game.GetCurrentPlayer(),
        Throws.InvalidOperationException
    );
}

//GetPlayerCardCounts      
    [Test]
    public void GetPlayerCardCounts_MultiplePlayers_ReturnCorrectCounts()
    {
        // Arrange
        var player1 = new Player("Player1");
        var player2 = new Player("Player2");
        

        var players = new List<IPlayer> { player1, player2 };
        
        _game.ChangePlayers(players);

        _game.DrawCard(player1);
        _game.DrawCard(player1);
        _game.DrawCard(player2);

        // Act
        var result = _game.GetPlayerCardCounts();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result["Player1"], Is.EqualTo(2));
        Assert.That(result["Player2"], Is.EqualTo(1));
    }

    [Test]
    public void GetPlayerCardCounts_NoPlayers_ReturnEmptyDictionary()
    {
        // Arrange
    
        _game.ChangePlayers(new List<IPlayer>());

        // Act
        var result = _game.GetPlayerCardCounts();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }

//StartGame
    [Test]
    public void StartGame_WithPlayers_SetIsGameStartedTrue()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1"),
            new Player("Player2")
        };
        _game.ChangePlayers(players);

        // Act
        _game.StartGame();

        // Assert
        Assert.That(_game.IsGameStarted, Is.True);
    }

    [Test]
    public void StartGame_NoPlayers_GameNotStarted()
    {
        // Arrange
        _game.ChangePlayers(new List<IPlayer>());

        // Act
        _game.StartGame();

        // Assert
        Assert.That(_game.IsGameStarted, Is.False);
    }

//callUno
    [Test]
    public void CallUno_PlayerHasOneCard_SetHasCalledUnoTrue()
    {
        // Arrange
        var player1 = new Player("Player1");
        _game.ChangePlayers(new List<IPlayer> { player1 });

        // Player ambil 1 kartu
        _game.DrawCard(player1);

        // Act
        _game.CallUno(player1);

        // Assert
        Assert.That(player1.HasCalledUno, Is.True);
    }

   [Test]
    public void CallUno_PlayerHasMoreThanOneCard_HasCalledUnoRemainsFalse()
    {
        // Arrange
        var player1 = new Player("Player1");
        _game.ChangePlayers(new List<IPlayer> { player1 });

        _game.DrawCard(player1);
        _game.DrawCard(player1);

        // Act
        _game.CallUno(player1);

        // Assert
        Assert.That(player1.HasCalledUno, Is.False);
    }
   
//NextTurn
    [Test]
    public void Nexturn_GameRunning_MoveToNextPlayer()
    {
        // Arrange
        var player1 = new Player("Player1");
        var player2 = new Player("Player2");

        _game.ChangePlayers(new List<IPlayer> { player1, player2 });
        _game.StartGame();

        var currentPlayer = _game.GetCurrentPlayer();

        // Act
        _game.Nexturn();

        // Assert
        Assert.That(_game.GetCurrentPlayer(), Is.Not.EqualTo(currentPlayer));
    }

    [Test]
    public void Nexturn_CurrentPlayerNull_DoesNotThrow()
    {
        // Arrange
        _game.ResetGame(_game.Deck, _game.DiscardPile);

        // Act & Assert
        Assert.That(() => _game.Nexturn(), Throws.Nothing);
    }

//PlayCard
   [Test]
public void PlayCard_ValidWildCard_RemovesCardFromPlayerHand()
{
    // Arrange
    var player1 = new Player("Player1");
    var player2 = new Player("Player2");

    _game.ChangePlayers(new List<IPlayer> { player1, player2 });

    _game.StartGame();
    // Buat Wild Card mock
    var wildCardMock = new Mock<ICard>();
    wildCardMock.Setup(c => c.Type).Returns(CardType.Wild);
    wildCardMock.Setup(c => c.Color).Returns((CardColor?)null); // Wild awalnya tanpa warna
    var wildCard = wildCardMock.Object;

    // Masukkan ke tangan player
    var playerCardsField = typeof(GameController)
        .GetField("_playerCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    var dict = (Dictionary<IPlayer, List<ICard>>)playerCardsField.GetValue(_game);
    dict[player1].Clear();
    dict[player1].Add(wildCard);

    int cardCountBefore = dict[player1].Count;

    // Act
    _game.PlayCard(player1, wildCard);

    // Assert
    Assert.That(dict[player1].Count, Is.EqualTo(cardCountBefore - 1),
        "Kartu Wild seharusnya dihapus dari tangan player setelah dimainkan");
}

   [Test]
    public void PlayCard_InvalidCard_DoesNothing()
    {
        // Arrange
        var player1 = new Player("Player1");
        var player2 = new Player("Player2");

        _game.ChangePlayers(new List<IPlayer> { player1, player2 });
        _game.StartGame();

        var lastPlayed = _game.GetLastPlayedCard();

        _game.DrawCard(player1);
        int countBefore = _game.GetPlayerCards(player1).Count;

        // BUAT MOCK CARD
        var mockCard = new Mock<ICard>();
        mockCard.Setup(c => c.Type).Returns(CardType.Skip);
        mockCard.Setup(c => c.Color).Returns(
            lastPlayed.Color == CardColor.Red
                ? CardColor.Blue
                : CardColor.Red
        );

        bool eventRaised = false;
        _game.CardPlayed += (_, _) => eventRaised = true;

        // Act
        _game.PlayCard(player1, mockCard.Object);

        // Assert
        Assert.That(_game.GetPlayerCards(player1).Count,
            Is.EqualTo(countBefore));

        Assert.That(eventRaised, Is.False);
    }


//GetPlayerName
    [Test]
    public void GetPlayerByName_NameExists_ReturnsPlayer()
    {
        // Arrange
        var player1 = new Player("Player1");
        var player2 = new Player("Player2");

        _game.ChangePlayers(new List<IPlayer> { player1, player2 });

        // Act
        var result = _game.GetPlayerByName("Player1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(player1));
    }

    [Test]
    public void GetPlayerByName_NameDoesNotExist_ReturnsNull()
    {
        // Arrange
        var player1 = new Player("Player1");

        _game.ChangePlayers(new List<IPlayer> { player1 });

        // Act
        var result = _game.GetPlayerByName("PlayerX");

        // Assert
        Assert.That(result, Is.Null);
    }

//drawCard
    [Test]
    public void DrawCard_PlayerDrawsCard_AddCardToHand()
{
    // Arrange
    var player = new Player("Player1");
    _game.ChangePlayers(new List<IPlayer> { player });

    int beforeCount = _game.GetPlayerCards(player).Count;

    // Act
    _game.DrawCard(player);

    // Assert
    Assert.That(
        _game.GetPlayerCards(player).Count,
        Is.EqualTo(beforeCount + 1)
    );
}

    [Test]
    public void DrawCard_PlayerNotRegistered_ThrowsKeyNotFoundException()
    {
        // Arrange
        var registeredPlayer = new Player("Player1");
        var unregisteredPlayer = new Player("PlayerX");

        _game.ChangePlayers(new List<IPlayer> { registeredPlayer });

        // Act & Assert
        Assert.That(
            () => _game.DrawCard(unregisteredPlayer),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }


    [Test]
    public void GetLastPlayedCard_AfterPlayCard_ReturnsLastPlayedCard()
{
    // Arrange
    var player1 = new Player("Player1");
    var player2 = new Player("Player2");

    _game.ChangePlayers(new List<IPlayer> { player1, player2 });
    _game.StartGame();


    var wildCardMock = new Mock<ICard>();

    // Set tipe kartu menjadi Wild
    wildCardMock.Setup(c => c.Type).Returns(CardType.Wild);

    // Set warnanya null (pemain akan pilih saat dimainkan)
    wildCardMock.Setup(c => c.Color).Returns((CardColor?)null);

    // Ambil object mock untuk digunakan
    ICard wildCard = wildCardMock.Object;

    var playerCardsField = typeof(GameController)
    .GetField("_playerCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

    var dict = (Dictionary<IPlayer, List<ICard>>)playerCardsField.GetValue(_game);

    // Masukkan wild card ke tangan player
    dict[player1].Add(wildCard);

    var card = _game.GetPlayerCards(player1).Last();
    // Act
    _game.PlayCard(player1,card );

    var result = _game.GetLastPlayedCard();

    // Assert
    Assert.That(result, Is.EqualTo(card));
}

    [Test]
    public void GetLastPlayedCard_NoCardPlayed_ReturnsNull()
    {
        // Arrange
        _game.ChangePlayers(new List<IPlayer>
        {
            new Player("Player1"),
            new Player("Player2")
        });

        // Act
        var result = _game.GetLastPlayedCard();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
public void GetLastPlayer_AfterPlayCard_ReturnsLastPlayer()
{
    // Arrange
    var player1 = new Player("Player1");
    var player2 = new Player("Player2");

    _game.ChangePlayers(new List<IPlayer> { player1, player2 });
    _game.StartGame();

   var wildCardMock = new Mock<ICard>();

    // Set tipe kartu menjadi Wild
    wildCardMock.Setup(c => c.Type).Returns(CardType.Wild);

    // Set warnanya null (pemain akan pilih saat dimainkan)
    wildCardMock.Setup(c => c.Color).Returns((CardColor?)null);

    // Ambil object mock untuk digunakan
    ICard wildCard = wildCardMock.Object;

    var playerCardsField = typeof(GameController)
    .GetField("_playerCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

    var dict = (Dictionary<IPlayer, List<ICard>>)playerCardsField.GetValue(_game);

    // Masukkan wild card ke tangan player
    dict[player1].Add(wildCard);

    ICard card = _game.GetPlayerCards(player1).Last();

    _game.PlayCard(player1, card);

    _game.Nexturn();
    var result = _game.GetLastPlayer();

    // Assert
    Assert.That(result, Is.EqualTo(player1));
}

[Test]
public void GetLastPlayer_NoCardPlayed_ReturnsNull()
{
    // Arrange
    _game.ChangePlayers(new List<IPlayer>
    {
        new Player("Player1"),
        new Player("Player2")
    });

    // Act
    var result = _game.GetLastPlayer();

    // Assert
    Assert.That(result, Is.Null);
}

[Test]
public void GetCurrentPlayer_PlayersInitialized_ReturnsFirstPlayer()
{
    // Arrange
    var player1 = new Player("Player1");
    var player2 = new Player("Player2");

    _game.ChangePlayers(new List<IPlayer> { player1, player2 });

    // Act
    var result = _game.GetCurrentPlayer();

    // Assert
    Assert.That(result, Is.EqualTo(player1));
}

[Test]
public void GetCurrentPlayer_NoPlayers_ThrowsInvalidOperationException()
{
    // Arrange
    _game.ResetGame(_game.Deck, _game.DiscardPile);

    // Act
    try
    {
        _game.GetCurrentPlayer();
        Assert.Fail("Expected InvalidOperationException was not thrown");
    }
    catch (InvalidOperationException)
    {
        // Assert
        Assert.Pass();
    }
}

[Test]
public void GetPlayerCards_RegisteredPlayer_ReturnsPlayerCards()
{
    // Arrange
    var player = new Player("Player1");
    _game.ChangePlayers(new List<IPlayer> { player });

    _game.DrawCard(player);

    // Act
    var cards = _game.GetPlayerCards(player);

    // Assert
    Assert.That(cards.Count, Is.EqualTo(1));
}

[Test]
public void GetPlayerCards_PlayerNotRegistered_ThrowsKeyNotFoundException()
{
    // Arrange
    var registeredPlayer = new Player("Player1");
    var unregisteredPlayer = new Player("PlayerX");

    _game.ChangePlayers(new List<IPlayer> { registeredPlayer });

    // Act
    try
    {
        _game.GetPlayerCards(unregisteredPlayer);
        Assert.Fail("Expected KeyNotFoundException was not thrown");
    }
    catch (KeyNotFoundException)
    {
        // Assert
        Assert.Pass();
    }
}

[Test]
public void IsCardValid_NoLastPlayedCard_ReturnsFalse()
{
    // Arrange
    var card = new Card(CardType.Number3, CardColor.Red);

    // Act
    var result = _game.IsCardValid(card);

    // Assert
    Assert.That(result, Is.False);
}

[Test]
public void IsCardValid_SameColorAsLastPlayed_ReturnsTrue()
{
    // Arrange
    var player1 = new Player("Player1");
    var player2 = new Player("Player2");

    _game.ChangePlayers(new List<IPlayer> { player1, player2 });

    _game.StartGame();

    // last played card sudah ada karena StartGame()
    var lastPlayed = _game.GetLastPlayedCard();
    Assert.That(lastPlayed, Is.Not.Null);

    // buat kartu dengan warna sama
    var validCard = new Card(
        CardType.Number5,
        lastPlayed.Color!.Value
    );

    // Act
    var result = _game.IsCardValid(validCard);

    // Assert
    Assert.That(result, Is.True);
}

[Test]
public void SetCurrentColor_ValidColor_UpdatesCurrentColorAndRaisesEvent()
{
    // Arrange
    CardColor? raisedColor = null;
    _game.CurrentColorChanged += color => raisedColor = color;

    // Act
    _game.SetCurrentColor(CardColor.Blue);

    // Assert
    Assert.That(_game.CurrentColor, Is.EqualTo(CardColor.Blue));
    Assert.That(raisedColor, Is.EqualTo(CardColor.Blue));
}

[Test]
public void SetCurrentColor_NoEventSubscriber_DoesNotThrow()
{
    // Act & Assert
    Assert.That(
        () => _game.SetCurrentColor(CardColor.Red),
        Throws.Nothing
    );
}


  [Test]
    public void ResetGame_Positive_AllStateReset()
    {
        // Arrange
        var oldDeck = _game.Deck;
        var oldDiscard = _game.DiscardPile;

        // Tambahkan pemain dan kartu agar ada state untuk di-reset
        var player = new Player("Player1");
        _game.ChangePlayers(new List<IPlayer> { player });
        _game.StartGame();

        // Act
        var newDeck = new Mock<IDeck>();
        newDeck.Setup(d => d.Cards).Returns(new Stack<ICard>());

        var newDiscard = new Mock<IDiscardPile>();
        newDiscard.Setup(d => d.Cards).Returns(new Stack<ICard>());

        _game.ResetGame(newDeck.Object, newDiscard.Object);

        // Assert
        Assert.That(_game.Players, Is.Null, "Players should be null after reset");
        Assert.That(_game.GetPlayerCardCounts().Count, Is.EqualTo(0), "Player cards should be empty");
        Assert.That(_game.GetLastPlayedCard(), Is.Null);
        Assert.That(_game.GetLastPlayer(), Is.Null);
        Assert.That(_game.IsGameOver, Is.False);
        Assert.That(_game.IsGameStarted, Is.False);
        Assert.That(_game.Direction, Is.EqualTo(Direction.Clockwise));
        Assert.That(_game.CurrentColor, Is.EqualTo(default(CardColor)));
        Assert.That(_game.Deck, Is.EqualTo(newDeck.Object));
        Assert.That(_game.DiscardPile, Is.EqualTo(newDiscard.Object));

        
    }

    [Test]
    public void ResetGame_Negative_OldDeckNotUsed()
    {
        // Arrange
        var oldDeck = _game.Deck;
        var oldDiscard = _game.DiscardPile;

        // Act
        var newDeck = new Mock<IDeck>();
        newDeck.Setup(d => d.Cards).Returns(new Stack<ICard>());

        var newDiscard = new Mock<IDiscardPile>();
        newDiscard.Setup(d => d.Cards).Returns(new Stack<ICard>());

        _game.ResetGame(newDeck.Object, newDiscard.Object);

        // Assert: pastikan objek lama **tidak lagi digunakan**
        Assert.That(_game.Deck, Is.Not.EqualTo(oldDeck));
        Assert.That(_game.DiscardPile, Is.Not.EqualTo(oldDiscard));
    }


}
