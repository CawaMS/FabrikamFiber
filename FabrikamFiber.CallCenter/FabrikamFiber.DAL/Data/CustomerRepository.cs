namespace FabrikamFiber.DAL.Data
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using FabrikamFiber.DAL.Models;
    using System.Collections.Generic;

    public interface ICustomerRepository
    {
        List<Customer> All { get; }

        IQueryable<Customer> AllIncluding(params Expression<Func<Customer, object>>[] includeProperties);

        Customer Find(int id);

        void InsertOrUpdate(Customer customer);

        void Delete(int id);

        void Save();
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly FabrikamFiberWebContext context = new FabrikamFiberWebContext();

        public static List<Customer> CustomerCache { get; set; }

        public List<Customer> All
        {
            get 
            {
                System.Diagnostics.Trace.WriteLine("Listing Customers");

                if (CustomerCache == null)
                {
                    CustomerCache = new List<Customer>();
                }
                foreach (var c in this.context.Customers)
                {
                    CustomerCache.Add(c);
                }
                return this.context.Customers.ToList();
            }
        }

        public IQueryable<Customer> AllIncluding(params Expression<Func<Customer, object>>[] includeProperties)
        {
            IQueryable<Customer> query = this.context.Customers;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
        
        private void EnsureCacheInitialized()
        {
            System.Diagnostics.Trace.WriteLine("Listing Customers");

            if (CustomerCache == null)
            {
                CustomerCache = new List<Customer>();
            }
            foreach (var c in this.context.Customers)
            {
                CustomerCache.Add(c);
            }
        }

        public static object _cacheLock = new object();
        public Customer Find(int id)
        {
            lock (_cacheLock)
            {
                var customer = this.context.Customers.Where(Customer => Customer.ID == id).First();
                System.Diagnostics.Trace.WriteLine(String.Format("Getting Customer", customer.FirstName));
                return customer;
            }
        }

        public void InsertOrUpdate(Customer customer)
        {
            if (customer.ID == default(int))
            {
                CustomerCache.Add(customer);
                //this.context.Customers.Add(customer);
            }
            else
            {
                this.context.Entry(customer).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var customer = CustomerCache.Find(Customer => Customer.ID == id);
            CustomerCache.Remove(customer);
            //this.context.Customers.Remove(customer);
        }

        public void Save()
        {
            System.Diagnostics.Trace.WriteLine("Starting to save New Customer");

            //foreach (Customer c in this.context.Customers)
            //{
            //    this.context.Customers.Remove(c);
            //}
            foreach (Customer c in CustomerCache)
            {
                if (this.context.Customers.Find(c.ID) == null)
                {
                    this.context.Customers.Add(c);
                }
            }

            foreach (Customer c in this.context.Customers)
            {
                if (CustomerCache.Find(Customer => Customer.ID == c.ID) == null)
                {
                    this.context.Customers.Remove(c);
                }

            }

            this.context.SaveChanges();
        }
    }
}