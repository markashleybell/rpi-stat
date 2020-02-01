import * as signalR from "@microsoft/signalr";
import { HubEndpoint } from "./types/HubEndpoint";
import { HeatingState } from "./types/HeatingState";

const tempElement = document.getElementById("temp-display");

const tempSetting = document.getElementById("temperature");

let currentHeatingState = HeatingState.Off;

function getTemperatureSetting() {
    const value = (tempSetting as HTMLInputElement).value;
    return parseFloat(value);
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/stathub")
    .withAutomaticReconnect()
    .build();

connection.on(HubEndpoint.ReceiveMessage, console.log);

connection.on(HubEndpoint.ReceiveHeatingStateConfirmation, (heatingState: HeatingState) => {
    currentHeatingState = heatingState;
    console.log(`Heating State Is Now: ${heatingState}`);
});

connection.on(HubEndpoint.ReceiveTemperature, (temperature: number) => {
    tempElement.innerHTML = temperature.toFixed(2);
    const temperatureSetting = getTemperatureSetting();
    if (temperature < temperatureSetting && currentHeatingState == HeatingState.Off) {
        console.log(`Request Heating State: ${HeatingState.On}`);
        connection.send(HubEndpoint.RequestHeatingState, HeatingState.On);
    } else if (temperature >= temperatureSetting && currentHeatingState == HeatingState.On) {
        console.log(`Request Heating State: ${HeatingState.Off}`);
        connection.send(HubEndpoint.RequestHeatingState, HeatingState.Off);
    }
});

connection.start().catch((err: any) => console.log(err));

export function send(message: string) {
    connection.send(HubEndpoint.SendMessage, message);
}
