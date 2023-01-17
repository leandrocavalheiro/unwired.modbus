using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Extensions;

public static class DoubleExtensions
{
	public static int ToInt(this double value)
	{
		try
		{
			return Convert.ToInt32(value);
		}
		catch
		{
			return 0;
		}
	}

	public static byte[] ToByteArray(this double value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
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

    public static double Round(this double value, RoundTypeEnum roundType = RoundTypeEnum.None, int precision = 0)
    {
        try
        {
			switch (roundType)
			{
                case RoundTypeEnum.Round:
                    return Math.Round(value, precision);

                case RoundTypeEnum.Truncate:
					return Math.Truncate(value);

				case RoundTypeEnum.Ceiling:
                    return Math.Ceiling(value);

                case RoundTypeEnum.Floor:
                    return Math.Floor(value);

                default:
					return value;
            }
        }
        catch
        {
            return 0d;
        }
    }

}
