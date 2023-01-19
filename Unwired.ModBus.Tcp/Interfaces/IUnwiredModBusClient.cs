using System.Numerics;
using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Interfaces;

public interface IUnwiredModBusClient
{
    (bool success, string? errorCode, string? error) ConnectDevice(string ip = "127.0.0.1", ushort port = 502, byte unitIdentifier = 1, ushort timeout = 5000, SwapTypeEnum swapType = SwapTypeEnum.NoSwap, string deviceAlias = "127.0.0.1");
  
    bool Connected();

    (bool sucess, bool[]? results, string? errorCode, string? error) ReadCoils(int startingAddress, int totalAddresses = 1);
    Task<(bool sucess, bool[]? results, string? errorCode, string? error)> ReadCoilsAsync(int startingAddress, int totalAddresses = 1);
    (bool sucess, bool[]? results, string? errorCode, string? error) ReadInputs(int startingAddress, int totalAddresses = 1);
    Task<(bool sucess, bool[]? results, string? errorCode, string? error)> ReadInputsAsync(int startingAddress, int totalAddresses = 1);
    (bool sucess, TResult[]? results, string? errorCode, string? error) ReadHoldings<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    Task<(bool sucess, TResult[]? results, string? errorCode, string? error)> ReadHoldingsAsync<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    (bool sucess, TResult[]? results, string? errorCode, string? error) ReadInputRegisters<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    Task<(bool sucess, TResult[]? results, string? errorCode, string? error)> ReadInputRegistersAsync<TResult>(int startingAddress, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0) where TResult : INumber<TResult>;
    (bool sucess, string? errorCode, string? error) WriteSingleCoils(int address, bool value);
    Task<(bool sucess, string? errorCode, string? error)> WriteSingleCoilsAsync(int address, bool value);
    (bool sucess, string? errorCode, string? error) WriteSingleHolding<TValueType>(int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte);
    Task<(bool sucess, string? errorCode, string? error)> WriteSingleHoldingAsync<TValueType>(int address, TValueType value, ValueTypeEnum valueType = ValueTypeEnum.Byte);
    (bool sucess, string? errorCode, string? error) WriteMultiplesCoils(int startingAddress, bool[] values);
    Task<(bool sucess, string? errorCode, string? error)> WriteMultiplesCoilsAsync(int startingAddress, bool[] values);
    (bool sucess, string? errorCode, string? error) WriteMultiplesHolding<TValueType>(int startingAddress, TValueType[] value, ValueTypeEnum valueType = ValueTypeEnum.Byte);
    Task<(bool sucess, string? errorCode, string? error)> WriteMultiplesHoldingAsync<TValueType>(int startingAddress, TValueType[] value, ValueTypeEnum valueType = ValueTypeEnum.Byte);
}
