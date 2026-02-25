using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class SupportHub : Hub
{
    public static ConcurrentDictionary<string, string> Users =
        new ConcurrentDictionary<string, string>();

    public static ConcurrentDictionary<string, string> ActiveSessions =
        new ConcurrentDictionary<string, string>();

    public override Task OnConnectedAsync()
    {
        var username = Context.User.Identity.Name;
        Users[Context.ConnectionId] = username;
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Users.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterAdmin()
    {
        Console.WriteLine($"Admin registered: {Context.ConnectionId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
    }

    public async Task RequestSupport()
    {
        Console.WriteLine($"Support requested by: {Context.ConnectionId}");

        await Clients.Group("Admins")
            .SendAsync("SupportRequested",
                Context.ConnectionId,
                Context.User?.Identity?.Name);
    }

    public async Task SendOffer(string targetId, string offer)
    {
        await Clients.Client(targetId)
            .SendAsync("ReceiveOffer", Context.ConnectionId, offer);
    }

    public async Task SendAnswer(string targetId, string answer)
    {
        await Clients.Client(targetId)
            .SendAsync("ReceiveAnswer", answer);
    }

    public async Task SendIce(string targetId, string candidate)
    {
        await Clients.Client(targetId)
            .SendAsync("ReceiveIce", candidate);
    }
}