using Microsoft.AspNetCore.SignalR;

namespace WelcomeScreen.API.Hubs;

public class DisplayHub : Hub
{
    // Client màn LED join vào group theo welcomeScreenId
    public async Task JoinScreen(string screenId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"screen_{screenId}");

    public async Task LeaveScreen(string screenId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"screen_{screenId}");
}