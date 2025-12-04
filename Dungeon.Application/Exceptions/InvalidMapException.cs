namespace Dungeon.Application.Exceptions;

public class InvalidMapException : Exception
{
    public InvalidMapException(string message) : base(message)
    {
    }

    public InvalidMapException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}