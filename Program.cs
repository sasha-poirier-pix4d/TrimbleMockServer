using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.IO;
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

        while (true)
        {
            //string msg = File.ReadAllText(@"./test-data/test.json");
            PseudoExtrasMessage extras = PseudoExtrasMessage.MakeNearMalley();

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
            Thread.Sleep(5000);
        }
    }
    else 
    {
        Console.WriteLine("Bad");
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
    }
});

await app.RunAsync();
