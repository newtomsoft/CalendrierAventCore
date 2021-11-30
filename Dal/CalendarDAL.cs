using System;
using System.Collections.Generic;
using System.Linq;
using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using Data.Config;
using Microsoft.Extensions.Options;

namespace Dal;

public class CalendarDal
{
    private readonly IOptions<MyConfig> _config;

    public CalendarDal(IOptions<MyConfig> config)
    {
        _config = config;
    }

    public Calendar Details(int id)
    {
        using DefaultDbContext db = new();
        return (from c in db.Calendars
                where c.Id == id
                select c).FirstOrDefault();
    }

    public Calendar DetailsByPublicName(string publicName)
    {
        using DefaultDbContext db = new();
        return (from c in db.Calendars
                where c.PublicName == publicName
                select c).FirstOrDefault();
    }

    public Calendar DetailsByPrivateName(string privateName)
    {
        using DefaultDbContext db = new();
        return (from c in db.Calendars
                where c.PrivateName == privateName
                select c).FirstOrDefault();
    }

    public List<Calendar> List()
    {
        using DefaultDbContext db = new();
        return (from c in db.Calendars
                select c).ToList();
    }

    public Dictionary<int, string> Dictionary(int id, DateTime? date = null)
    {
        var dateOk = date ?? DateTime.MaxValue;
        if (dateOk == DateTime.MaxValue) return new PictureDal(_config).Dictionary(id);
        return dateOk.Month == 12 ? new PictureDal(_config).Dictionary(id, dateOk.Day) : new Dictionary<int, string>();
    }
   
    public void Add(Calendar calendar)
    {
        calendar.BoxId = 1; //TODO
        using DefaultDbContext db = new();
        db.Calendars.Add(calendar);
        db.SaveChanges();
    }
}
