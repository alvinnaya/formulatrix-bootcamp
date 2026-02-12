using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Cards;
using Players;
using Controller;
using Dto;
namespace Api
{
    [ApiController]
    [Route("game")]
    public class GameApiController : ControllerBase
    {
        private readonly GameController game;
        private readonly IHubContext<global::GameHub> hub;

        public GameApiController(GameController game, IHubContext<global::GameHub> hub)
        {
            this.game = game;
            this.hub = hub;
        }

        // ===== CREATE PLAYERS =====
        [HttpPost("createplayer")]
        public IActionResult CreatePlayers([FromQuery] string? prefix, [FromQuery] int count)
        {
            if (!string.IsNullOrEmpty(prefix) && prefix != "Game") return Ok();
            if (game.IsGameStarted) return Ok();
            if (count <= 0) return BadRequest("count must be > 0");

            var newPlayers = new List<IPlayer>();
            for (int i = 1; i <= count; i++)
            {
                newPlayers.Add(new Player($"Player{i}"));
            }

            game.ChangePlayers(newPlayers);

            BroadcastJson("info", new { message = $"{count} players created" });
            return Ok(new { message = $"{count} players created" });
        }

        // ===== START GAME =====
        [HttpPost("start")]
        public IActionResult StartGame()
        {
            if (game.IsGameStarted) return Ok();
            if (game.Players == null || game.Players.Count == 0) return BadRequest("No players to start the game");

            foreach (var p in game.Players)
                for (int i = 0; i < 7; i++)
                    game.DrawCard(p);

            game.StartGame();
            BroadcastGameState("Game started");
            return Ok(new { message = "Game started" });
        }

        // ===== GET CURRENT GAME STATE =====
        [HttpGet("state")]
        public IActionResult GetCurrentState()
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started" });

            var state = new
            {
                CurrentPlayer = game.GetCurrentPlayer().Name,
                LastCard = game.GetLastPlayedCard()?.ToString(),
                PlayerCardCounts = game.GetPlayerCardCounts(),
                CurrentColor = game.CurrentColor.ToString(),
                Direction = game.Direction.ToString(),
                IsGameOver = game.IsGameOver
            };

            BroadcastGameState("state requested");
            return Ok(state);
        }

        // ===== GET PLAYER HAND =====
        [HttpGet("hand")]
        public IActionResult GetPlayerHand(string playerName)
        {
            if (!game.IsGameStarted) return Ok();

            var player = game.GetPlayerByName(playerName);
            if (player == null) return NotFound("Player not found");

            var playerState = new PlayerStateDTO
            {
                Player = player.Name,
                LastCard = game.GetLastPlayedCard()?.ToString() ?? "",
                Hand = BuildHandState(game.GetPlayerCards(player))
            };

            return Ok(playerState);
        }

        // ===== DRAW CARD =====
        [HttpPost("draw")]
        public IActionResult DrawCard(string playerName)
        {
            if (!game.IsGameStarted) return Ok();

            var current = game.GetCurrentPlayer();
            if (current.Name != playerName) return BadRequest("Not your turn");

            game.DrawCard(current);
            game.Nexturn();

            var playerState = new PlayerStateDTO
            {
                Player = current.Name,
                LastCard = game.GetLastPlayedCard()?.ToString() ?? "",
                Hand = BuildHandState(game.GetPlayerCards(current))
            };

            BroadcastGameState($"{current.Name} drew a card");
            return Ok(playerState);
        }

        // ===== PLAY CARD =====
        [HttpPost("play")]
        public IActionResult PlayCard(string playerName, int cardIndex, string? color = null)
        {
            if (!game.IsGameStarted) return Ok();

            var current = game.GetCurrentPlayer();
            if (current.Name != playerName) return BadRequest("Not your turn");

            var hand = game.GetPlayerCards(current);
            if (cardIndex < 0 || cardIndex >= hand.Count)
                return BadRequest("Invalid card index");

            var card = hand[cardIndex];

            if (!game.IsCardValid(card))
                return BadRequest("Invalid card played");

            if ((card.Type == CardType.Wild || card.Type == CardType.WildDrawFour) && color != null)
            {
                if (!Enum.TryParse<CardColor>(color, true, out var chosenColor))
                    return BadRequest("Invalid color for wild card");

                game.SetCurrentColor(chosenColor);
            }

            game.PlayCard(current, card);
            game.Nexturn();

            // BroadcastGameState($"{current.Name} played {card}");
            return Ok(new { message = $"{current.Name} played {card}" });
        }

        // ===== CALL UNO =====
        [HttpPost("uno")]
        public IActionResult CallUno(string playerName)
        {
            if (!game.IsGameStarted) return Ok();

            var player = game.GetPlayerByName(playerName);
            if (player == null) return NotFound("Player not found");

            game.CallUno(player);
            // BroadcastJson("info", new { message = $"{player.Name} called UNO!" });
            // BroadcastGameState($"{player.Name} called UNO");

            return Ok(new { message = $"{player.Name} called UNO" });
        }


        private void BroadcastGameState(string action)
        {
            if (!game.IsGameStarted) return;

            var lastCard = game.GetLastPlayedCard();

            var gameState = new GameStateDTO
            {
                LastCard = lastCard?.ToString() ?? "",
                CurrentPlayer = game.GetCurrentPlayer().Name,
                AllPlayers = GetPlayersCardCount(game),
                Action = action,
                CurrentColor = game.CurrentColor.ToString(),
                GameEnd = game.IsGameOver
            };

            BroadcastJson("gameState", gameState);
        }

        private void BroadcastJson(string type, object data)
        {
            var payload = new { type, data };
            _ = hub.Clients.All.SendAsync("info", payload);
        }

        private List<PlayerCardCountDTO> GetPlayersCardCount(GameController game)
        {
            var playerCounts = game.GetPlayerCardCounts();

            return playerCounts.Select(kvp => new PlayerCardCountDTO
            {
                Name = kvp.Key,
                CardCount = kvp.Value
            }).ToList();
        }

        //retturn list yang berisi informasi tentang setiap kartu yang dimiliki player
        private List<CardDTO> BuildHandState(IReadOnlyList<ICard> hand)
        {
            return hand
                .Select((c, idx) => new CardDTO
                {
                    Index = idx,
                    Card = c.ToString() ?? string.Empty,
                    CardColor = c.Color?.ToString() ?? string.Empty,
                    CardType = c.Type.ToString()
                })
                .ToList();
        }

    }
}
