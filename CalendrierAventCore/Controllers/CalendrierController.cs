using CalendrierAventCore.DAL;
using CalendrierAventCore.Data.Models;
using CalendrierAventCore.Data.ViewModels;
using CalendrierAventCore.Tools;
using Data.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AdventCalendar.Controllers
{
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
            Calendar calendar = CalendarDAL.DetailsByPublicName(name);
            if (calendar == null)
            {
                // if calendar witch PublicName don't exist, test with PrivateName and redirect to admin 
                if (CalendarDAL.DetailsByPrivateName(name) != null)
                {
                    //return Redirect($"{Request.Url.Scheme}://{Request.Url.Authority}/Modifier/{name}");
                    return Redirect($"/Modifier/{name}");
                }
                else
                {
                    return RedirectToAction("Ajouter");
                }
            }

            BoxDAL boxDAL = new BoxDAL();
            Box box = boxDAL.Details(calendar.BoxId);

            string boxPictureFullName = Path.Combine("\\", _config.Value.BoxPicturePath, box.Path); //todo "\\"plus propre
            Dictionary<int, string> dictionaryGenericsPicturesNames = new Dictionary<int, string>();
            for (int i = 1; i <= 24; i++)
            {
                dictionaryGenericsPicturesNames.Add(i, Path.Combine(boxPictureFullName, $"{i}.png"));
            }
            Dictionary<int, string> dictionaryPicturesNames = CalendarDAL.Dictionary(calendar.Id, DateTime.Today);

            CalendarViewModel calendarViewModel = new CalendarViewModel(calendar, dictionaryPicturesNames, dictionaryGenericsPicturesNames);
            return View(calendarViewModel);
        }

        [HttpGet]
        public IActionResult Ajouter()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ajouter(Calendar calendar, string email)
        {
            if (ModelState.IsValid)
            {
                string randomPublicSufix = "-" + Tool.RandomAsciiPrintable(6);
                string randomPrivateSufix = "-" + Tool.RandomAsciiPrintable(10);
                calendar.DisplayName = calendar.DisplayName.Replace("-", "");
                calendar.PublicName = calendar.DisplayName + randomPublicSufix;
                calendar.PrivateName = calendar.DisplayName + randomPrivateSufix;

                string directoryName = Path.Combine(_environment.WebRootPath, _config.Value.PicturePath, calendar.PublicName);
                Directory.CreateDirectory(directoryName);
                if (email != null)
                {
                    const string subject = "Calendrier créé";
                    string publicUrl = $"/{calendar.PublicName}";
                    string privateUrl = $"/{calendar.PrivateName}";
                    string message = "Bonjour\n\n" +
                        "Bravo, vous avez créé un calendrier.\n" +
                        "Voilà les liens indispensables :\n" +
                        $"public : {publicUrl}\n" +
                        $"privé (uniquement pour vous) : {privateUrl}\n" +
                        "N'oubliez pas d'y ajouter les photos / images\n\n" +
                        "A bientôt sur calendar"; //TODO
                    Tool.EnvoieMail(email, subject, message);
                }
                return RedirectToAction("Modifier", "Calendrier", new {name = calendar.PrivateName});
            }
            else
            {
                return View((Calendar)null);
            }
        }

        [HttpGet]
        public IActionResult Liste()
        {
            return View((Calendar)null);
        }

        [HttpPost]
        public IActionResult Liste(string password)
        {
            string cryptedGoodPassword = _config.Value.Adminpassword;
            string cryptedPassword = password.GetHash();
            if (cryptedPassword == cryptedGoodPassword)
            {
                return View(CalendarDAL.List());
            }
            else
            {
                ViewBag.Message = "Mot de passe incorrect";
                return View((Calendar)null);
            }
        }

        [HttpGet]
        public IActionResult Modifier(string name)
        {
            // TODO DRY même système que Calendrier Index de area "" 
            Calendar calendar = CalendarDAL.DetailsByPrivateName(name);
            BoxDAL boxDAL = new BoxDAL();
            Box box = boxDAL.Details(calendar.BoxId);
            string boxPictureFullName = Path.Combine("\\", _config.Value.BoxPicturePath, box.Path);
            Dictionary<int, string> dictionaryGenericsPicturesNames = new Dictionary<int, string>();
            for (int i = 1; i <= 24; i++)
            {
                dictionaryGenericsPicturesNames.Add(i, Path.Combine(boxPictureFullName, $"{i}.png"));
            }
            Dictionary<int, string> dictionaryPicturesNames = CalendarDAL.Dictionary(calendar.Id);
            CalendarViewModel calendarViewModel = new CalendarViewModel(calendar, dictionaryPicturesNames, dictionaryGenericsPicturesNames);
            return View(calendarViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Modifier(string privateName, IFormFile file, int dayNumber) //TODO
        {
            Calendar calendar = CalendarDAL.DetailsByPrivateName(privateName);
            int calendarId = calendar.Id;
            string publicName = calendar.PublicName;
            if (file != null)
            {
                string directoryName = Path.Combine(_environment.WebRootPath, _config.Value.PicturePath, publicName);
                string extension = Path.GetExtension(file.FileName);
                if (extension.Length > 5)
                    extension = extension.Substring(0, 5);
                string fileName = Guid.NewGuid().ToString("n") + extension;
                string fullFileName = Path.Combine(directoryName, fileName);

                using (FileStream stream = new FileStream(fullFileName, FileMode.Create))
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                }

                #region futurFeature
                //if (extension == ".webp")
                //{
                //    using (FileStream fileStream = new FileStream(fullFileName, FileMode.Create))
                //    {
                //        files[i].InputStream.Seek(0, SeekOrigin.Begin);
                //        files[i].InputStream.CopyTo(fileStream);
                //    }
                //}
                //else
                //{
                //    using (Image image = Image.FromStream(files[i].InputStream))
                //        image.Save(fullFileName);
                //}
                #endregion
                PictureDAL.Add(calendarId, dayNumber, fileName);
            }
            return Redirect($"/{privateName}");
        }
    }
}