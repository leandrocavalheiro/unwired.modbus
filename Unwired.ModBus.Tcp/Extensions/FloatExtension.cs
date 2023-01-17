using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Extensions;

public static class FloatExtensions
{
    public static byte[] ToByteArray(this float value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
    {
        try
        {
            return BitConverter.GetBytes(value).Swap(swapType);
        }
        catch
        {
            return BitConverter.GetBytes(0).Swap(swapType);
        }

    }
    public static float Round(this float value, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0)
    {
        try
        {
            switch (roundType)
            {
                case RoundTypeEnum.Round:
                    return (float)Math.Round(value, precision);
                case RoundTypeEnum.Truncate:
                    return (float)Math.Truncate(value);

                case RoundTypeEnum.Ceiling:
                    return (float)Math.Ceiling(value);

                case RoundTypeEnum.Floor:
                    return (float)Math.Floor(value);

                default:
                    return value;
            }
        }
        catch
        {
            return 0f;
        }
    }
}