﻿# Unwired Modbus

Open-source **Modbus Communicator** generated by `Leandro Luiz G. Cavalheiro`. Developed to facilitate communication between .Net applications and ModBus TCP/IP devices.
Developed in .Net 7.

- 👉 [Nuget Package](https://www.nuget.org/packages/Unwired.ModBus.Tcp) - `nuget page`

<br />

> Product Roadmap

| Status | Item                                         | info            |
| ------ | -------------------------------------------- | --------------- |
|   ✅   | **FC01 FC02 FC03 FC04 FC05 FC06 FC15 FC016** | Basic Functions |
|   ✅   | **Async Basic Functions**                    |                 |
|   ❌   | **FC23 Sync/Async Tasks**                    |                 |

> Something is missing? Submit a new `product feature request` using the [issues tracker](https://github.com/leandrocavalheiro/unwired.modbus/issues).

<br />

## ✨ Using the library

> 👉 **Step 1** - Install library into project

- **Package Manager**

```bash
$ Install-Package Unwired.ModBus.Tcp
```

- **.Net CLI**

```bash
$ dotnet add package Unwired.ModBus.Tcp
```

<br />

> 👉 **Step 2** - instantiate the client

```bash
var myclient = new UnwiredModBusClient();
var (successConnect, errorCodeConnect, errorConnect) = myclient.ConnectDevice("127.0.0.1", 502, 0x01, swapType: SwapTypeEnum.SwapWordsAndBytes, deviceAlias: "MyModbus");
if (!successConnect)
{
    Console.WriteLine($"Error: {errorCodeConnect} - {errorConnect}");
    return;
}

var (success, result, errorCode, error) = await _unwiredModBusClient.ReadCoilsAsync(0, 5);
if (!success)
{
    Console.WriteLine($"Error: {errorCode} - {error}");
    return;
}


var valueCoil = result[0];
```

## ✨ Code-base structure

The project is coded using a simple and intuitive structure presented below:

```bash
< PROJECT ROOT >
   |
   |-- Unwired.ModBus.Tcp/                 # Library
   |    |-- Configurations/                # Configurations to use Dependency Injection
   |    |-- Enumarators/                   # All enumarators used in the library
   |    |-- Exceptions/                    # Definitions of exceptions classes
   |    |-- Extensions/                    # Extensions methods
   |    |-- Implementations/               # Implementations of contracts
   |    |-- Interfaces/                    # Contracts library
   |    |-- Methods/                       # Generic methods
   |-- Unwired.ModBus.Tcp.Test/            # WorkService to tests
```

## ✨ Contacts

> 📧 **Email** - leo.cavalheiro.ti@gmail.com
