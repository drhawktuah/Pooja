using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Exceptions.Economy;

public class PlayerNotFoundException : EconomyException
{
    public PlayerNotFoundException()
    {

    }

    public PlayerNotFoundException(string message) : base(message)
    {

    }

    public PlayerNotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }
}