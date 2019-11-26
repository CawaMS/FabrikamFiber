using FabrikamFiber.DAL.Data;
using System;
using System.ComponentModel.DataAnnotations;
namespace FabrikamFiber.DAL.Models
{
    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        //[RegularExpression(@"\d{5}-\d{4}|\d{5}|\d{9}|[ABCEGHJ-NPRSTVXY]{1}[0-9]{1}[ABCEGHJ-NPRSTV-Z]{1}[ ]?[0-9]{1}[ABCEGHJ-NPRSTV-Z]{1}[0-9]{1}")]
        [RegularExpression(@"\d{5}-\d{4}|\d{5}|\d{9}")]
        public string Zip { get; set; }

        public override string ToString()
        {
            return Street;
        }

        public string FullAddress()
        {
            return String.Format("{0}, {1}, {2}, {3}", Street, City, AddressValidator.ValidZipCode(Zip), State);
        }
    }
}
