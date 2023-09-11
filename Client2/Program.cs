using System.Net.WebSockets;
using System.Text;


var client = new ClientWebSocket();
Uri serverUri = new Uri("ws://localhost:8080/"); // URL do servidor WebSocket

try
{
    await client.ConnectAsync(serverUri, CancellationToken.None);
    Console.WriteLine("Conectado ao servidor WebSocket");

    var sendTask = Task.Run(async () =>
    {
        while (true)
        {
            Console.Write("Digite uma mensagem para enviar ao servidor (ou 'exit' para sair): ");
            string input = Console.ReadLine();
            if (input.ToLower() == "exit")
            {
                break;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(input);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    });

    var receiveTask = Task.Run(async () =>
    {
        var receiveBuffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            string response = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            Console.WriteLine($"Mensagem do servidor: {response}");

            result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        }
    });

    await Task.WhenAny(sendTask, receiveTask);

    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechando a conexão", CancellationToken.None);
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}
