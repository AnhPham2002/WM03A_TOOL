using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class SerialPortManager
    {
        private SerialPort _serialPort;

        public bool IsOpen
        {
            get
            {
                return _serialPort != null && _serialPort.IsOpen;
            }
        }

        public void Open(string portName)
        {
            _serialPort = new SerialPort(
                portName,
                115200,
                Parity.None,
                8,
                StopBits.One);

            _serialPort.Open();
        }

        public void Close()
        {
            if (_serialPort == null)
            {
                return;
            }

            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Dispose();
            _serialPort = null;
        }

        public void Send(byte[] data)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("COM chưa được mở.");
            }

            _serialPort.Write(data, 0, data.Length);
        }
    }
}
