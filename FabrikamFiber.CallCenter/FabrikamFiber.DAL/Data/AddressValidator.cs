using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabrikamFiber.DAL.Data
{
    public class AddressValidator
    {
        public static int ValidZipCode(string zip)
        {
            int zipCode = Int32.Parse(zip);
            if (zipCode > 99999)
            {
                throw new InvalidOperationException();
            }
            return zipCode;
        }
    }
}
