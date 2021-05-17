using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcLearningApp.Entities;
using MvcLearningApp.Repositories;
using MvcLearningApp.Models;
using System.IO;
using System.Web.UI.WebControls;

namespace MvcLearningApp.Controllers
{
    public class AddPictureController : Controller
    {
        private ClientsRepository _clientRepo;
        private ForTestingDbEntities _db;

        public AddPictureController()
        {
            _clientRepo = new ClientsRepository();
            _db = new ForTestingDbEntities();
        }

        //
        // GET: /AddPicture/

        public ActionResult Index(int id)
        {
            return View(new ClientView() { Id = id });
        }

        [HttpPost]
        public ActionResult AddPictureToClient(int id, HttpPostedFileBase file)
        {
            if(_clientRepo.UpdateImage(id, file))
                return RedirectToAction("Index", "ClientsManager"); 
            else return View();
        }

        public FileContentResult GetImage(int id)
        {
            var clientPicture = _db.Clients.FirstOrDefault(a => a.Id == id);
            if (clientPicture != null)
                return new FileContentResult(clientPicture.Avatar, clientPicture.AvatarMimeType);
            else return null;
        }
    }
}
