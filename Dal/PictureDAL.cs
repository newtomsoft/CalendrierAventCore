using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using Data.Config;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Options;

namespace Dal;

public class PictureDal
{
    private readonly IOptions<MyConfig> _config;

    public PictureDal(IOptions<MyConfig> config)
    {
        _config = config;
    }

    public Dictionary<int, string> Dictionary(int calendarId, int dayNumber = 31)
    {
        string calendarPath = new CalendarDal(_config).Details(calendarId).PublicName;
        var openPicturePath = Path.Combine(Path.DirectorySeparatorChar.ToString(), _config.Value.PicturePath, calendarPath);
        //var openPicturePath = Path.Combine(Path.DirectorySeparatorChar.ToString(), _config.Value.PicturePath, calendarPath);

        using DefaultDbContext db = new DefaultDbContext();
        return (from p in db.Pictures
            join c in db.Calendars on p.CalendarId equals c.Id
            where p.CalendarId == calendarId && p.DayNumber <= dayNumber
            select p).ToDictionary(x => x.DayNumber, x => Path.Combine(openPicturePath, x.Name));
    }

    public void Add(int calendarId, int dayNumber, string name)
    {
        using DefaultDbContext db = new DefaultDbContext();
        var picture = (from p in db.Pictures
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