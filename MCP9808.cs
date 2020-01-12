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

            /*
            Chip specs & reference:
            http://ww1.microchip.com/downloads/en/DeviceDoc/25095A.pdf (Page 24-25 is the relevant section)
            https://www.cs.cornell.edu/~tomf/notes/cps104/twoscomp.html

            Code ported from these sources:
            https://github.com/adafruit/Adafruit_CircuitPython_MCP9808/blob/master/adafruit_mcp9808.py#L100
            https://github.com/adafruit/Adafruit_MCP9808_Library/blob/master/Adafruit_MCP9808.cpp

            So at this point, readBuffer contains the 16 bits from the 2-byte ambient temperature register.

            The first three bits of the upper byte represent the alert pin state (we're ignoring it for now).

            The next bit is the sign bit: 0 for temps > 0C, 1 for < 0C.

            The remaining 4 bits of the upper byte combined with the 8 from the lower byte make up the
            temperature value, in two's compliment format.
            */

            // Bit mask for the first 3 bits (alert state) of the upper byte
            // This is just 0x1F, but the binary literal makes this much easier to visualise
            const int alertStateMask = 0b00011111;

            // Bit mask for the sign bit of the upper byte
            const int signMask = 0b00010000;

            // Logical ANDing the upper byte with the mask gives us the
            // temperature value bytes without the alert state values
            var upper = readBuffer[0] & alertStateMask;
            var lower = readBuffer[1];

            double temperature = -1;

            // From EXAMPLE 5-1 in chip documentation
            static double convertToDecimal(int u, int l) => (u * 16) + (l / 16.0);

            // The temperature bits are in two’s compliment format, therefore positive
            // and negative temperature values need to be computed differently.

            // If the sign bit of the upper byte is 1, we're looking at a negative number
            if ((upper & signMask) == 0x10)
            {
                // Clear the sign bit, so we're just left with numeric bits
                upper &= 0x0f;

                temperature = 256 - convertToDecimal(upper, lower);
            }
            else
            {
                temperature = convertToDecimal(upper, lower);
            }

            return Temperature.FromCelsius(temperature);
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
