using CalendrierAventCore.Data;
using CalendrierAventCore.Data.Models;
using System.Linq;

namespace CalendrierAventCore.DAL;

public class BoxDal
{
    public Box Details(int id)
    {
        using DefaultDbContext db = new();
        var box = (from c in db.Boxes
            where c.Id == id
            select c).FirstOrDefault();
        return box;
    }
}