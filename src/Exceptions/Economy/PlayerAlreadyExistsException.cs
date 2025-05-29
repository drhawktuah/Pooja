using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Exceptions.Economy;

public class PlayerAlreadyExistsException : EconomyException
{
    public PlayerAlreadyExistsException()
    {

    }

    public PlayerAlreadyExistsException(string message) : base(message)
    {

    }

    public PlayerAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {

    }
}