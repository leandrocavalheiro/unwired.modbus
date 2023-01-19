using System.Net;
using System.Net.Sockets;
using Unwired.ModBus.Tcp.Interfaces;
using Unwired.ModBus.Tcp.Extensions;
using Unwired.ModBus.Tcp.Enumarators;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unwired.ModBus.Tcp.Implementations;

public class UnwiredModBusClient : IUnwiredModBusClient, IDisposable
{    
    private bool _connected = false;
    private string _ip = "127.0.0.1";
    private string _deviceAlias = "127.0.0.1";
    private ushort _port = 502;
    private ushort _timeout = 5000;    
    private uint _transactionId = 0;
    private byte[] _protocolIdentifier;
    private byte[] _messageLength = new byte[2];
    private byte _unitIdentifier;
    private const ushort _trueBytes = 0xFF00;
    private const ushort _falseBytes = 0x0000;
    IPAddress _ipAddress;
    IPEndPoint _modbusServerEP;
    SwapTypeEnum _swapType;

    private Socket _socket;    



    public UnwiredModBusClient()
    {
        _ipAddress = IPAddress.Parse(_ip);
        _modbusServerEP = new IPEndPoint(_ipAddress, _port);
        _protocolIdentifier = 0.ToByteArray();
        _messageLength = 6.ToByteArray();
        //_trueBytes = 65280.ToByteArray();
        //_falseBytes = 0.ToByteArray();
        _socket = InitializeSocket();
    }

