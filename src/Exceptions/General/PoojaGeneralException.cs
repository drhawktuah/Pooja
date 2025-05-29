namespace Pooja.src.Exceptions.General;

public class PoojaGeneralException : Exception
{
    public PoojaGeneralException()
    {

    }

    public PoojaGeneralException(string message) : base(message)
    {

    }

    public PoojaGeneralException(string message, Exception innerException) : base(message, innerException)
    {

    }
}