using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Exceptions.General;

public class PoojaAdminAlreadyExistsException : PoojaGeneralException
{
    public PoojaAdminAlreadyExistsException()
    {

    }

    public PoojaAdminAlreadyExistsException(string message) : base(message)
    {

    }

    public PoojaAdminAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {

    }
}