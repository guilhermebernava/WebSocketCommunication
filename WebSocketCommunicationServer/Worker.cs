using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Server;

public static class Worker
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> clients = new();
    public static HttpListener InitServer()
    {
        //primeiramente abrirmos um server HTTP para receber as requisições
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:8080/");
        httpListener.Start();
        Console.WriteLine("Server running on http://localhost:8080/");
        return httpListener;
    }

    public async static Task HandleRequests(HttpListener httpListener)
    {
        while (true)
        {
            //quando recebemos a requisição verificamos se ela é uma requisição de websocket
            //e caso for chamamos o método do HANDLE_CLIENT
            var context = await httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                var webSocket = webSocketContext.WebSocket;

                var clientId = Guid.NewGuid();
                clients.TryAdd(clientId, webSocket);
                Console.WriteLine($"Client Connected: {clientId}");

                _ = HandleClient(clientId, webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async Task HandleClient(Guid clientId, WebSocket webSocket)
    {
        var buffer = new byte[1024];
        WebSocketReceiveResult result;

        try
        {
            //Por aqui vamos pegar o dado que o usuário mandou, mostrar em tela
            //e podemos mandar uma mensagem de volta para ele
            while (webSocket.State == WebSocketState.Open)
            {
                //pega o que é o cliente mandou
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received from {clientId}: {message}");

                    var formattedMessage = string.Concat(DateTime.Now.ToString("HH:mm:ss") + $" {clientId}: ", message);
                    if (formattedMessage == null) throw new Exception("Error in format string");
                    var formattedResponse = Encoding.UTF8.GetBytes(formattedMessage);
                    var senderClient = clients.GetValueOrDefault(clientId);

                    foreach (var client in clients.Values)
                    {
                        if (client == senderClient) continue;
                        await client.SendAsync(new ArraySegment<byte>(formattedResponse), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
        catch (WebSocketException ex)
        {
            await Console.Out.WriteLineAsync($"Error: {ex}");
        }
        finally
        {
            clients.TryRemove(clientId, out _);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Connection", CancellationToken.None);
        }
    }
}