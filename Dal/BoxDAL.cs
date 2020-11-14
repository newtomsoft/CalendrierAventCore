using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using System;
using System.Linq;

namespace CalendrierAventCore.DAL
{
    public class BoxDAL
    {
        public Box Details(int id)
        {
            using DefaultContext db = new DefaultContext();
            Box box = (from c in db.Boxes
                       where c.Id == id
                       select c).FirstOrDefault();
            return box;
        }

        public Box Details(string name)
        {
            using DefaultContext db = new DefaultContext();
            Box box = (from c in db.Boxes
                       where c.Name == name
                       select c).FirstOrDefault();
            return box;
        }

        public void Add(string name)
        {
            Box box = new Box()
            {
                Name = name,
                Path = Guid.NewGuid().ToString("n"),
            };

            using DefaultContext db = new DefaultContext();
            db.Boxes.Add(box);
            db.SaveChanges();
        }
    }
}