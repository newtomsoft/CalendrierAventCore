using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CalendrierAventCore.DAL;
using CalendrierAventCore.Data.Models;
using CalendrierAventCore.Data.ViewModels;
using CalendrierAventCore.Tools;
using Dal;
using Data.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CalendrierAventCore.Controllers;

public class CalendrierController : Controller
{
    private readonly IOptions<MyConfig> _config;
    private readonly IWebHostEnvironment _environment;
    public CalendrierController(IOptions<MyConfig> config, IWebHostEnvironment environment)
    {
        _environment = environment;
        _config = config;
    }

    [HttpGet]
    public IActionResult Lire(string name)
    {
        var calendar = new CalendarDal(_config).DetailsByPublicName(name);
        if (calendar == null)
        {
            // if calendar witch PublicName don't exist, test with PrivateName and redirect to admin 
            if (new CalendarDal(_config).DetailsByPrivateName(name) != null)
            {
                //return Redirect($"{Request.Url.Scheme}://{Request.Url.Authority}/Modifier/{name}");
                return Redirect($"/Modifier/{name}");
            }
            return RedirectToAction("Ajouter");
        }
        var boxDAL = new BoxDal();
        var box = boxDAL.Details(calendar.BoxId);
        string boxPictureFullName = Path.Combine("/", _config.Value.BoxPicturePath, box.Path);
        var dictionaryGenericsPicturesNames = new Dictionary<int, string>();
        for (var i = 1; i <= 24; i++) dictionaryGenericsPicturesNames.Add(i, Path.Combine(boxPictureFullName, $"{i}.png"));
        var dictionaryPicturesNames = new CalendarDal(_config).Dictionary(calendar.Id, DateTime.Today);
        var calendarViewModel = new CalendarViewModel(calendar, dictionaryPicturesNames, dictionaryGenericsPicturesNames);
        return View(calendarViewModel);
    }

    [HttpGet]
    public IActionResult Ajouter() => View();

    [HttpPost]
    public IActionResult Ajouter(Calendar calendar, string email)
    {
        if (!ModelState.IsValid) return View((Calendar)null);
        var randomPublicSufix = "-" + Tool.RandomAsciiPrintable(6);
        var randomPrivateSufix = "-" + Tool.RandomAsciiPrintable(10);
        calendar.DisplayName = calendar.DisplayName.Replace("-", "");
        calendar.PublicName = calendar.DisplayName + randomPublicSufix;
        calendar.PrivateName = calendar.DisplayName + randomPrivateSufix;
        new CalendarDal(_config).Add(calendar);
        string directoryName = Path.Combine(_environment.WebRootPath, _config.Value.PicturePath, calendar.PublicName);
        Console.WriteLine(directoryName);
        Directory.CreateDirectory(directoryName);
        if (email == null) return RedirectToAction("Modifier", "Calendrier", new {name = calendar.PrivateName});
        const string subject = "Calendrier créé";
        var publicUrl = $"/{calendar.PublicName}";
        var privateUrl = $"/{calendar.PrivateName}";
        var message = "Bonjour\n\n" +
                      "Bravo, vous avez créé un calendrier.\n" +
                      "Voilà les liens indispensables :\n" +
                      $"public : {publicUrl}\n" +
                      $"privé (uniquement pour vous) : {privateUrl}\n" +
                      "N'oubliez pas d'y ajouter les photos / images\n\n" +
                      "A bientôt sur calendar"; //TODO
        Tool.EnvoieMail(email, subject, message);
        return RedirectToAction("Modifier", "Calendrier", new { name = calendar.PrivateName });
    }

    [HttpGet]
    public IActionResult Liste() => View((Calendar)null);

    [HttpPost]
    public IActionResult Liste(string password)
    {
        var cryptedGoodPassword = _config.Value.Adminpassword;
        var cryptedPassword = password.GetHash();
        if (cryptedPassword == cryptedGoodPassword) return View(new CalendarDal(_config).List());
        ViewBag.Message = "Mot de passe incorrect";
        return View((Calendar)null);
    }

    [HttpGet]
    public IActionResult Modifier(string name)
    {
        // TODO DRY même système que Calendrier Index de area "" 
        var calendar = new CalendarDal(_config).DetailsByPrivateName(name);
        var boxDal = new BoxDal();
        var box = boxDal.Details(calendar.BoxId);
        string boxPictureFullName = Path.Combine("/", _config.Value.BoxPicturePath, box.Path);
        var dictionaryGenericsPicturesNames = new Dictionary<int, string>();
        for (var i = 1; i <= 24; i++) dictionaryGenericsPicturesNames.Add(i, Path.Combine(boxPictureFullName, $"{i}.png"));
        var dictionaryPicturesNames = new CalendarDal(_config).Dictionary(calendar.Id);
        var calendarViewModel = new CalendarViewModel(calendar, dictionaryPicturesNames, dictionaryGenericsPicturesNames);
        return View(calendarViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Modifier(string privateName, IFormFile file, int dayNumber) //TODO
    {
        var calendar = new CalendarDal(_config).DetailsByPrivateName(privateName);
        var calendarId = calendar.Id;
        var publicName = calendar.PublicName;
        if (file == null) return Redirect($"/{privateName}");
        string directoryName = Path.Combine(_environment.WebRootPath, _config.Value.PicturePath, publicName);
        var extension = Path.GetExtension(file.FileName);
        if (extension.Length > 5) extension = extension[..5];
        var fileName = Guid.NewGuid().ToString("n") + extension;
        var fullFileName = Path.Combine(directoryName, fileName);

        await using var stream = new FileStream(fullFileName, FileMode.Create);
        await file.CopyToAsync(stream).ConfigureAwait(false);
        new PictureDal(_config).Add(calendarId, dayNumber, fileName);
        return Redirect($"/{privateName}");
    }
}