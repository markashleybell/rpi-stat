import * as signalR from "@microsoft/signalr";
import { HubEndpoint } from "./types/HubEndpoint";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/stathub")
    .withAutomaticReconnect()
    .build();

connection.on(HubEndpoint.ReceiveMessage, (message: string) => console.log(message));

connection.start().catch((err: any) => console.log(err));

export function send(message: string) {
    connection.send(HubEndpoint.SendMessage, message);
}
