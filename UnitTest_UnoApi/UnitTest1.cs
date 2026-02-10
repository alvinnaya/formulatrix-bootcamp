using Cards;
using Deck;
using GameControllerNamespace;
using Players;
using helperFunction;
using Moq;


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
        _deck = new Deck.Deck();
        _discardPile = new DiscardPile();
        _playerList = new List<IPlayer>();
        Helper.InitDeck(_deck);
        _game = new GameController( _playerList ,_deck, _discardPile);
        
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
    public void PlayCard_ValidCard_RemoveCardFromPlayerHand()
    {
        // Arrange
        var player1 = new Player("Player1");
        var player2 = new Player("Player2");

        _game.ChangePlayers(new List<IPlayer> { player1, player2 });
        _game.StartGame();

        // Ambil kartu lewat API resmi
        _game.DrawCard(player1);
        var card = _game.GetPlayerCards(player1).First();

        int cardCountBefore = _game.GetPlayerCards(player1).Count;

        // Act
        _game.PlayCard(player1, card);

        // Assert
        Assert.That(_game.GetPlayerCards(player1).Count,
            Is.EqualTo(cardCountBefore - 1));
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

}
