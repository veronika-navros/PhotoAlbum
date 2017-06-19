using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.DAL.Repositories
{
    public class UserRepository : IRepository<User>
    {
        public PhotoAlbumContext Db { get; set; }

        public UserRepository(PhotoAlbumContext db)
        {
            Db = db;
        }

        public IEnumerable<User> GetAll() => Db.Users;

        public User Get(int id) => Db.Users.Find(id);

        public void Create(User item) => Db.Users.Add(item);

        public IEnumerable<User> Find(Func<User, bool> predicate)
            => Db.Users.Where(predicate).ToList();

        public void Update(User item) => Db.Entry(item).State = EntityState.Modified;

        public void Delete(int id)
        {
            User client = Db.Users.Find(id);
            if (!ReferenceEquals(client, null))
                Db.Users.Remove(client);
        }
    }
}
