import * as signalR from "@microsoft/signalr";
import { HubEndpoint } from "./types/HubEndpoint";

const tempElement = document.getElementById("temp-display");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/stathub")
    .withAutomaticReconnect()
    .build();

connection.on(HubEndpoint.ReceiveMessage, (message: string) => {
    console.log(message);
});

connection.on(HubEndpoint.ReceiveTemperature, (temperature: number) => {
    tempElement.innerHTML = temperature.toFixed(2);
});

connection.start().catch((err: any) => console.log(err));

export function send(message: string) {
    connection.send(HubEndpoint.SendMessage, message);
}
