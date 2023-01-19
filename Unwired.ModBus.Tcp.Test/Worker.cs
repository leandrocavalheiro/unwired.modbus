using System.Net;
using Unwired.ModBus.Tcp.Enumarators;
using Unwired.ModBus.Tcp.Implementations;
using Unwired.ModBus.Tcp.Interfaces;

namespace Unwired.ModBus.Tcp.Test
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IUnwiredModBusClient _unwiredModBusClient;

        public Worker(ILogger<Worker> logger, IUnwiredModBusClient unwiredModBusClient)
        {
            _logger = logger;
            _unwiredModBusClient = unwiredModBusClient;
        }


        private void WriteText(string text, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Write(text, newLine);
        }

        private void WriteError(string text, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Write(text, newLine);
        }

        private void WriteSuccess(string text, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Write(text, newLine);
        }

        private void WritePrimary(string text, bool newLine = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Write(text, newLine);
        }

        private void Write(string text, bool newLine = false)
        {
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            if (newLine)
                Console.WriteLine();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int numberOfRegisters = 3;

            Dictionary<int, int> readCoils = new Dictionary<int, int>();
            Dictionary<int, int> readInputs = new Dictionary<int, int>();
            Dictionary<int, int> readInputsRegisters = new Dictionary<int, int>();
            Dictionary<int, int> readInputHoldings = new Dictionary<int, int>();

            Dictionary<int, bool> writeSingleCoils = new Dictionary<int, bool>();
            Dictionary<int, ushort> writeSingleByte = new Dictionary<int, ushort>();
            Dictionary<int, float> writeSingleWord = new Dictionary<int, float>();
            Dictionary<int, double> writeSingleDWord = new Dictionary<int, double>();

            Dictionary<int, bool[]> writeMultiplesCoils = new Dictionary<int, bool[]>();
            Dictionary<int, ushort> writeMultiplesByte = new Dictionary<int, ushort>();
            Dictionary<int, float> writeMultiplesWord = new Dictionary<int, float>();
            Dictionary<int, double> writeMultiplesDWord = new Dictionary<int, double>();

            readCoils.Add(0, 10);
            writeSingleCoils.Add(0, true);

            writeMultiplesCoils.Add(4, new bool[numberOfRegisters] { true, true, true});

            var myclient = new UnwiredModBusClient();
            var (successConnect, errorCodeConnect, errorConnect) = myclient.ConnectDevice("127.0.0.1", 502, 0x01, swapType: SwapTypeEnum.SwapWordsAndBytes, deviceAlias: "MyModbus");
            if (!successConnect)
            {
                WriteError($"Error: {errorCodeConnect} - {errorConnect}", true);
                return;
            }

            WriteText($"Test#1: Write/Read Single Coil", true);
            var currentValue = await myclient.ReadCoilsAsync(writeSingleCoils.FirstOrDefault().Key, 1);
            WriteText($"#Address: {writeSingleCoils.FirstOrDefault().Key}|Old Value: ");
            WritePrimary($"{currentValue.results[0]}", true);

            _ = await myclient.WriteSingleCoilsAsync(writeSingleCoils.FirstOrDefault().Key, writeSingleCoils.FirstOrDefault().Value);
            currentValue = await myclient.ReadCoilsAsync(writeSingleCoils.FirstOrDefault().Key, 1);

            WriteText($"#Address: {writeSingleCoils.FirstOrDefault().Key}|New Value: ");
            WriteSuccess($"{currentValue.results[0]}", true);

            WriteText($"", true);
            WriteText($"Test#2: Write/Read Multiples Coil", true);
            currentValue = await myclient.ReadCoilsAsync(writeMultiplesCoils.FirstOrDefault().Key, numberOfRegisters);
            var currentAddress = writeMultiplesCoils.FirstOrDefault().Key;
            foreach (var item in currentValue.results)
            {
                WriteText($"#Address: {currentAddress}|Old Value: ");
                WritePrimary($"{item}", true);
                currentAddress++;
            }
            _ = await myclient.WriteMultiplesCoilsAsync(writeMultiplesCoils.FirstOrDefault().Key, writeMultiplesCoils.FirstOrDefault().Value);

            currentValue = await myclient.ReadCoilsAsync(writeMultiplesCoils.FirstOrDefault().Key, numberOfRegisters);
            currentAddress = writeMultiplesCoils.FirstOrDefault().Key;
            foreach (var item in currentValue.results)
            {
                WriteText($"#Address: {currentAddress}|New Value: ");
                WritePrimary($"{item}", true);
                currentAddress++;
            }

        }
    }
}