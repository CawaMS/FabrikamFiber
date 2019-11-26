namespace FabrikamFiber.Web.Controllers
{
    using System.Linq;
    using System.Linq.Expressions;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Common;
    using System.Web.Mvc;
    using FabrikamFiber.DAL.Data;
    using FabrikamFiber.DAL.Models;
    using FabrikamFiber.Web.ViewModels;
    using Microsoft.ApplicationInsights;
    using System.Collections;
    using System.Collections.Generic;

    public class ReportsController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IServiceTicketRepository serviceTicketRepository;
        private static ArrayList _connections = new ArrayList();

        public ReportsController(IEmployeeRepository employeeRepository, IServiceTicketRepository serviceTicketRepository)
        {
            this.employeeRepository = employeeRepository;
            this.serviceTicketRepository = serviceTicketRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Employees()
        {
            var employees = this.employeeRepository.All.Select(e => new EmployeeSummary { Employee = e }).ToList();
            foreach (var summary in employees)
            {
                var tickets = this.serviceTicketRepository.AllIncluding(t => t.Customer).Where(t => t.AssignedToID == summary.Employee.ID).ToList();
                summary.AssignedTickets = tickets.Count();

                var firstTicket = tickets.FirstOrDefault();
                summary.CurrentCustomer = firstTicket == null ? null : firstTicket.Customer;
            }

            return View(new EmployeeReportViewModel { Employees = employees });
        }

        public ActionResult Tickets()
        {
            var report = this.AllForReport(
                serviceticket => serviceticket.Customer,
                serviceticket => serviceticket.CreatedBy,
                serviceticket => serviceticket.AssignedTo);

            return View();
        }

        private IQueryable<ServiceTicket> AllForReport(params Expression<Func<ServiceTicket, object>>[] includeProperties)
        {
            // DEMO: TrackTrace, TrackEvent
            TelemetryClient tc = new TelemetryClient();

            if (HttpContext.Request.Params.Get("clear") != null)
            {
                if (_connections == null)
                {
                    tc.TrackTrace("_connections is null", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Critical);
                }
                else
                {
                    tc.TrackEvent("Reports-All-ClearPools", new Dictionary<string, string> { { "Count", _connections.Count.ToString() } });
                }

                // Dispose all connections to remove failures
                foreach (DbConnection c in _connections)
                {
                    c.Close();
                    c.Dispose();
                }
                System.Data.SqlClient.SqlConnection.ClearAllPools();
                _connections.Clear();
                tc.TrackTrace("Pools are cleared");

                // DEMO: System.Diagnostics.Trace
                System.Diagnostics.Trace.TraceInformation("Pools are cleared - from System.Diagnostics.Trace");

                return null;
            }

            FabrikamFiberWebContext warehouseContext;
            DbConnection conn;
            IQueryable<ServiceTicket> query;
            try
            {
                warehouseContext = new FabrikamFiberWebContext("FabrikamFiber-DataWarehouse");
                query = warehouseContext.ServiceTickets.Take(5);
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                conn = warehouseContext.Database.Connection;
                conn.Open();
                tc.TrackTrace("Opened connection " + conn.GetHashCode());

                foreach (var tkt in query)
                {
                    if (tkt.AssignedTo != null)
                    {
                        // do somethign
                    }
                }
            }
            catch (Exception e)
            {
                throw new ConnectionTimeoutException(e.Message);
            }


            string isTest = HttpContext.Request.Params.Get("all");
            if (isTest != null)
            {
                tc.TrackTrace("Disposing connection " + conn.GetHashCode());
                warehouseContext.Dispose();
                return query;
            }

            try
            {
                processQuery(conn, query);
            }
            catch (Exception e)
            {
                // DEMO: TracKexception
                tc.TrackException(e);
                return null;
            }

            tc.TrackTrace("Disposing connection " + conn.GetHashCode());
            warehouseContext.Dispose();

            return query;
        }



        private void processQuery(System.Data.Common.DbConnection conn, IQueryable<ServiceTicket> query)
        {
            if (new Random().Next(10) > 8)
            {
                _connections.Add(conn);
                throw new Exception("Invalid query");
            }
        }
    }
}
