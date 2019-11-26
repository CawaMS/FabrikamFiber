using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FabrikamFiber.DAL.Data;

namespace FabrikamFiber.Api.Controllers
{
    public class CustomersController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public dynamic Get(int id)
        {
            CustomerRepository repository = new CustomerRepository();
            var customer = repository.Find(id);
            return customer;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
