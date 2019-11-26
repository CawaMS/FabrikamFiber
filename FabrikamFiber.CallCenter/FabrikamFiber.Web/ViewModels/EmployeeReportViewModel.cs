namespace FabrikamFiber.Web.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using FabrikamFiber.DAL.Models;
    using FabrikamFiber.DAL.Data;
    using System;
    using System.Net;
    using System.IO;

    public class EmployeeReportViewModel : Controller
    {
        public IEnumerable<EmployeeSummary> Employees { get; set; }

        public string GetCustomerSummary(Customer customer)
        {
            //if (customer == null) return "No customer is set";
            System.Diagnostics.Trace.WriteLine("Getting Summary of New Customer");

            if (new Random().Next(4) == 0)
            {
                System.Diagnostics.Trace.WriteLine("Reading customer updates from Twitter");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://twitter.com/statuseses/update.xml");
                // set the method to POST
                request.Method = "POST";
                // set up the stream
                Stream reqStream = request.GetRequestStream();
                if (reqStream.CanRead)  reqStream.ReadByte();
                // close the stream
                reqStream.Close();
            }
            
            string result = string.Format("{0}, {1}", customer.FullName, customer.Address.City);
            return result;
        }
    }
}
