using System.Numerics;
using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Interfaces;

public interface IUnwiredModBusClient
{
    (bool success, string? errorCode, string? error) ConnectDevice(string ip, ushort port, byte unitIdentifier, ushort timeout = 5000, SwapTypeEnum swapType = SwapTypeEnum.NoSwap);   
    bool Connected();

    (bool sucess, bool[]? results, string? errorCode, string? error) ReadCoils(int startingAddress, int totalAddresses = 1);
    (bool sucess, bool[]? results, string? errorCode, string? error) ReadInputs(int startingAddress, int totalAddresses = 1);
    (bool sucess, TResult[]? results, string? errorCode, string? error) ReadHoldings<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    (bool sucess, TResult[]? results, string? errorCode, string? error) ReadInputRegisters<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    (bool sucess, string? errorCode, string? error) WriteSingleCoils(int address, bool value);
    (bool sucess, string? errorCode, string? error) WriteSingleHolding<TValueType>(int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte);
    (bool sucess, string? errorCode, string? error) WriteMultiplesCoils(int startingAddress, bool[] values);
    (bool sucess, string? errorCode, string? error) WriteMultiplesHolding<TValueType>(int startingAddress, TValueType[] value, ValueTypeEnum valueType = ValueTypeEnum.Byte);




}
