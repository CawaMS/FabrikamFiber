namespace FabrikamFiber.Web.Controllers
{
    using System.Web.Mvc;

    using FabrikamFiber.DAL.Data;
    using FabrikamFiber.Web.ViewModels;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using FabrikamFiber.DAL.Models;
    using System.Collections.Generic;
    using System.Threading;
    using System;

    public class HomeController : Controller
    {
        private readonly IServiceTicketRepository serviceTickets;
        private readonly IMessageRepository messageRepository;
        private readonly IAlertRepository alertRepository;
        private readonly IScheduleItemRepository scheduleItemRepository;

        private int ID = 0;

        public HomeController(
                              IServiceTicketRepository serviceTickets,
                              IMessageRepository messageRepository,
                              IAlertRepository alertRepository,
                              IScheduleItemRepository scheduleItemRepository)
        {
            this.serviceTickets = serviceTickets;
            this.messageRepository = messageRepository;
            this.alertRepository = alertRepository;
            this.scheduleItemRepository = scheduleItemRepository;
        }

        private void HandleTick(object state)
        {
            // do something
            MvcApplication.ViewModelCache[ID] = new DashboardViewModel
            {
                ScheduleItems = this.scheduleItemRepository.All,
                Messages = this.messageRepository.All,
                Alerts = this.alertRepository.All,
                Tickets = this.serviceTickets.AllIncluding(serviceticket => serviceticket.Customer, serviceticket => serviceticket.CreatedBy, serviceticket => serviceticket.AssignedTo)

            };
        }

        public ActionResult Index()
        {
            ID = MvcApplication.CacheID;
            if (!MvcApplication.ViewModelCache.ContainsKey(MvcApplication.CacheID))
            {
                var viewModel = new DashboardViewModel();

                viewModel = new DashboardViewModel
                {
                    ScheduleItems = this.scheduleItemRepository.All,
                    Messages = this.messageRepository.All,
                    Alerts = this.alertRepository.All,
                    Tickets = this.serviceTickets.AllIncluding(serviceticket => serviceticket.Customer, serviceticket => serviceticket.CreatedBy, serviceticket => serviceticket.AssignedTo)

                };

                MvcApplication.ViewModelCache.Add(MvcApplication.CacheID, viewModel);
            }

            Timer timer = new Timer(HandleTick);
            timer.Change(System.TimeSpan.FromSeconds(5), System.TimeSpan.FromSeconds(5));

            MvcApplication.CacheID++;

            InitializeMetaData();

            return View(MvcApplication.ViewModelCache[MvcApplication.CacheID - 1]);
        }

        private void InitializeMetaData()
        {
            //DEMO TrackMetric
            Stopwatch timer = new Stopwatch();
            timer.Start();

            FabrikamFiberAzureStorage.GetStorageBlobData();
            FabrikamFiberAzureStorage.GetStorageQueueData();

            timer.Stop();
            MvcApplication.AppInsights.TrackMetric("home.index.init", timer.ElapsedMilliseconds);
        }
    }
}
