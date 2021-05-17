using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcLearningApp.Models;
using MvcLearningApp.Entities;

namespace MvcLearningApp.Repositories
{
    public class ClientsRepository
    {
        private ForTestingDbEntities db;

        public ClientsRepository()
        {
            db = new ForTestingDbEntities();
        }

        public static readonly Func<Client, ClientView> Map_Client_ClientView = t =>
        {
            var res = new ClientView()
            {
                Id = t.Id,
                Name = t.Name,
                Date = t.Date,
                Id_Address=Convert.ToInt32(t.Id_Address),
                Id_User=t.Id_User,
                Email = t.Email,
                Avatar = t.Avatar,
                Phone = t.Phone
            };
            return res;
        };

        public List<ClientView> GetAllClients()
        {
            var list = new List<ClientView>();
            list = (from x in db.Clients 
                    orderby x.Date descending 
                    select new ClientView() { 
                        Id = x.Id, 
                        Email = x.Email, 
                        Date = x.Date, 
                        Name = x.Name, 
                        Phone = x.Phone }).ToList();
            return list;
        }

        public GridViewModel<ClientView> Compute(int? page, string words, int idUser, int rowPerPage = 5, int pagerLength = 5)
        {
            if (!page.HasValue) page = 1;
            var result = new GridViewModel<ClientView>() { Pager = new PagerView() };

            var queryC = from x in db.Clients where x.Id_User == idUser select x;

            if (!string.IsNullOrEmpty(words))
            {
                foreach (var word in words.Split(' '))
                {
                    queryC = from c in queryC where c.Name.Contains(word) || c.Email.Contains(word) select c;
                }
            }

            int totalRecords = queryC.Count();
            if (totalRecords > 0)
            {
                int reqPageNumber = result.Pager.Init(totalRecords, rowPerPage, pagerLength, page);

                result.Items = (from x in queryC orderby x.Date descending select new ClientView() { Id = x.Id, Email = x.Email, Date = x.Date, Name = x.Name, Phone = x.Phone, CanLoad = x.Avatar != null }).Skip(result.Pager.RecordsPerPage * (result.Pager.CurrentPage - 1)).Take(result.Pager.RecordsPerPage).ToList();

            }
            return result;
        }

        public ClientView AddClient(ClientView model, int userId, HttpPostedFileBase image)
        {
            var clientToAdd = new Client();
            clientToAdd.Name = model.Name;
            clientToAdd.Date = DateTime.Now;
            clientToAdd.Email = model.Email;
            clientToAdd.Id_User = userId;
            clientToAdd.Phone = model.Phone;
            //if (image != null)
            //{
            //    clientToAdd.AvatarMimeType = image.ContentType;
            //    clientToAdd.Avatar = new byte[image.ContentLength];
            //    image.InputStream.Read(clientToAdd.Avatar, 0, image.ContentLength);
            //}
            db.SaveChanges();
            return Map_Client_ClientView(clientToAdd);
        }

        public bool UpdateImage(int userId, HttpPostedFileBase image)
        {
           var clientToAdd = db.Clients.FirstOrDefault(a => a.Id == userId);
            if (image != null)
            {
                clientToAdd.AvatarMimeType = image.ContentType;
                clientToAdd.Avatar = new byte[image.ContentLength];
                image.InputStream.Read(clientToAdd.Avatar, 0, image.ContentLength);
            }

            db.SaveChanges();
            return true;
        }

        public ClientView GetEditClient(int id)
        {
            string mesaj = string.Empty;
            //return db.Clients.Select(Map_Client_ClientView).FirstOrDefault(x => x.Id == id);
            return (from x in db.Clients where x.Id == id select new ClientView() { Name = x.Name, Email = x.Email, Date = x.Date, Id = x.Id }).FirstOrDefault();
        }

        public ClientView PostEditClient(ClientView model)
        {
            Client clEntry;
            if (model.Id.HasValue)
                clEntry = db.Clients.FirstOrDefault(a => a.Id == model.Id.Value);
            else
                clEntry = new Client();
            clEntry.Name = model.Name;
            clEntry.Date = model.Date;
            clEntry.Email = model.Email;
            clEntry.Phone = model.Phone;

            if (!model.Id.HasValue)
            {
                db.AddToClients(clEntry);
            }
            db.SaveChanges();
            return Map_Client_ClientView(clEntry);
        }

        public bool DeleteClient(int? id)
        {
            string mesaj = string.Empty;
            db.DeleteObject((from x in db.Clients where x.Id == id select x).FirstOrDefault());
            db.SaveChanges();
            return true;
        }

        // in controller-ul ClientController -> la create (verific sa nu mai fie un email asemanator)
        public bool IsUniqueClient(string mail)
        {
            var ok = true;
            ok = (from x in db.Clients where x.Email == mail select x.Id).Count() < 1;
            return ok;
        }
    }
}