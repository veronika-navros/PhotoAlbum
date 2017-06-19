using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.DAL.Repositories
{
    public class LikeRepository : IRepository<Like>
    {
        public PhotoAlbumContext Db { get; set; }

        public LikeRepository(PhotoAlbumContext db)
        {
            Db = db;
        }

        public IEnumerable<Like> GetAll()
            => Db.Likes.Include(c => c.User);

        public Like Get(int id) => Db.Likes.Find(id);

        public void Create(Like like) => Db.Likes.Add(like);

        public void Update(Like like) => Db.Entry(like).State = EntityState.Modified;

        public IEnumerable<Like> Find(Func<Like, bool> predicate)
            => Db.Likes.Where(predicate).ToList();

        public void Delete(int id)
        {
            Like like = Db.Likes.Find(id);
            if (!ReferenceEquals(like, null))
                Db.Likes.Remove(like);
        }
    }
}
