using ImagesShareLikes.Data;
using ImagesShareLikes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Text.Json;

namespace ImagesShareLikes.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;
        private IWebHostEnvironment _webHostEnvironment;
        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            return View(new HomePageViewModel
            {
                Images = repo.GetImages().OrderByDescending( i => i.DateUploaded).ToList()
            });
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(Image image, IFormFile imageFile)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            image.ImageName = fileName;
            image.DateUploaded = DateTime.Now;
            repo.Add(image);

            return Redirect("/");
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);

            var vm = new ImageViewModel
            {
                Image = repo.GetById(id)
            };

            List<int> idsInSession = HttpContext.Session.Get<List<int>>("idsInSession");

            if (idsInSession != null && idsInSession.Contains(vm.Image.Id))
            {
                vm.Liked = true;
            }

            return View(vm);
        }
        public IActionResult GetImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var image = repo.GetById(id);

            return Json(image);
        }
        [HttpPost]
        public void UpdateImageLikes(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.UpdateLikes(id);
            var idsInSession = HttpContext.Session.Get<List<int>>("idsInSession");

            if (idsInSession == null)
            {
                idsInSession = new List<int>();
            }

            idsInSession.Add(id);
            HttpContext.Session.Set("idsInSession", idsInSession);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}