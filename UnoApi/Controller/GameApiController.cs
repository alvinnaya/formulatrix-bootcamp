using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameControllerNamespace;
using helperFunction;
using Players;
using Deck;
using Cards;
using Dto;



namespace Controllers;

    [ApiController]
    [Route("game")]
    public class GameApiController : ControllerBase
    {
        private readonly GameController game; // Injected game object
         // Injected hub for notifications
        private readonly IHubContext<GameHub> hub;

        public GameApiController(GameController game,IHubContext<GameHub> hub)
        {
            this.game = game;
            this.hub = hub;
            
        }

        [HttpPost("createplayer")]
        public IActionResult CreatePlayer( [FromQuery] int count)
        {
          
            if (game.IsGameStarted) return Ok(new { message = "Game already started" });
            Console.WriteLine(count);
            List<IPlayer> newPlayers = new();
            Helper.CreatePlayers(count, newPlayers);
            game.ChangePlayers(newPlayers);

            // BroadcastJson("info", new { message = $"there was {count} Players" });
            return Ok(new { message = $"Created {count} players" });
        }

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            if (game.IsGameStarted) return Ok(new { message = "Game already started" });
            if (game.Players == null || game.Players.Count == 0) return Ok(new { message = "No players in the game" });

            foreach (var p in game.Players)
                for (int i = 0; i < 7; i++)
                    game.DrawCard(p);

            game.StartGame();
            // BroadcastGameState("game start");
            return Ok(new { message = "Game started successfully" });
        }

        [HttpGet("getcurrentstate")]
        public IActionResult GetCurrentState()
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started yet" });
            // BroadcastGameState("state");
            GameStateDTO state = Helper.BroadcastGameState(game, "GameState");
            return Ok(state);
        }

        [HttpGet("getcard")]
        public IActionResult GetCard([FromQuery] string player)
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started yet" });

            IPlayer p = game.GetPlayerByName(player);
            if (p == null) return Ok(new { message = "Player not found" });

            PlayerStateDTO PlayerState = new PlayerStateDTO
            {
                LastCard = game.GetLastPlayedCard()?.ToString() ?? "",
                Player = p.Name,
                Hand = Helper.BuildHandState(game.GetPlayerCards(p))
            };

            return Ok(PlayerState);
        }

        [HttpPost("draw")]
        public IActionResult Draw([FromQuery] string player)
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started yet" });

            var current = game.GetCurrentPlayer();
            if (current.Name != player) return Ok(new { message = "you are not current player" });

            game.DrawCard(current);
            game.Nexturn();

            PlayerStateDTO PlayerState = new PlayerStateDTO
            {
                LastCard = game.GetLastPlayedCard()?.ToString() ?? "",
                Player = current.Name,
                Hand = Helper.BuildHandState(game.GetPlayerCards(current))
            };
            GameStateDTO Gamestate = Helper.BroadcastGameState(game, "state");

            // BroadcastGameState($"{current} draw card");
            Helper.BroadcastJson("GameState",Gamestate,hub);
            return Ok(PlayerState);
        }

        [HttpPost("play")]
        public async Task<IActionResult> Play(
            [FromQuery] string player, 
            [FromQuery] int idx, 
            [FromQuery] string? color,
             [FromQuery] string connectionId)
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started yet" });

            var current = game.GetCurrentPlayer();
            if (current.Name != player) return Ok(new { message = "you are not current player" });

            var hand = game.GetPlayerCards(current);
            var card = hand[idx];

            if (!game.IsCardValid(card))
            {
                return BadRequest(new { message = "invalid card" });
            }

            if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
            {
                if (!Enum.TryParse<CardColor>(color, true, out var chosen))
                    return BadRequest(new { message = "invalid color for wild card" });

                game.SetCurrentColor(chosen);
            }

            game.PlayCard(current, card);

            if (hand.Count == 1)
            {
                PlayerStateDTO PlayerState = new PlayerStateDTO{
                    LastCard = game.GetLastPlayedCard()?.ToString(), 
                    Player = current.Name, 
                    Hand = Helper.BuildHandState(game.GetPlayerCards(current)) 
                };
                await hub.Clients.Client(connectionId)
                    .SendAsync("PlayerState", PlayerState );
                
                GameStateDTO gameStateBefore =  Helper.BroadcastGameState(game,$"{current} played {card}");
                Helper.BroadcastJson("GameState",gameStateBefore,hub);
                await Task.Delay(3000);
            }

            game.Nexturn();
            PlayerStateDTO PlayerStateAfter = new PlayerStateDTO
                {
                    LastCard = game.GetLastPlayedCard()?.ToString(), 
                    Player = current.Name, 
                    Hand = Helper.BuildHandState(game.GetPlayerCards(current)),
                };

            await hub.Clients.Client(connectionId)
                .SendAsync("PlayerState", PlayerStateAfter);

             GameStateDTO gameStateAfter =  Helper.BroadcastGameState(game,$"{current} played {card}");
                Helper.BroadcastJson("GameState",gameStateAfter,hub);
            return Ok();
        }

        [HttpPost("uno")]
        public IActionResult CallUno([FromQuery] string player)
        {
            if (!game.IsGameStarted) return Ok(new { message = "Game not started yet" });

            var p = game.GetPlayerByName(player);
            game.CallUno(p);

            Helper.BroadcastJson("info", new { message = $"{p.Name} called UNO!" },hub);
            // GameStateDTO gameState =  Helper.BroadcastGameState(game, $"{p} call uno");
            // Helper.BroadcastJson(,hub)
            return Ok(new { message = $"{p.Name} called UNO!" });
        }
    
    
    
    
    
    }

