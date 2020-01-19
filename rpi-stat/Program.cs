using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using core;
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

            var httpClient = new HttpClient();

            var ok = true;

            while (ok)
            {
                var temperature = sensor.ReadTemperature().Celsius;

                var (succeeded, response) = await SendTemperatureReading(httpClient, locationID, temperature);

                Console.WriteLine(response);

                if (!succeeded)
                {
                    ok = false;
                    break;
                }

                Thread.Sleep(10000);
            }

            return ok ? 0 : -1;
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
