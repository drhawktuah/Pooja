using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Exceptions.Economy;

public class EconomyException : Exception
{
    public EconomyException()
    {

    }

    public EconomyException(string message) : base(message)
    {

    }

    public EconomyException(string message, Exception innerException) : base(message, innerException)
    {

    }
}