namespace Dto;

public class GameStateDTO
{
    public string LastCard { get; set; }
    public string CurrentPlayer { get; set; }
    public List<PlayerCardCountDTO> AllPlayers { get; set; }

    public string Action { get; set; }
    public string CurrentColor { get; set; }
    public bool IsGameStarted { get; set; }

    public bool GameEnd { get; set; }
    
}