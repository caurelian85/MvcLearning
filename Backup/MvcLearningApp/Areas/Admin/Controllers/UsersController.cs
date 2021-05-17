using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcLearningApp.Models;
using MvcLearningApp.Repositories;
using MvcLearningApp.Helper;

namespace MvcLearningApp.Areas.Admin.Controllers
{
    [Authorize]
    public class UsersController : AdminController
    {
        private UsersRepository userRepo;
        public UsersController()
        {
            userRepo = new UsersRepository();
        }
        //
        // GET: /Admin/Users/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetListAsync(int? page, string words)
        {
            string msg = "Afisare useri";
            bool ok = true;
            var model = new GridViewModel<UserView>();
            try
            {
                model = userRepo.Compute(page, words);
                if (model.Items == null)
                {
                    msg = "List Empty";
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                ok = false;
            }
            return Json(new
            {
                Success = ok,
                Message = msg,
                Html = ok ? this.RenderPartialView("_UsersManagerList", model) : ""
            });
        }

        [HttpPost]
        public JsonResult EnableDisableAsync(int id, bool enable)
        {
            string msg = "Enable_Disable";
            bool ok = true;
            var res = new UserView();

            try
            {
                var model = new UserView();
                model.Id = id;
                
                model.Enable = enable;
                res = userRepo.EnDiUser(model);
            }
            catch (Exception ex)
            {
                ok = false;
                msg = ex.Message;
            }

            return Json(new
            {
                Success = ok,
                Message = msg
            });
        }

        [HttpPost]
        public JsonResult DeleteAsync(int id)
        {
            var model = new UserView();
            string msg = "Delete User";
            bool ok = true;
            if (userRepo.DeleteUser(id))
            {
                ok = true;
            }
            else
            {
                ok = false;
                msg = "Stergerea a esuat!";
            }
            return Json(new 
            {
                Success=ok,
                Message=msg
            });
        }
    }
}
