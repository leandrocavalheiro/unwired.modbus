namespace Unwired.ModBus.Tcp.Exceptions;

public class UnwiredModBusClientConnectionException : Exception
{
    public UnwiredModBusClientConnectionException() : base()
    {
    }
    public UnwiredModBusClientConnectionException(string message) : base(message)
    {
    }
    public UnwiredModBusClientConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

}