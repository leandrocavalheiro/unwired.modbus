using Unwired.ModBus.Tcp.Enumarators;

namespace Unwired.ModBus.Tcp.Extensions
{
    public static class IntegerExtensions
    {

        public static byte[] ToByteArray(this ushort value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
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
        public static byte[] ToByteArray(this ushort value, bool isWord, EndiannessEnum endianness = EndiannessEnum.LittleEndian)
        {
            try
            {
                var result = BitConverter.GetBytes(value);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }
            catch
            {
                var result = BitConverter.GetBytes(0);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }

        }
        public static byte[] ToByteArray(this uint value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
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
        public static byte[] ToByteArray(this uint value, bool isWord, EndiannessEnum endianness = EndiannessEnum.LittleEndian)
        {
            try
            {
                var result = BitConverter.GetBytes(value);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }
            catch
            {
                var result = BitConverter.GetBytes(0);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }
            
        }
        public static byte[] ToByteArray(this int value, bool isWord, EndiannessEnum endianness = EndiannessEnum.LittleEndian)
        {

            try
            {
                var result = BitConverter.GetBytes(value);
                if(isWord)                                
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }
            catch
            {
                var result = BitConverter.GetBytes(0);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }

        }
        public static byte[] ToByteArray(this int value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
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
        public static byte[] ToByteArray(this long value, bool isWord, EndiannessEnum endianness = EndiannessEnum.LittleEndian)
        {

            try
            {
                var result = BitConverter.GetBytes(value);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }
            catch
            {
                var result = BitConverter.GetBytes(0);
                if (isWord)
                    return BitConverter.GetBytes(value).ToSelectedEndianess(endianness);

                return result;
            }

        }
        public static byte[] ToByteArray(this long value, SwapTypeEnum swapType = SwapTypeEnum.NoSwap)
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


        public static byte ToByte(this int value)
        {

            try
            {
                return Convert.ToByte(value);
            }
            catch (Exception)
            {

                return default;
            }

        }
        public static byte ToByte(this uint value)
        {

            try
            {
                return Convert.ToByte(value);
            }
            catch (Exception)
            {

                return default;
            }
        }       
        public static int ToInt(this int? value)
        => value ?? 0;
    }
}
