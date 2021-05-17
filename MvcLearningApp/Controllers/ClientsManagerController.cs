using System;
using System.Web.Mvc;
using MvcLearningApp.Entities;
using MvcLearningApp.Repositories;
using MvcLearningApp.Helper;
using MvcLearningApp.Models;

namespace MvcLearningApp.Controllers
{
    [Authorize]
    [CheckSessionValid]
    public class ClientsManagerController : Controller
    {

        private ClientsRepository _clientRepo;
        public ClientsManagerController()
        {
            _clientRepo = new ClientsRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration=3000)]
        public JsonResult GetListAsync(int? page, string words)
        {
            //Thread.Sleep(3000);
            string msg = "Afisare clienti";
            bool ok = true;
            var model = new GridViewModel<ClientView>();
            try
            {
                model = _clientRepo.Compute(page, words, Convert.ToInt32(Session["Id_User"]));
                if (model.Items == null)
                {
                    msg = "List empty for this user";
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                ok = false;
            }
            return Json(new { 
                Success=ok,
                Message=msg,
                Html = ok ? this.RenderPartialView("_ClientsManagerList", model) : ""
            });
        }

        public JsonResult CreateAsync(ClientView model)
        {

            string msg = "Salvare Client";
            bool ok = true;
            //var res = new ClientView();
            var res = new Client();
            try
            {
                if (!_clientRepo.IsUniqueClient(model.Email))
                {
                    ok = false;
                    ModelState.AddModelError("Email", "Email in use");
                }
                else
                {
                    res = _clientRepo.AddClient(model, Convert.ToInt32(Session["Id_User"]), null);

                }
            }
            catch (Exception ex)
            {
                ok = false;
                msg = ex.Message;
            }

            return Json(new
            {
                Html = ok ? this.RenderPartialView("_NewRow", res) : "",
                Form = ok ? "" : this.RenderPartialView("_Create", model),
                Success = ok,
                Message = msg
            });
        }

        public JsonResult SaveClient(int id, string name, DateTime date, string mail)
        {
            var res = new ClientView();
            bool ok = true;
            var msg = "";

            try
            {
                var model = new ClientView();
                model.Id = id;
                model.Name = name;
                model.Email = mail;
                model.Date = date;
                res = _clientRepo.PostEditClient(model);
            }
            catch (Exception ex)
            {
                ok = false;
                msg = ex.Message;
            }
            return Json(new
            {
                Html = ok ? this.RenderPartialView("_NewRow", res) : "",
                Success = ok,
                Message = msg
            });
        }

        [HttpGet]
        public JsonResult GetEditAsync(int id)
        {
            string mesaj = string.Empty;
            var clientToEdit = new ClientView();
            try
            {
                clientToEdit = _clientRepo.GetEditClient(id);
            }
            catch (Exception ex)
            {
                mesaj = ex.Message;
            }
            return Json(new
            {
                Message = mesaj,
                Html = this.RenderPartialView("Edit", clientToEdit)
            });
        }

        [HttpPost]
        public JsonResult DeleteAsync(int id)
        {
            //Thread.Sleep(2000);
            string msg = "Delete Client";
            bool ok = true;
            if (_clientRepo.DeleteClient(id))
            {
                ok = true;
            }
            else
            {
                ok = false;
                msg = "Stergerea clientului a esuat, va rugam mai incercati";
            }

            return Json(new
            {
                Success = ok,
                Message = msg
            });
        }
    }
}
