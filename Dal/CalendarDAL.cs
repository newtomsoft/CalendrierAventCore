using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using CalendrierAventCore.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendrierAventCore.DAL
{
    public static class CalendarDAL
    {
        public static Calendar Details(int id)
        {
            using DefaultContext db = new DefaultContext();
            return (from c in db.Calendars
                    where c.Id == id
                    select c).FirstOrDefault();
        }

        public static Calendar DetailsByPublicName(string publicName)
        {
            using DefaultContext db = new DefaultContext();
            return (from c in db.Calendars
                    where c.PublicName == publicName
                    select c).FirstOrDefault();
        }

        public static Calendar DetailsByPrivateName(string privateName)
        {
            using DefaultContext db = new DefaultContext();
            return (from c in db.Calendars
                    where c.PrivateName == privateName
                    select c).FirstOrDefault();
        }

        public static List<Calendar> List()
        {
            using DefaultContext db = new DefaultContext();
            return (from c in db.Calendars
                    select c).ToList();
        }

        public static Dictionary<int, string> PicturesList(int id, DateTime date)
        {
            if (date.Month == 12)
                return PictureDAL.Dictionary(Details(id).Id, date.Day);
            else
                return new Dictionary<int, string>();
        }

        public static Dictionary<int, string> Dictionary(int id, DateTime? date = null)
        {
            DateTime dateOk = date ?? DateTime.MaxValue;
            if (dateOk == DateTime.MaxValue)
            {
                return PictureDAL.Dictionary(id);
            }
            else if (dateOk.Month == 12)
            {
                return PictureDAL.Dictionary(id, dateOk.Day);
            }
            else
            {
                return new Dictionary<int, string>();
            }
        }

        public static Calendar Add(string name)
        {
            name = name.Replace("-", "");
            string randomPublicSuffix = "-" + Tool.RandomAsciiPrintable(6);
            string randomPrivateSuffix = "-" + Tool.RandomAsciiPrintable(10);
            Calendar calendar = new Calendar()
            {
                DisplayName = name,
                PublicName = name + randomPublicSuffix,
                PrivateName = name + randomPrivateSuffix,
                BoxId = 1 //TODO
            };
            using (DefaultContext db = new DefaultContext())
            {
                db.Calendars.Add(calendar);
                db.SaveChanges();
            }

            return calendar;
        }

        public static void Add(Calendar calendar)
        {
            using DefaultContext db = new DefaultContext();
            db.Calendars.Add(calendar);
            db.SaveChanges();
        }
    }
}