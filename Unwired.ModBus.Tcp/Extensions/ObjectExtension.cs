﻿using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Extensions;

public static class ObjectExtension
{
	public static bool ToBool(this object? value)
	{
			try
			{
				return (bool)(value ?? false);
			}
			catch
			{
				return false;
			}
	}
	public static ushort ToUShort(this object? value)
	{
		try
		{
			return (ushort)(value ?? 0);
		}
		catch
		{
			return 0;
		}
	}
	public static ushort? ToUShortNullable(this object? value)
	{
		try
		{
			if (value is null)
				return null;

			return (ushort)(value);

		}
		catch
		{
			return null;
		}
	}
    public static int ToInt(this object? value)
    {
        try
        {
            if(value == null)
				return 0;

			_ = int.TryParse(value.ToString(), out var result);
			return result;
        }
        catch
        {
            return 0;
        }
    }
    public static int? ToIntNullable(this object? value)
    {
        try
        {
            if (value is null)
                return null;

            _ = int.TryParse(value.ToString(), out var result);
            return result;

        }
        catch
        {
            return null;
        }
    }
    public static long ToLong(this object? value)
    {
        try
        {
            if (value == null)
                return 0;

            _ = long.TryParse(value.ToString(), out var result);
            return result;
        }
        catch
        {
            return 0;
        }
    }
    public static long? ToLongNullable(this object? value)
    {
        try
        {
            if (value is null)
                return null;

            _ = long.TryParse(value.ToString(), out var result);
            return result;

        }
        catch
        {
            return null;
        }
    }
    public static uint ToUInt(this object? value)
    {
        try
        {
            if (value == null)
                return 0;

            _ = uint.TryParse(value.ToString(), out var result);
            return result;
        }
        catch
        {
            return 0;
        }
    }
    public static uint? ToUIntNullable(this object? value)
    {
        try
        {
            if (value is null)
                return null;

            _ = uint.TryParse(value.ToString(), out var result);
            return result;

        }
        catch
        {
            return null;
        }
    }
    public static float ToFloat(this object? value)
	{
		try
		{
			return (float)(value ?? 0f);
		}
		catch
		{
			return 0f;
		}
	}
	public static float? ToFloatNullable(this object? value)
	{
		try
		{
			if (value is null)
				return null;

			return (float)(value);
		}
		catch
		{
			return 0f;
		}
	}
	public static double ToDouble(this object? value)
	{
		try
		{
			return (double)(value ?? 0d);
		}
		catch
		{
			return 0d;
		}
	}
	public static double? ToDoubleNullable(this object? value)
	{
		try
		{
			if (value is null)
				return null;

			return (double)(value);
		}
		catch
		{
			return 0f;
		}
	}
    public static byte[] ToByteArray(this object value, ValueTypeEnum valueType, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
    {
        switch (valueType)
        {
            case ValueTypeEnum.Word:
                return value.ToFloat().ToByteArray(swapType);
            case ValueTypeEnum.DWord:
                return value.ToDouble().ToByteArray(swapType);
            default:

				var type = value.GetType();
				switch (type.Name.ToLower())
				{
					case "ushort":
                        return value.ToUShort().ToByteArray(swapType);
                    case "uint32":
                        return value.ToUInt().ToByteArray(swapType);
                    case "int32":
                        return value.ToInt().ToByteArray(swapType);
                    case "int64":
                        return value.ToLong().ToByteArray(swapType);
                    default:
						break;
				}
				return value.ToUShort().ToByteArray(swapType);
        }
    }
}
