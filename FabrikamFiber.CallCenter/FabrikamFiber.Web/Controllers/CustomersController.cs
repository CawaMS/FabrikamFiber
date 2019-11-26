namespace FabrikamFiber.Web.Controllers
{
    using System.Web.Mvc;

    using DAL.Data;
    using DAL.Models;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System.Configuration;
    using System.Net.Http;

    public class CustomersController : Controller
    {
        private readonly ICustomerRepository customerRepository;
        private object _customerLock = new object();

        public CustomersController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public ViewResult Index()
        {
            var defCustomer = this.customerRepository.All[0];
            FabrikamFiberAzureStorage.GetStorageTableData(defCustomer);
            return View(this.customerRepository.All);
        }

        public async Task<ViewResult> Details(int id)
        {
            var primary = ConfigurationManager.AppSettings["ApiService-Backup"];

            var client = new HttpClient();

            var result = await client.GetAsync(string.Format("{0}/api/customers/{1}", primary, id));
            var resultStr = await result.Content.ReadAsStringAsync();
            Customer customer = null;
            lock (_customerLock)
            {
                customer = JsonConvert.DeserializeObject<Customer>(resultStr);
            }
            
            //if (result != null)
            //{
            //    var secondary = ConfigurationManager.AppSettings["ApiService-Backup"];
            //    result = await client.GetAsync(string.Format("{0}/api/customers/{1}", secondary, id));
            //    resultStr = await result.Content.ReadAsStringAsync();
            //    lock (_customerLock)
            //    {
            //        customer = JsonConvert.DeserializeObject<Customer>(resultStr);
            //    }
            //}
            //FabrikamFiberAzureStorage.GetStorageTableData(customer);
            return View(customer);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                this.customerRepository.InsertOrUpdate(customer);
                this.customerRepository.Save();
                return RedirectToAction("Index");
            }
            
            return this.View();
        }

        public ActionResult Edit(int id)
        {
            return View(this.customerRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                this.customerRepository.InsertOrUpdate(customer);
                this.customerRepository.Save();
                return RedirectToAction("Index");
            }
            
            return this.View();
        }

        public ActionResult Delete(int id)
        {
            return View(this.customerRepository.Find(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            this.customerRepository.Delete(id);
            this.customerRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