    public (bool success, string? errorCode, string? error) ConnectDevice(string ip = "127.0.0.1", ushort port = 502, byte unitIdentifier = 1, ushort timeout = 5000, SwapTypeEnum swapType = SwapTypeEnum.NoSwap, string deviceAlias = "127.0.0.1")
    {
        try
        {
            _ip = ip;
            _port = port;
            _timeout = timeout;
            _unitIdentifier = unitIdentifier;
            _ipAddress = IPAddress.Parse(ip);
            _socket = InitializeSocket();
            _swapType = swapType;
            _connected = false;
            _deviceAlias = deviceAlias;




            _socket.ReceiveTimeout = _timeout;
            _socket.SendTimeout = _timeout;

            var result = _socket.BeginConnect(_ipAddress, (int)port, null, null);
            _ = result.AsyncWaitHandle.WaitOne(_timeout, true);

            
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReceiveBuffer, 0);
            //_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);


            if (!_socket.Connected)
                return (_connected, "ConnectionFailed", $"Failed to connect device {deviceAlias}.");

            _connected = _socket.Connected;
            return (true, null, null);
        }
        catch (Exception error)
        {
            _connected = false;
            return (false, "ConnectionFailed", $"Device {deviceAlias}: {error.Message}");
        }
    }    
    public bool Connected()
        => _connected;
    public (bool sucess, bool[]? results, string? errorCode, string? error) ReadCoils(int startingAddress, int totalAddresses = 1)
    {        
        var result = ReadFromDevice(FunctionEnum.FC01, startingAddress, totalAddresses);
        if (!result.sucess)
            return (false, null, result.errorCode, result.error);

        if (result.results == null)
            return (true, null, null, null);

        var response = new bool[totalAddresses];
        for (int counter = 0; counter < totalAddresses; counter++)
        {
            int intData = result.results[9 + counter / 8];
            var mask = Math.Pow(2, (counter % 8)).ToInt();
            response[counter] = Convert.ToBoolean((intData & mask) / mask);
        }

        return (true, response, null, null);
    }
    public async Task<(bool sucess, bool[]? results, string? errorCode, string? error)> ReadCoilsAsync(int startingAddress, int totalAddresses = 1)
    {
        var result = await ReadFromDeviceAsync(FunctionEnum.FC01, startingAddress, totalAddresses);
        if (!result.sucess)
            return (false, null, result.errorCode, result.error);

        if (result.results == null)
            return (true, null, null, null);

        var response = new bool[totalAddresses];
        for (int counter = 0; counter < totalAddresses; counter++)
        {
            int intData = result.results[9 + counter / 8];
            var mask = Math.Pow(2, (counter % 8)).ToInt();
            response[counter] = Convert.ToBoolean((intData & mask) / mask);
        }

        return (true, response, null, null);
    }
    public (bool sucess, bool[]? results, string? errorCode, string? error) ReadInputs(int startingAddress, int totalAddresses = 1)
    {

        var result = ReadFromDevice(FunctionEnum.FC02, startingAddress, totalAddresses);
        if (!result.sucess)
            return (false, null, result.errorCode, result.error);

        if (result.results == null)
            return (true, null, null, null);

        var response = new bool[totalAddresses];
        for (int counter = 0; counter < totalAddresses; counter++)
        {
            int intData = result.results[9 + counter / 8];
            var mask = Math.Pow(2, (counter % 8)).ToInt();
            response[counter] = Convert.ToBoolean((intData & mask) / mask);
        }

        return (true, response, null, null);
    }
    public async Task<(bool sucess, bool[]? results, string? errorCode, string? error)> ReadInputsAsync(int startingAddress, int totalAddresses = 1)
    {
        var (sucess, results, errorCode, error) = await ReadFromDeviceAsync(FunctionEnum.FC02, startingAddress, totalAddresses);
        if (!sucess)
            return (false, null, errorCode, error);

        if (results == null)
            return (true, null, null, null);

        var response = new bool[totalAddresses];
        for (int counter = 0; counter < totalAddresses; counter++)
        {
            int intData = results[9 + counter / 8];
            var mask = Math.Pow(2, (counter % 8)).ToInt();
            response[counter] = Convert.ToBoolean((intData & mask) / mask);
        }

        return (true, response, null, null);
    }
    public (bool sucess, TResult[]? results, string? errorCode, string? error) ReadHoldings<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 2) where TResult : INumber<TResult>
    {       
        var (sucess, results, errorCode, error) = ReadFromDevice(FunctionEnum.FC03, startingAddress, totalAddresses *= (int)valueType);
        if (!sucess)
            return (false, null, errorCode, error);

        if (results == null)
            return (true, null, null, null);

        return (true, results.ConvertResult<TResult>(totalAddresses, valueType, _swapType, roundType, precision), null, null);
    }
    public async Task<(bool sucess, TResult[]? results, string? errorCode, string? error)> ReadHoldingsAsync<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 2) where TResult : INumber<TResult>
    {
        var (sucess, results, errorCode, error) = await ReadFromDeviceAsync(FunctionEnum.FC03, startingAddress, totalAddresses *= (int)valueType);
        if (!sucess)
            return (false, null, errorCode, error);

        if (results == null)
            return (true, null, null, null);

        return (true, results.ConvertResult<TResult>(totalAddresses, valueType, _swapType, roundType, precision), null, null);
    }
    public (bool sucess, TResult[]? results, string? errorCode, string? error) ReadInputRegisters<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 2) where TResult : INumber<TResult>
    {
        var (sucess, results, errorCode, error) = ReadFromDevice(FunctionEnum.FC04, startingAddress, totalAddresses *= (int)valueType);
        if (!sucess)
            return (false, null, errorCode, error);

        if (results == null)
            return (true, null, null, null);

        return (true, results.ConvertResult<TResult>(totalAddresses, valueType, _swapType, roundType, precision), null, null);
    }
    public async Task<(bool sucess, TResult[]? results, string? errorCode, string? error)> ReadInputRegistersAsync<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 2) where TResult : INumber<TResult>
    {
        var (sucess, results, errorCode, error) = await ReadFromDeviceAsync(FunctionEnum.FC04, startingAddress, totalAddresses *= (int)valueType);
        if (!sucess)
            return (false, null, errorCode, error);

        if (results == null)
            return (true, null, null, null);

        return (true, results.ConvertResult<TResult>(totalAddresses, valueType, _swapType, roundType, precision), null, null);
    }
    public (bool sucess, string? errorCode, string? error) WriteSingleCoils(int address, bool value)
    {
        var (sucess, errorCode, error) = WriteSingleInDevice(FunctionEnum.FC05, address, value);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public async Task<(bool sucess, string? errorCode, string? error)> WriteSingleCoilsAsync(int address, bool value)
    {
        var (sucess, errorCode, error) = await WriteSingleInDeviceAsync(FunctionEnum.FC05, address, value);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public (bool sucess, string? errorCode, string? error) WriteSingleHolding<TValueType>(int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        var (sucess, errorCode, error) = WriteSingleInDevice(FunctionEnum.FC06, address, value, valueType);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public async Task<(bool sucess, string? errorCode, string? error)> WriteSingleHoldingAsync<TValueType>(int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        var (sucess, errorCode, error) = await WriteSingleInDeviceAsync(FunctionEnum.FC06, address, value, valueType);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public (bool sucess, string? errorCode, string? error) WriteMultiplesCoils(int startingAddress, bool[] values)
    {
        var (sucess, errorCode, error) = WriteMultiplesInDevice(FunctionEnum.FC15, startingAddress, values);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public async Task<(bool sucess, string? errorCode, string? error)> WriteMultiplesCoilsAsync(int startingAddress, bool[] values)
    {
        var (sucess, errorCode, error) = await WriteMultiplesInDeviceAsync(FunctionEnum.FC15, startingAddress, values);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public (bool sucess, string? errorCode, string? error) WriteMultiplesHolding<TValueType>(int startingAddress, TValueType[] value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        var (sucess, errorCode, error) = WriteMultiplesInDevice(FunctionEnum.FC16, startingAddress, value, valueType);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }
    public async Task<(bool sucess, string? errorCode, string? error)> WriteMultiplesHoldingAsync<TValueType>(int startingAddress, TValueType[] value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        var (sucess, errorCode, error) = await WriteMultiplesInDeviceAsync(FunctionEnum.FC16, startingAddress, value, valueType);
        if (!sucess)
            return (false, errorCode, error);

        return (true, null, null);
    }


    private async Task<(bool sucess, byte[]? results, string? errorCode, string? error)> ComunicateAsync(byte[] data)
    {
        var result = new byte[1024];
        try
        {
            //TODO Tratar para não criar uma nova conexão a cada request
            //Deixei dessa forma, pois estoura erro ao fazer várias request sem recriar o objeto
            _socket = InitializeSocket();
            _connected = false;
            var connection = _socket.BeginConnect(_ipAddress, (int)_port, null, null);
            _ = connection.AsyncWaitHandle.WaitOne(_timeout, true);
            _connected = _socket.Connected;


            _ = await _socket.SendAsync(data);
            var res = await _socket.ReceiveAsync(result, SocketFlags.None);
        }
        catch (Exception exception)
        {
            return (false, null, "CommunicationFailed", exception.Message);
        }

        if (result[0] != 0x81)
            return (true, result, null, null);

        var error = GetMessageError(result[1]);
        return (false, null, error.errorCode, error.error);
    }
    private (bool sucess, byte[]? results, string? errorCode, string? error) Comunicate(byte[] data)
    {
        var result = new byte[1024];
        try
        {
            //TODO Tratar para não criar uma nova conexão a cada request
            //Deixei dessa forma, pois estoura erro ao fazer várias request sem recriar o objeto
            _socket = InitializeSocket();
            _connected = false;
            var connection = _socket.BeginConnect(_ipAddress, (int)_port, null, null);
            _ = connection.AsyncWaitHandle.WaitOne(_timeout, true);
            _connected = _socket.Connected;


            _ = _socket.Send(data);
            var res = _socket.Receive(result, SocketFlags.None);
        }
        catch (Exception exception)
        {
            return (false, null, "CommunicationFailed", exception.Message);
        }

        if (result[0] != 0x81)
            return (true, result, null, null);

        var error = GetMessageError(result[1]);
        return (false, null, error.errorCode, error.error);
    }
    private (string errorCode, string error) GetMessageError(byte error)
    {
        switch (error)
        {

            case 0x01:
                return ("IllegalFunction", "Function code not supported.");
            case 0x02:
                return ("IllegalDataAddress", "The data address received by the slave is not an authorized address for the device.");
            case 0x03:
                return ("IllegalDataValue", "The value in the request data field is not an authorized value for the device.");
            case 0x04:
                return ("DeviceFailure", "The device fails to perform a requested action because of an unrecoverable error.");
            case 0x05:
                return ("Acknowledge", "The device accepts the request but needs a long time to process it.");
            case 0x06:
                return ("SlaveDeviceBusy", "The slave is busy processing another command. The master must send the request once the device is available.");
            case 0x07:
                return ("NegativeAcknowledgment", "The device cannot perform the programming request sent by the master.");
            case 0x08:
                return ("MemoryParityError", "The device detects a parity error in the memory when attempting to read extended memory.");
            case 0x0A:
                return ("GatewayPathUnavailable", "The gateway is overloaded or not correctly configured.");
            case 0x0B:
                return ("GatewayTargetDeviceFailedToRespond", "The device is not present on the network.");
            default:
                return ("UnknownError", "Unknown error.");
        }
    }    
    
    private (bool sucess, byte[]? results, string? errorCode, string? error) ReadFromDevice(FunctionEnum function, int startingAddress, int totalAddresses = 1)
    {
        if (startingAddress > 65535 | totalAddresses > 2000)
            return (false, null, "ArgumentException", "Starting address must be 0 - 65535; quantity must be 0 - 2000");

        if (!_connected)
            return (false, null, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");



        //TODO tratar conexão fechada


        _transactionId++;

        var transactionIdentifier = _transactionId.ToByteArray();
        var startAt = startingAddress.ToByteArray();
        var quantity = totalAddresses.ToByteArray();

        var data = new byte[]{
                            transactionIdentifier[1],
                            transactionIdentifier[0],
                            _protocolIdentifier[1],
                            _protocolIdentifier[0],
                            _messageLength[1],
                            _messageLength[0],
                            _unitIdentifier,
                            ((int)function).ToByte(),
                            startAt[1],
                            startAt[0],
                            quantity[1],
                            quantity[0]
        };

        return Comunicate(data);        
    }
    private async Task<(bool sucess, byte[]? results, string? errorCode, string? error)> ReadFromDeviceAsync(FunctionEnum function, int startingAddress, int totalAddresses = 1)
    {
        if (startingAddress > 65535 | totalAddresses > 2000)
            return (false, null, "ArgumentException", "Starting address must be 0 - 65535; quantity must be 0 - 2000");

        if (!_connected)
            return (false, null, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");

        //TODO tratar conexão fechada

        _transactionId++;

        var transactionIdentifier = _transactionId.ToByteArray();
        var startAt = startingAddress.ToByteArray();
        var quantity = totalAddresses.ToByteArray();

        var data = new byte[]{
                            transactionIdentifier[1],
                            transactionIdentifier[0],
                            _protocolIdentifier[1],
                            _protocolIdentifier[0],
                            _messageLength[1],
                            _messageLength[0],
                            _unitIdentifier,
                            ((int)function).ToByte(),
                            startAt[1],
                            startAt[0],
                            quantity[1],
                            quantity[0]
        };

        return await ComunicateAsync(data);        
    }  
    private (bool sucess, string? errorCode, string? error) WriteSingleInDevice<TValueType>(FunctionEnum function, int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        if (address > 65535)
            return (false, "ArgumentException", "Starting address must be 0 - 65535");

        if (!_connected)
            return (false, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");

        _transactionId++;

        var valueToSend = new byte[2];
        var transactionIdentifier = _transactionId.ToByteArray();
        var startAt = address.ToByteArray();

        if (function == FunctionEnum.FC05)
            valueToSend = value.ToBool() ? _trueBytes.ToByteArray(ValueTypeEnum.Byte, SwapTypeEnum.SwapBytes) : _falseBytes.ToByteArray(ValueTypeEnum.Byte, SwapTypeEnum.SwapBytes);        

        if (function == FunctionEnum.FC06)
        {
            if (value is null)
                return (false, "ValueInvalid", "Value is required.");


            if (valueType != ValueTypeEnum.Byte)            
                return WriteMultiplesInDevice(FunctionEnum.FC16, address, new TValueType[] { value }, valueType);
           
          
            valueToSend = value.ToByteArray(valueType, SwapTypeEnum.SwapBytes);
        }

        var data = new byte[]{  transactionIdentifier[1],
                            transactionIdentifier[0],
                            _protocolIdentifier[1],
                            _protocolIdentifier[0],
                            _messageLength[1],
                            _messageLength[0],
                            _unitIdentifier,
                            ((int)function).ToByte(),
                            startAt[1],
                            startAt[0],
                            valueToSend[0], //A inversão dos bytes é feita no FormatToSend
                            valueToSend[1] //A inversão dos bytes é feita no FormatToSend
                            };


        var (sucess, _, errorCode, error) = Comunicate(data);
        return (sucess, errorCode, error);
    }
    private async Task<(bool sucess, string? errorCode, string? error)> WriteSingleInDeviceAsync<TValueType>(FunctionEnum function, int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        if (address > 65535)
            return (false, "ArgumentException", "Starting address must be 0 - 65535");

        if (!_connected)
            return (false, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");

        _transactionId++;

        var valueToSend = new byte[2];
        var transactionIdentifier = _transactionId.ToByteArray();
        var startAt = address.ToByteArray();

        if (function == FunctionEnum.FC05)
            valueToSend = value.ToBool() ? _trueBytes.ToByteArray(ValueTypeEnum.Byte, SwapTypeEnum.SwapBytes) : _falseBytes.ToByteArray(ValueTypeEnum.Byte, SwapTypeEnum.SwapBytes);

        if (function == FunctionEnum.FC06)
        {
            if (value is null)
                return (false, "ValueInvalid", "Value is required.");


            if (valueType != ValueTypeEnum.Byte)
                return await WriteMultiplesInDeviceAsync(FunctionEnum.FC16, address, new TValueType[] { value }, valueType);


            valueToSend = value.ToByteArray(valueType, SwapTypeEnum.SwapBytes);
        }

        var data = new byte[]{  transactionIdentifier[1],
                            transactionIdentifier[0],
                            _protocolIdentifier[1],
                            _protocolIdentifier[0],
                            _messageLength[1],
                            _messageLength[0],
                            _unitIdentifier,
                            ((int)function).ToByte(),
                            startAt[1],
                            startAt[0],
                            valueToSend[0], //A inversão dos bytes é feita no FormatToSend
                            valueToSend[1] //A inversão dos bytes é feita no FormatToSend
                            };

        var (sucess, _, errorCode, error) = await ComunicateAsync(data);
        return (sucess, errorCode, error);
    }
    private (bool sucess, string? errorCode, string? error) WriteMultiplesInDevice<TValueType>(FunctionEnum function, int startingAddress, TValueType[] values, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        if (!_connected)
            return (false, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");

        _transactionId++;

        var valueLength = values.Length;
        if (valueType == ValueTypeEnum.Word)
            valueLength *= 2;

        if (valueType == ValueTypeEnum.DWord)
            valueLength *= 4;

        var transactionIdentifier = _transactionId.ToByteArray();
        (var allDataLength, var dataContentLength, var byteCount, var quantityOfOutputs) = GetLength(function, valueLength, valueType);
        var startAt = startingAddress.ToByteArray();

        var data = new byte[allDataLength];
        data[0] = transactionIdentifier[1];
        data[1] = transactionIdentifier[0];
        data[2] = _protocolIdentifier[1];
        data[3] = _protocolIdentifier[0];
        data[4] = dataContentLength[1];
        data[5] = dataContentLength[0];
        data[6] = _unitIdentifier;
        data[7] = ((int)function).ToByte();
        data[8] = startAt[1];
        data[9] = startAt[0];
        data[10] = quantityOfOutputs[1];
        data[11] = quantityOfOutputs[0];
        data[12] = byteCount;

        byte singleCoilValue = 0;
        byte coilValue;
        byte[] singleRegisterValue;
        if (function == FunctionEnum.FC15)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if ((i % 8) == 0)
                    singleCoilValue = 0;

                if (values[i].ToBool())
                    coilValue = 1;
                else
                    coilValue = 0;

                singleCoilValue = (byte)((int)coilValue << (i % 8) | (int)singleCoilValue);

                data[13 + (i / 8)] = singleCoilValue;
            }
        }
        if (function == FunctionEnum.FC16)
        {

            var numberOfByte = 1;
            if (valueType == ValueTypeEnum.Word)
                numberOfByte = 2;
            if (valueType == ValueTypeEnum.DWord)
                numberOfByte = 4;
            
            for (int count = 0; count < valueLength; count+=numberOfByte)
            {

                var currentIndexArrayValues = count / numberOfByte;

                //TODO Criar tratamento de erro
                if (values[currentIndexArrayValues] != null)
                {
                    singleRegisterValue = values[currentIndexArrayValues].ToByteArray(valueType, valueType == ValueTypeEnum.Byte ? SwapTypeEnum.SwapBytes : _swapType);

                    data[13 + count * 2] = singleRegisterValue[0];
                    data[14 + count * 2] = singleRegisterValue[1];

                    if (numberOfByte > 1)
                    {
                        data[15 + count * 2] = singleRegisterValue[2];
                        data[16 + count * 2] = singleRegisterValue[3];
                    }

                    if (numberOfByte == 4)
                    {
                        data[17 + count * 2] = singleRegisterValue[4];
                        data[18 + count * 2] = singleRegisterValue[5];
                        data[19 + count * 2] = singleRegisterValue[6];
                        data[20 + count * 2] = singleRegisterValue[7];
                    }
                }

            }
        }

        var (sucess, _, errorCode, error) = Comunicate(data);
        return (sucess, errorCode, error);
    }
    private async Task<(bool sucess, string? errorCode, string? error)> WriteMultiplesInDeviceAsync<TValueType>(FunctionEnum function, int startingAddress, TValueType[] values, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        if (!_connected)
            return (false, "DeviceIsNotConnected", $"Device {_deviceAlias} is not connected.");

        _transactionId++;

        var valueLength = values.Length;
        if (valueType == ValueTypeEnum.Word)
            valueLength *= 2;

        if (valueType == ValueTypeEnum.DWord)
            valueLength *= 4;

        var transactionIdentifier = _transactionId.ToByteArray();
        (var allDataLength, var dataContentLength, var byteCount, var quantityOfOutputs) = GetLength(function, valueLength, valueType);
        var startAt = startingAddress.ToByteArray();

        var data = new byte[allDataLength];
        data[0] = transactionIdentifier[1];
        data[1] = transactionIdentifier[0];
        data[2] = _protocolIdentifier[1];
        data[3] = _protocolIdentifier[0];
        data[4] = dataContentLength[1];
        data[5] = dataContentLength[0];
        data[6] = _unitIdentifier;
        data[7] = ((int)function).ToByte();
        data[8] = startAt[1];
        data[9] = startAt[0];
        data[10] = quantityOfOutputs[1];
        data[11] = quantityOfOutputs[0];
        data[12] = byteCount;

        byte singleCoilValue = 0;
        byte coilValue;
        byte[] singleRegisterValue;
        if (function == FunctionEnum.FC15)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if ((i % 8) == 0)
                    singleCoilValue = 0;

                if (values[i].ToBool())
                    coilValue = 1;
                else
                    coilValue = 0;

                singleCoilValue = (byte)((int)coilValue << (i % 8) | (int)singleCoilValue);

                data[13 + (i / 8)] = singleCoilValue;
            }
        }
        if (function == FunctionEnum.FC16)
        {

            var numberOfByte = 1;
            if (valueType == ValueTypeEnum.Word)
                numberOfByte = 2;
            if (valueType == ValueTypeEnum.DWord)
                numberOfByte = 4;

            for (int count = 0; count < valueLength; count += numberOfByte)
            {

                var currentIndexArrayValues = count / numberOfByte;

                //TODO Criar tratamento de erro
                if (values[currentIndexArrayValues] != null)
                {
                    singleRegisterValue = values[currentIndexArrayValues].ToByteArray(valueType, valueType == ValueTypeEnum.Byte ? SwapTypeEnum.SwapBytes : _swapType);

                    data[13 + count * 2] = singleRegisterValue[0];
                    data[14 + count * 2] = singleRegisterValue[1];

                    if (numberOfByte > 1)
                    {
                        data[15 + count * 2] = singleRegisterValue[2];
                        data[16 + count * 2] = singleRegisterValue[3];
                    }

                    if (numberOfByte == 4)
                    {
                        data[17 + count * 2] = singleRegisterValue[4];
                        data[18 + count * 2] = singleRegisterValue[5];
                        data[19 + count * 2] = singleRegisterValue[6];
                        data[20 + count * 2] = singleRegisterValue[7];
                    }
                }

            }
        }


        var (sucess, _, errorCode, error) = await ComunicateAsync(data);
        return (sucess, errorCode, error);
    }
    private Socket InitializeSocket()    
        => new (IPAddress.Parse(_ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);    
    private (int allDataLength, byte[] dataContentLength, byte byteCount, byte[] quantityOutputs) GetLength(FunctionEnum function, int valuesLength, ValueTypeEnum valueType = ValueTypeEnum.Byte)
    {
        var allDataLength = 12;
        byte[] dataContentLength = _messageLength;
        byte byteCount = (valuesLength * 2).ToByte();
        int multiplier = 1;
        if (valueType == ValueTypeEnum.Word)        
            multiplier = 2;
        if (valueType == ValueTypeEnum.DWord)
            multiplier = 4;



        switch (function)
        {
            case FunctionEnum.FC01:
            case FunctionEnum.FC02:
            case FunctionEnum.FC03:
            case FunctionEnum.FC04:
            case FunctionEnum.FC05:
            case FunctionEnum.FC06:
                break;

            case FunctionEnum.FC15:
                allDataLength = (14 + 2 + (valuesLength % 8 != 0 ? valuesLength / 8 : (valuesLength / 8) - 1));
                dataContentLength = (7 + (valuesLength % 8 != 0 ? valuesLength / 8 + 1 : (valuesLength / 8))).ToByteArray();
                byteCount = (valuesLength % 8 != 0 ? valuesLength / 8 + 1 : (valuesLength / 8)).ToByte();
                break;

            case FunctionEnum.FC16:
                allDataLength = (13 + 2 + valuesLength * 2);
                dataContentLength = (7 + valuesLength * 2).ToByteArray();                
                break;

            default:
                byteCount = allDataLength.ToByte();
                break;
        }


        return (allDataLength, dataContentLength, byteCount, (valuesLength * multiplier).ToByteArray());

    }

    public void Dispose()
    {
        if (_socket.Connected)
        {
            _socket.Disconnect(false);
            _socket.Close();
        }
        GC.SuppressFinalize(this);
    }
}
