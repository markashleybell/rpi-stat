import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/stathub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveMessage", (message: string) => console.log(message));

connection.start().catch((err: any) => console.log(err));

export function send(message: string) {
    connection.send("SendMessage", message);
        // .then(() => tbMessage.value = "");
}
