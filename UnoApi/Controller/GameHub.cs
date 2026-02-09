using Microsoft.AspNetCore.SignalR;
namespace Controllers;
public class GameHub : Hub
{
    // kosong pun tidak apa-apa
    // controller yang akan broadcast
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
}
