# .NET WebSocket Chat Application

This is a simple chat application using WebSockets in .NET 6. It includes a WebSocket server and client that allow real-time communication between multiple clients.

## Features

- Clients can connect to the server using WebSocket.
- Clients can send and receive text messages in real-time.
- Multiple clients can participate in the chat.

## Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional)

## Usage

1. Clone this repository:

   ```bash
   git clone https://github.com/guilhermebernava/WebSocketCommunication.git
   cd websocket-chat-app
   ```

2. Open the solution in your preferred IDE (e.g., Visual Studio or Visual Studio Code).

3. Build and run the WebSocket server:

   - If using Visual Studio, simply press F5 to start the server.
   - If using the command line, navigate to the server project folder and run:

     ```bash
     dotnet run
     ```

4. Build and run the WebSocket client:

   - If using Visual Studio, right-click on the client project and select "Start Debugging."
   - If using the command line, navigate to the client project folder and run:

     ```bash
     dotnet run
     ```

5. In the client console, you can send and receive messages in real-time.

6. To exit the client or server, type "exit."
