using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcLearningApp.Entities;

namespace MvcLearningApp.Repositories
{
    public class AddressesRepository
    {

        public ForTestingDbEntities db;
        public AddressesRepository()
        {
            db = new ForTestingDbEntities();
        }


    }
}