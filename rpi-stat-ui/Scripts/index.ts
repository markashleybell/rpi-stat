import * as signalR from "@microsoft/signalr";
import { HubEndpoint } from "./types/HubEndpoint";
import { HeatingState } from "./types/HeatingState";
import { IRetryPolicy } from "@microsoft/signalr";

const tempElement = document.getElementById("temp-display");

const tempSetting = document.getElementById("temperature");

let currentHeatingState = HeatingState.Off;

function getTemperatureSetting() {
    const value = (tempSetting as HTMLInputElement).value;
    return parseFloat(value);
}

function asString(heatingState: HeatingState) {
    return heatingState === HeatingState.On ? 'On' : 'Off';
}

const retryPolicy: IRetryPolicy = {
    nextRetryDelayInMilliseconds: () => 2000
};

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://rpi-stat/stathub")
    .withAutomaticReconnect(retryPolicy)
    .build();

connection.on(HubEndpoint.ReceiveMessage, console.log);

connection.on(HubEndpoint.ReceiveHeatingStateConfirmation, (heatingState: HeatingState) => {
    currentHeatingState = heatingState;
    console.log(`Heating state confirmation received: ${asString(heatingState)}`);
});

connection.on(HubEndpoint.ReceiveTemperature, (temperature: number) => {
    tempElement.innerHTML = temperature.toFixed(2);
    const temperatureSetting = getTemperatureSetting();
    if (temperature < temperatureSetting && currentHeatingState == HeatingState.Off) {
        console.log(`Requesting heating state: ${asString(HeatingState.On)}`);
        connection.send(HubEndpoint.RequestHeatingState, HeatingState.On);
    } else if (temperature >= temperatureSetting && currentHeatingState == HeatingState.On) {
        console.log(`Requesting heating state: ${asString(HeatingState.Off)}`);
        connection.send(HubEndpoint.RequestHeatingState, HeatingState.Off);
    }
});

connection.start().then(() => console.log('Connected to server')).catch((err: any) => console.log(err));

connection.onclose(error => console.log('Connection closed'));

connection.onreconnected(connectionId => console.log('Connected to server'));

export function send(message: string) {
    connection.send(HubEndpoint.SendMessage, message);
}
