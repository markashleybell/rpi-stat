using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading;

namespace rpi_stat
{
    public class ENER314
    {
        // 0: all sockets
        private readonly Dictionary<int, PinValue[]> _on = new Dictionary<int, PinValue[]> {
            { 0, new[] { PinValue.High, PinValue.Low, PinValue.High, PinValue.High } },
            { 1, new[] { PinValue.High, PinValue.High, PinValue.High, PinValue.High } },
            { 2, new[] { PinValue.High, PinValue.High, PinValue.High, PinValue.Low } },
            { 3, new[] { PinValue.High, PinValue.High, PinValue.Low, PinValue.High } },
            { 4, new[] { PinValue.High, PinValue.High, PinValue.Low, PinValue.Low } }
        };

        private readonly Dictionary<int, PinValue[]> _off = new Dictionary<int, PinValue[]> {
            { 0, new[] { PinValue.Low, PinValue.Low, PinValue.High, PinValue.High } },
            { 1, new[] { PinValue.Low, PinValue.High, PinValue.High, PinValue.High } },
            { 2, new[] { PinValue.Low, PinValue.High, PinValue.High, PinValue.Low } },
            { 3, new[] { PinValue.Low, PinValue.High, PinValue.Low, PinValue.High } },
            { 4, new[] { PinValue.Low, PinValue.High, PinValue.Low, PinValue.Low } }
        };

        private readonly int PIN_MODE_SELECT;

        private readonly int PIN_ENCODER_SIGNAL_D0;
        private readonly int PIN_ENCODER_SIGNAL_D1;
        private readonly int PIN_ENCODER_SIGNAL_D2;
        private readonly int PIN_ENCODER_SIGNAL_D3;

        private readonly int PIN_MODULATOR_ON_OFF;

        private readonly int[] PINS;

        private GpioController _gpioController;

        public const int LOGICAL_PIN_MODE_SELECT = 24;

        public const int LOGICAL_PIN_ENCODER_SIGNAL_D0 = 17;
        public const int LOGICAL_PIN_ENCODER_SIGNAL_D1 = 22;
        public const int LOGICAL_PIN_ENCODER_SIGNAL_D2 = 23;
        public const int LOGICAL_PIN_ENCODER_SIGNAL_D3 = 27;

        public const int LOGICAL_PIN_MODULATOR_ON_OFF = 25;

        public const int BOARD_PIN_MODE_SELECT = 18;

        public const int BOARD_PIN_ENCODER_SIGNAL_D0 = 11;
        public const int BOARD_PIN_ENCODER_SIGNAL_D1 = 15;
        public const int BOARD_PIN_ENCODER_SIGNAL_D2 = 16;
        public const int BOARD_PIN_ENCODER_SIGNAL_D3 = 13;

        public const int BOARD_PIN_MODULATOR_ON_OFF = 22;

        public ENER314(GpioController gpioController)
        {
            _gpioController = gpioController;

            if (_gpioController.NumberingScheme == PinNumberingScheme.Board)
            {
                PIN_MODE_SELECT = BOARD_PIN_MODE_SELECT;

                PIN_ENCODER_SIGNAL_D0 = BOARD_PIN_ENCODER_SIGNAL_D0;
                PIN_ENCODER_SIGNAL_D1 = BOARD_PIN_ENCODER_SIGNAL_D1;
                PIN_ENCODER_SIGNAL_D2 = BOARD_PIN_ENCODER_SIGNAL_D2;
                PIN_ENCODER_SIGNAL_D3 = BOARD_PIN_ENCODER_SIGNAL_D3;

                PIN_MODULATOR_ON_OFF = BOARD_PIN_MODULATOR_ON_OFF;
            }
            else
            {
                PIN_MODE_SELECT = LOGICAL_PIN_MODE_SELECT;

                PIN_ENCODER_SIGNAL_D0 = LOGICAL_PIN_ENCODER_SIGNAL_D0;
                PIN_ENCODER_SIGNAL_D1 = LOGICAL_PIN_ENCODER_SIGNAL_D1;
                PIN_ENCODER_SIGNAL_D2 = LOGICAL_PIN_ENCODER_SIGNAL_D2;
                PIN_ENCODER_SIGNAL_D3 = LOGICAL_PIN_ENCODER_SIGNAL_D3;

                PIN_MODULATOR_ON_OFF = LOGICAL_PIN_MODULATOR_ON_OFF;
            }

            PINS = new[] {
                PIN_ENCODER_SIGNAL_D3,
                PIN_ENCODER_SIGNAL_D2,
                PIN_ENCODER_SIGNAL_D1,
                PIN_ENCODER_SIGNAL_D0
            };

            // Pins for encoder K0-K3 data inputs
            _gpioController.OpenPin(PIN_ENCODER_SIGNAL_D0, PinMode.Output);
            _gpioController.OpenPin(PIN_ENCODER_SIGNAL_D1, PinMode.Output);
            _gpioController.OpenPin(PIN_ENCODER_SIGNAL_D2, PinMode.Output);
            _gpioController.OpenPin(PIN_ENCODER_SIGNAL_D3, PinMode.Output);

            // Pin for mode (Amplitude Shift Keying/Frequency Shift Keying)
            _gpioController.OpenPin(PIN_MODE_SELECT, PinMode.Output);

            // Pin for modulator enable/disable
            _gpioController.OpenPin(PIN_MODULATOR_ON_OFF, PinMode.Output);
        }

        public void On(int id) =>
            Command(id, _on);

        public void Off(int id) =>
            Command(id, _off);

        private void Command(int id, Dictionary<int, PinValue[]> data)
        {
            // Disable the modulator
            _gpioController.Write(PIN_MODULATOR_ON_OFF, PinValue.Low);

            // Select ASK (On Off Keying) mode
            _gpioController.Write(PIN_MODE_SELECT, PinValue.Low);

            // Initialise encoder inputs (0000)
            _gpioController.Write(PIN_ENCODER_SIGNAL_D0, PinValue.Low);
            _gpioController.Write(PIN_ENCODER_SIGNAL_D1, PinValue.Low);
            _gpioController.Write(PIN_ENCODER_SIGNAL_D2, PinValue.Low);
            _gpioController.Write(PIN_ENCODER_SIGNAL_D3, PinValue.Low);

            // Send the command to the specified socket
            for (var i = 0; i < 4; i++)
            {
                _gpioController.Write(PINS[i], data[id][i]);
            }

            // Encoder needs some time to 'settle'
            Thread.Sleep(100);

            // Enable the modulator for a period then disable again
            _gpioController.Write(PIN_MODULATOR_ON_OFF, PinValue.High);

            Thread.Sleep(250);

            _gpioController.Write(PIN_MODULATOR_ON_OFF, PinValue.Low);
        }

        public void Dispose()
        {
            _gpioController.ClosePin(PIN_ENCODER_SIGNAL_D0);
            _gpioController.ClosePin(PIN_ENCODER_SIGNAL_D1);
            _gpioController.ClosePin(PIN_ENCODER_SIGNAL_D2);
            _gpioController.ClosePin(PIN_ENCODER_SIGNAL_D3);
            _gpioController.ClosePin(PIN_MODE_SELECT);
            _gpioController.ClosePin(PIN_MODULATOR_ON_OFF);

            _gpioController?.Dispose();
            _gpioController = null;
        }
    }
}
