namespace Dto;

public class PlayerStateDTO
{
    public string LastCard { get; set; }
    public string Player { get; set; }
    public List<CardDTO> Hand { get; set; }
}
