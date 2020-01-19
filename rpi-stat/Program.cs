using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using core;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace rpi_stat
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var settings = new I2cConnectionSettings(1, MCP9808.MCP9808_I2CADDR_DEFAULT);
            var i2cDevice = I2cDevice.Create(settings);

            var sensor = new MCP9808(i2cDevice);

            if (sensor.Test())
            {
                Console.WriteLine("Sensor detected");
            }

            //while (true)
            //{
            //    Console.WriteLine(sensor.ReadTemperature().Celsius);

            //    Thread.Sleep(500);
            //}

            //var gpioController = new GpioController();

            //var transmitter = new ENER314(gpioController);

            //if (args[0] == "on")
            //{
            //    transmitter.On(1);
            //}
            //else
            //{
            //    transmitter.Off(1);
            //}

            var locationID = new Guid("e9a1b593-e609-4edb-b794-92ace8d4bccf");

            var connection = new HubConnectionBuilder()
                .WithUrl("https://rpi-stat/stathub")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (error) => {
                Console.WriteLine(error);
                await connection.StartAsync();
            };

            connection.On<string>(HubEndpoint.ReceiveMessage, message => {
                Console.WriteLine(message);
            });

            await connection.StartAsync();

            await connection.InvokeAsync(HubEndpoint.SendMessage, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: Stat Is Connected");

            while (true)
            {
                var temperature = sensor.ReadTemperature().Celsius;

                try
                {
                    await connection.InvokeAsync(HubEndpoint.SendMessage, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: Temp is {temperature}C");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(10000);
            }
        }

        private static async Task<(bool succeeded, string response)> SendTemperatureReading(
            HttpClient httpClient,
            Guid locationID,
            double temperature)
        {
            var reading = new Reading {
                LocationID = locationID,
                Temperature = (decimal)temperature
            };

            var content = new StringContent(JsonConvert.SerializeObject(reading), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://rpi-stat/home/addreading", content).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return (response.IsSuccessStatusCode, responseContent);
        }
    }
}
