using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace rpi_stat
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            //var settings = new I2cConnectionSettings(1, MCP9808.MCP9808_I2CADDR_DEFAULT);
            //var i2cDevice = I2cDevice.Create(settings);

            //var sensor = new MCP9808(i2cDevice);

            //if (sensor.Test())
            //{
            //    Console.WriteLine("Sensor detected");
            //}

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

            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://rpi-stat/home/test");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseContent);
            }
        }
    }
}
