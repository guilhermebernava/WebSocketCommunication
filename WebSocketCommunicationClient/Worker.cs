using System.Net.WebSockets;
using System.Text;

namespace Client;

public static class Worker
{
    private static readonly object consoleLock = new object();
    //primeiramente tentamos se conectar com o server
    public static async Task<ClientWebSocket> ConnectToServer()
    {
        var client = new ClientWebSocket();
        Uri serverUri = new ("ws://localhost:8080/");

        try
        {
            await client.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Conectado ao servidor WebSocket");
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            throw;
        }
    }

    private static Task HandleRequests(ClientWebSocket client)
    {
        var sendTask = Task.Run(async () =>
        {
            while (true)
            {
                Console.Write("Send a message ('exit' to close): ");
                var input = Console.ReadLine();
                if (input == null) continue;

                if (input.ToLower() == "exit") break;

                byte[] buffer = Encoding.UTF8.GetBytes(input);

                lock (consoleLock)
                {
                    Console.WriteLine();
                    Console.WriteLine($"You: {input}");
                }

                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        });

        return sendTask;
    }

    private static Task HandleResponses(ClientWebSocket client)
    {
        var receiveTask = Task.Run(async () =>
        {
            var receiveBuffer = new byte[1024];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string response = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

                lock (consoleLock)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Server: {response}");
                }

                result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            }
        });

        return receiveTask;
    }

    public static async Task HandleMessages(ClientWebSocket client)
    {
        //em qualquer momento que alguma dessas duas chegar ele vai escutar.
        await Task.WhenAny(HandleResponses(client),HandleRequests(client));
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
    }
}
