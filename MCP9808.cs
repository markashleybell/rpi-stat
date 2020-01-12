using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;
using Iot.Units;

namespace rpi_stat
{
    /// <summary>
    /// Microchip MCP9808 I2C Temperature Sensor
    /// </summary>
    public sealed class MCP9808 : IDisposable
    {
        private I2cDevice _i2cDevice;

        // Default I2C address
        public const byte MCP9808_I2CADDR_DEFAULT = 0x18;

        // Manufacturer and device identifiers
        public const ushort MCP9808_MANUF_ID = 0x0054;
        public const ushort MCP9808_DEVICE_ID = 0x0400;

        // Register addresses
        public const byte MCP9808_REG_CONFIG = 0x01;
        public const byte MCP9808_REG_UPPER_TEMP = 0x02;
        public const byte MCP9808_REG_LOWER_TEMP = 0x03;
        public const byte MCP9808_REG_CRIT_TEMP = 0x04;
        public const byte MCP9808_REG_AMBIENT_TEMP = 0x05;
        public const byte MCP9808_REG_MANUF_ID = 0x06;
        public const byte MCP9808_REG_DEVICE_ID = 0x07;

        // Configuration register values
        // TODO: should this be defined as a short?
        public const byte MCP9808_REG_CONFIG_SHUTDOWN = unchecked((byte)0x0100);
        public const byte MCP9808_REG_CONFIG_CRITLOCKED = 0x0080;
        public const byte MCP9808_REG_CONFIG_WINLOCKED = 0x0040;
        public const byte MCP9808_REG_CONFIG_INTCLR = 0x0020;
        public const byte MCP9808_REG_CONFIG_ALERTSTAT = 0x0010;
        public const byte MCP9808_REG_CONFIG_ALERTCTRL = 0x0008;
        public const byte MCP9808_REG_CONFIG_ALERTSEL = 0x0002;
        public const byte MCP9808_REG_CONFIG_ALERTPOL = 0x0002;
        public const byte MCP9808_REG_CONFIG_ALERTMODE = 0x0001;

        public MCP9808(I2cDevice i2cDevice) =>
            _i2cDevice = i2cDevice;

        public bool Test()
        {
            var manufacturerID = ReadRegister(MCP9808_REG_MANUF_ID);
            var deviceID = ReadRegister(MCP9808_REG_DEVICE_ID);

            return manufacturerID == MCP9808_MANUF_ID
                && deviceID == MCP9808_DEVICE_ID;
        }

        public Temperature ReadTemperature()
        {
            Span<byte> writeBuffer = stackalloc byte[] { MCP9808_REG_AMBIENT_TEMP };
            Span<byte> readBuffer = stackalloc byte[2];

            _i2cDevice.WriteRead(writeBuffer, readBuffer);

            // http://ww1.microchip.com/downloads/en/DeviceDoc/25095A.pdf

            // Code ported from these sources:
            // https://github.com/adafruit/Adafruit_CircuitPython_MCP9808/blob/master/adafruit_mcp9808.py#L100
            // https://github.com/adafruit/Adafruit_MCP9808_Library/blob/master/Adafruit_MCP9808.cpp

            // Mask off the first 3 bits of the upper byte; these contain
            var upperByte = readBuffer[0] & 0b00011111; // 0x1F;
            var lowerByte = readBuffer[1];

            if ((upperByte & 0x10) == 0x10)
            {
                upperByte &= 0x0f;

                var temp1 = (upperByte * 16) + (lowerByte / 16.0) - 256;

                return Temperature.FromCelsius(temp1);
            }

            var temp2 = (upperByte * 16) + (lowerByte / 16.0);

            return Temperature.FromCelsius(temp2);
        }

        public void Dispose()
        {
            _i2cDevice?.Dispose();
            _i2cDevice = null;
        }

        private ushort ReadRegister(byte register)
        {
            Span<byte> writeBuffer = stackalloc byte[] { register };
            Span<byte> readBuffer = stackalloc byte[2];

            _i2cDevice.WriteRead(writeBuffer, readBuffer);

            return BinaryPrimitives.ReadUInt16BigEndian(readBuffer);
        }
    }
}
