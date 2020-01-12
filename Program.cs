using System;
using System.Device.I2c;
using System.Threading;

namespace rpi_stat
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var settings = new I2cConnectionSettings(1, MCP9808.MCP9808_I2CADDR_DEFAULT);
            var i2cDevice = I2cDevice.Create(settings);

            var sensor = new MCP9808(i2cDevice);

            if (sensor.Test())
            {
                Console.WriteLine("Sensor detected");
            }

            while (true)
            {
                Console.WriteLine(sensor.ReadTemperature().Celsius);

                Thread.Sleep(500);
            }
        }
    }
}
