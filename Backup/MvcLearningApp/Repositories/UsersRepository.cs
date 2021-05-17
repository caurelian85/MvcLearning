using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcLearningApp.Entities;
using System.Web.Security;
using MvcLearningApp.Models;
using System.Linq.Expressions;

namespace MvcLearningApp.Repositories
{
    public class UsersRepository
    {
        private ForTestingDbEntities db;
        public UsersRepository()
        {
            db = new ForTestingDbEntities();
        }
        public static readonly Func<User, UserView> Map_User_UserView = t =>
        {
            var res = new UserView()
            {
                UserName=t.UserName,
                Password=t.Password,
                Email=t.Email,
                Enable=t.Enable
            };
            return res;
        };

        public UserView EnDiUser(UserView model)
        {
            User userEntry = new User();
            if (model.Id.HasValue)
            {
                userEntry = db.Users.FirstOrDefault(a=>a.Id==model.Id.Value);
            }
            userEntry.Enable = model.Enable;
            if (!model.Id.HasValue)
            {
                db.AddToUsers(userEntry);
            }
            db.SaveChanges();
            return Map_User_UserView(userEntry);
        }

        public bool DeleteUser(int? id)
        {
            UserView usr = new UserView();
            string msg = string.Empty;
            var userIdFromClients = (from c in db.Clients where c.Id_User == id select c).ToList();
            if (userIdFromClients.Count == 0)
            {
                db.DeleteObject((from x in db.Users where x.Id == id select x).FirstOrDefault());
                db.SaveChanges();
                msg = "User sters";
                return true;
            }
            else
            {
                msg = "Userul are inregistrari";
                return false;      
            }
         }

        public bool UserIsValid(string name, string passw, out int id)
        {
            id = 0;
            bool authenticated = false;
            var passWord = FormsAuthentication.HashPasswordForStoringInConfigFile(passw, "SHA1");
            var idUser = (from x in db.Users where x.UserName == name && x.Password == passWord where x.Enable==true select x.Id).FirstOrDefault();
            if (idUser > 0)
            {
                authenticated = true;
                id = idUser;
            }
            else { authenticated = false; }
            return authenticated;
        }

        public MembershipCreateStatus CreateUser(string name, string passw, string email, out MembershipCreateStatus createStatus)
        {
            createStatus = 0;
            var userToAdd = new User();
            userToAdd.UserName = name;
            userToAdd.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(passw, "SHA1");
            userToAdd.Email = email;
            userToAdd.Enable = true;

            var userDuplicate = from x in db.Users where x.UserName == name select x;
            if (userDuplicate.Count() > 1)
                createStatus = MembershipCreateStatus.DuplicateUserName;

            var emailDuplicate = from x in db.Users where x.Email == email select x;
            if (emailDuplicate.Count() > 1)
                createStatus = MembershipCreateStatus.DuplicateEmail;

            if (createStatus == 0)
            {
                db.AddToUsers(userToAdd);
                db.SaveChanges();
            }
            return createStatus;
        }

        public GridViewModel<UserView> Compute(int? page, string words, int rowPerPage = 5, int pagerLength = 5)
        {

            var count = from x in db.Users where x.Id == 34 select x.Clients.Count;

            if (!page.HasValue) page = 1;
            var result = new GridViewModel<UserView>();
            result.Pager = new PagerView();

            var queryC = from x in db.Users where x.Id !=null select x;

            if (!string.IsNullOrEmpty(words))
            {
                foreach (var word in words.Split(' '))
                {
                    queryC = from c in queryC where c.UserName.Contains(word) || c.Email.Contains(word) select c;
                }
            }

            int totalRecords = queryC.Count();
            if (totalRecords > 0)
            {
                int reqPageNumber = result.Pager.Init(totalRecords, rowPerPage, pagerLength, page);

                result.Items = (from x in queryC orderby x.Id descending select new UserView() { Id = x.Id,UserName=x.UserName, Email = x.Email, Enable=x.Enable, CanDelete = x.Clients.Count<1 }).Skip(result.Pager.RecordsPerPage * (result.Pager.CurrentPage - 1)).Take(result.Pager.RecordsPerPage).ToList();

            }
            return result;
        }

        #region changePassword
        /*public MembershipUser GetUser(string p, bool userIsOnline)
        {
        }*/

        public bool ChangePassword(string oldPass, string newPass)
        {
            bool Success = false;
            var userChange = new User();
            userChange.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(newPass, "SHA1");
            db.SaveChanges();

            return Success;
        }
        #endregion
    }
}