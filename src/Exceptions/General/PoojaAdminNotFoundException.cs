using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pooja.src.Exceptions.General;

public class PoojaAdminNotFoundException : PoojaGeneralException
{
    public PoojaAdminNotFoundException()
    {

    }

    public PoojaAdminNotFoundException(string message) : base(message)
    {

    }

    public PoojaAdminNotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }
}