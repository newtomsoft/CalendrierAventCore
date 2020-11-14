using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CalendrierAventCore.DAL
{
    public static class PictureDAL
    {
        public static Picture Details(int calendarId, int dayNumber)
        {
            using DefaultContext db = new DefaultContext();
            return (from p in db.Pictures
                    join c in db.Calendars on p.CalendarId equals c.Id
                    where p.CalendarId == calendarId && p.DayNumber == dayNumber
                    select p).FirstOrDefault();
        }

        public static Dictionary<int, string> Dictionary(int calendarId, int dayNumber = 31)
        {
            string calendarPath = CalendarDAL.Details(calendarId).PublicName;
            string openPicturePath = Path.Combine("/Content/Photos/", calendarPath);
            //TODO path
            //string openPicturePath = Path.Combine(ConfigurationManager.AppSettings["PicturePath"], calendarPath);

            using DefaultContext db = new DefaultContext();
            return (from p in db.Pictures
                    join c in db.Calendars on p.CalendarId equals c.Id
                    where p.CalendarId == calendarId && p.DayNumber <= dayNumber
                    select p).ToDictionary(x => x.DayNumber, x => Path.Combine(openPicturePath, x.Name));
        }

        public static void Add(int calendarId, int dayNumber, string name)
        {
            using DefaultContext db = new DefaultContext();
            Picture picture = (from p in db.Pictures
                               join c in db.Calendars on p.CalendarId equals c.Id
                               where p.CalendarId == calendarId && p.DayNumber == dayNumber
                               select p).FirstOrDefault();

            if (picture == null)
            {
                picture = new Picture()
                {
                    CalendarId = calendarId,
                    DayNumber = dayNumber,
                    Name = name,
                };
                db.Pictures.Add(picture);
            }
            else
            {
                picture.Name = name;
            }
            db.SaveChanges();
        }
    }
}