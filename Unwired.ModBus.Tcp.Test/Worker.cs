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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/

            var (success, errorCode, error) = _unwiredModBusClient.ConnectDevice("127.0.0.1", 502, 0x01, swapType: SwapTypeEnum.SwapWordsAndBytes);
            if (!success)
            {
                Console.WriteLine($"Set Device Error: {error}");                
            }

            var fc04 = _unwiredModBusClient.ReadHoldings<double>(2, 2, ValueTypeEnum.Word, RoundTypeEnum.Floor);
            if (fc04.sucess && fc04.results != null)
             {
                 int address = 0;
                 Console.WriteLine($"FC04");
                 foreach (var item in fc04.results)
                 {
                     Console.WriteLine($"Address {address}: {item}");
                     address++;
                 }
             }
             



            //var res = _unwiredModBusClient.WriteSingleHolding(0, 5253, Enumarators.ValueTypeEnum.Byte);
            //    var res1 = _unwiredModBusClient.WriteSingleHolding(0, 89.90288f, Enumarators.ValueTypeEnum.Word);

         //   var res2 = _unwiredModBusClient.WriteMultiplesHolding(2, new float[] { 89.90288f, 89.90288f }, Enumarators.ValueTypeEnum.Word);


            /*var fc03 = _unwiredModBusClient.ReadHoldings<float>(0, 10);





            var fc04 = _unwiredModBusClient.FC04(0, 10);
            var fc05 = _unwiredModBusClient.FC05(0, true);
            
            var fc15 = _unwiredModBusClient.FC15(5, new bool[] {true, false, true, true});*/
            


            Console.WriteLine($"Result");
           /* if (fc03.sucess && fc03.results != null)
            {
                int address = 0;
                Console.WriteLine($"FC03");
                foreach (var item in fc03.results)
                {
                    Console.WriteLine($"Address {address}: {item}");
                    address++;
                }
            }
            */


            //Console.WriteLine($"Result: {result.error}");

            /*if (fc01.sucess && fc01.results != null)
            {
                int address = 0;
                Console.WriteLine($"FC01");
                foreach (var item in fc01.results)
                {
                    Console.WriteLine($"Address {address}: {item}");
                    address++;
                }
            }

            if (fc02.sucess && fc02.results != null)
            {
                int address = 0;
                Console.WriteLine($"FC02");
                foreach (var item in fc02.results)
                {
                    Console.WriteLine($"Address {address}: {item}");
                    address++;
                }
            }

            if (fc03.sucess && fc03.results != null)
            {
                int address = 0;
                Console.WriteLine($"FC03");
                foreach (var item in fc03.results)
                {
                    Console.WriteLine($"Address {address}: {item}");
                    address++;
                }
            }

            if (fc04.sucess && fc04.results != null)
            {
                int address = 0;
                Console.WriteLine($"FC04");
                foreach (var item in fc04.results)
                {
                    Console.WriteLine($"Address {address}: {item}");
                    address++;
                }
            }*/

            //var sucess = fc01.sucess ? "Sucess" : "Fail";
            //Console.WriteLine($"FC01: {sucess}");

            //sucess = fc02.sucess ? "Sucess" : "Fail";
            //Console.WriteLine($"FC02: {sucess}");

            /*var sucess = fc03.sucess ? "Sucess" : "Fail";
            Console.WriteLine($"FC03: {sucess}");

            sucess = fc04.sucess ? "Sucess" : "Fail";
            Console.WriteLine($"FC04: {sucess}");

            sucess = fc05.sucess ? "Sucess" : "Fail";
            Console.WriteLine($"FC05: {sucess}");



            sucess = fc15.sucess ? "Sucess" : "Fail";
            Console.WriteLine($"FC15: {sucess}");
            */



            
                /*if (coil.sucess)
                {
                    int address = 0;
                    Console.WriteLine($"Sucess!!!");
                    foreach (var item in coil.results)
                    {
                        Console.WriteLine($"Address {address}: {item}");
                        address++;
                    }

                    foreach (var item in register.results)
                    {
                        Console.WriteLine($"Address {address}: {item}");
                        address++;
                    }



                }
                else
                {
                    Console.WriteLine($"Fail!!!");
                    Console.WriteLine($"ErrorCode: {coil.errorCode}");
                    Console.WriteLine($"Error: {coil.error}");
                }*/
            }
    }
}