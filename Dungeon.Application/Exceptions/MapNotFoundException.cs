namespace Dungeon.Application.Exceptions;

public class MapNotFoundException : Exception
{
    public MapNotFoundException(string message) : base(message)
    {
    }

    public MapNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}