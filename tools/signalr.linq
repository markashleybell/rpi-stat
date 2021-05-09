<Query Kind="Program">
  <NuGetReference>Microsoft.AspNetCore.SignalR.Client</NuGetReference>
  <Namespace>Microsoft.AspNetCore.SignalR.Client</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    var connection = new HubConnectionBuilder()
        .WithUrl("https://rpi-stat/stathub")
        .WithAutomaticReconnect()
        .Build();

    connection.Closed += error => {
        error.Dump();
        return Task.CompletedTask;
    };

    connection.On<string>("ReceiveMessage", message => {
        message.Dump();
    });

    try
    {
        await connection.StartAsync();
    }
    catch (Exception ex)
    {
        ex.Dump();
    }

    try
    {
        await connection.InvokeAsync("SendMessage", "TEST MESSAGE FROM CLIENT");
    }
    catch (Exception ex)
    {
        ex.Dump();
    }
}


