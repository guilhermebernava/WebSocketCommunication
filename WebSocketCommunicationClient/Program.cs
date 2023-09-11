using Client;

await Worker.HandleMessages(await Worker.ConnectToServer());
