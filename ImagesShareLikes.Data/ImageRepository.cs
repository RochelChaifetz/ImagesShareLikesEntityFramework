using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesShareLikes.Data
{
    public class ImageRepository
    {
        private string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetImages()
        {
            using var context = new ImagesDbContext(_connectionString);
            return context.Images.ToList();
        }

        public void Add(Image image)
        {
            using var context = new ImagesDbContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetById(int id)
        {
            using var context = new ImagesDbContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void UpdateLikes(int id)
        {
            using var context = new ImagesDbContext(_connectionString);
            var image = context.Images.FirstOrDefault(i => i.Id == id);
            if (image != null)
            {
                image.Likes++;
                context.SaveChanges();
            }

        }

    }
}
