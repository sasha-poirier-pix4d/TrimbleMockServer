using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using PseudoExtras;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
app.Map("/", async context => 
{
    Console.WriteLine("Client Connected");
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        
        int count = 0;
        DiffStatus status = DiffStatus.Autonomous;
        while (true)
        {
            if (count >= 5) {
                count = 0;
                status = status switch {
                    DiffStatus.Autonomous => DiffStatus.DGPS,
                    DiffStatus.DGPS => DiffStatus.Float,
                    DiffStatus.Float => DiffStatus.Fixed,
                    DiffStatus.Fixed => DiffStatus.Autonomous,
                    _ => DiffStatus.Autonomous
                };
            }
            PseudoExtrasMessage extras = PseudoExtrasMessage.MakeNearMalley(status);
            string t = JsonSerializer.Serialize(extras);
            var bytes = Encoding.UTF8.GetBytes(t);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(arraySegment,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
            {
                Console.WriteLine("WebSocket connection closed");
                break;
            }
            count++;
            Thread.Sleep(1000);
        }
    }
    else 
    {
        Console.WriteLine("Bad");
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
    }
});

await app.RunAsync();
