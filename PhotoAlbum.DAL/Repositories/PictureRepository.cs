using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.DAL.Repositories
{
    public class PictureRepository : IRepository<Picture>
    {
        public PhotoAlbumContext Db { get; set; }

        public PictureRepository(PhotoAlbumContext db)
        {
            Db = db;
        }

        public IEnumerable<Picture> GetAll()
            => Db.Pictures.Include(c => c.User);

        public Picture Get(int id) => Db.Pictures.Find(id);

        public void Create(Picture picture) => Db.Pictures.Add(picture);

        public void Update(Picture picture) => Db.Entry(picture).State = EntityState.Modified;

        public IEnumerable<Picture> Find(Func<Picture, bool> predicate)
            => Db.Pictures.Where(predicate).ToList();

        public void Delete(int id)
        {
            Picture picture = Db.Pictures.Find(id);
            if (!ReferenceEquals(picture, null))
                Db.Pictures.Remove(picture);
        }
    }
}
