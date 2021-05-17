using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcLearningApp.Repositories;
using MvcLearningApp.Models;

namespace MvcLearningApp.Controllers
{
    public class ClientsListController : Controller
    {
        private ClientsRepository clientRepo;
        public ClientsListController()
        {
            clientRepo = new ClientsRepository();
        }
        //
        // GET: /ClientsList/

        public ActionResult IndexListClients()
        {
            return View(new ClientIndexView() { List = clientRepo.GetAllClients() });
        }
    }
}
