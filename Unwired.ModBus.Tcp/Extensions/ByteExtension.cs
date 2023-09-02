using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Extensions;

public static class ByteExtension
{
    public static TResult[] ConvertResult<TResult>(this byte[] values, int totalAddresses = 1, ValueTypeEnum valueType = ValueTypeEnum.Byte, SwapTypeEnum swapType = SwapTypeEnum.NoSwap,  RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0)
    {
        var resultLength = totalAddresses / (int)valueType;
        if (resultLength == 0)
            return default(TResult[]);
               
        TResult[]? result = new TResult[resultLength];
        var type = typeof(TResult).ToString();

        if (valueType == ValueTypeEnum.Byte)
        {
            for (int currentAddress = 0; currentAddress < resultLength; currentAddress++)
                result[currentAddress] = (TResult)Convert.ChangeType(BitConverter.ToUInt16(new byte[] { values[9 + currentAddress * 2 + 1], values[9 + currentAddress * 2] }, 0), typeof(TResult));

            return result;
        }

        var swapperByte = values.Swap(swapType);
        var bytesPerResult = 1;

        if (valueType == ValueTypeEnum.Word)
            bytesPerResult = 2;

        if (valueType == ValueTypeEnum.DWord)
            bytesPerResult = 4;
       
        var currentValue = 0;
        for (int currentAddress = 0; currentAddress < resultLength; currentAddress++)
        {
            var currentByes = (new byte[] { values[9 + currentValue * 2], values[9 + currentValue * 2 + 1], values[9 + currentValue * 2 + 2], values[9 + currentValue * 2 + 3] }).Swap(swapType);

            switch (type)
            {
                case "System.Single":
                    result[currentAddress] = (TResult)Convert.ChangeType(BitConverter.ToSingle(currentByes).Round(roundType, precision), typeof(TResult));
                    break;

                case "System.Double":
                    if (valueType == ValueTypeEnum.Word)
                        result[currentAddress] = (TResult)Convert.ChangeType((double)BitConverter.ToSingle(currentByes, 0).Round(roundType, precision), typeof(TResult));
                    
                    if (valueType == ValueTypeEnum.DWord)
                        result[currentAddress] = (TResult)Convert.ChangeType(BitConverter.ToDouble(currentByes, 0), typeof(TResult));

                    break;

                default:
                    result[currentAddress] = (TResult)Convert.ChangeType(BitConverter.ToUInt16(currentByes), typeof(TResult));
                    break;

            }
            currentValue += bytesPerResult;
        }

        if (result == null)
            return default;

        

        return result;
    }
    public static byte[] ToSelectedEndianess(this byte[] values, EndiannessEnum endianness = EndiannessEnum.LittleEndian)
    {
        if (endianness == EndiannessEnum.LittleEndian)
            return values;

        return new byte[] { values[1], values[0] };
    }
    public static byte[] Swap(this byte[] values, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
    {
        var result = new byte[values.Length];
        switch (swapType)
        {
            case SwapTypeEnum.NoSwap:
                return values;

            case SwapTypeEnum.SwapBytes:
                for (int count = 0; count < values.Length; count+=2)
                {
                    result[count] = values[count +1];
                    result[count + 1] = values[count];
                }
                break;

            case SwapTypeEnum.SwapWords:
                if (values.Length <= 3)
                    return values;

                for (int count = 0; count < values.Length; count += 4)
                {
                    result[count] = values[count + 2];
                    result[count + 1] = values[count + 3];

                    result[count + 2] = values[count];
                    result[count + 3] = values[count + 1];
                }
                
                break;
            case SwapTypeEnum.SwapWordsAndBytes:
                if (values.Length <= 3)
                    return values;

                for (int count = 0; count < values.Length; count+=4)
                {
                    result[count] = values[count + 3];
                    result[count + 1] = values[count + 2];

                    result[count + 2] = values[count + 1];
                    result[count + 3] = values[count];
                }
                break;
            default:
                break;
        }        
        return result;
    }
}