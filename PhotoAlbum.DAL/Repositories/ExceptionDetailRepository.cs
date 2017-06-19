using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.DAL.Repositories
{
    public class ExceptionDetailRepository : IRepository<ExceptionDetail>
    {
        public PhotoAlbumContext Db { get; set; }

        public ExceptionDetailRepository(PhotoAlbumContext db)
        {
            Db = db;
        }

        public IEnumerable<ExceptionDetail> GetAll()
            => Db.ExceptionDetails.Include(c => c.User);

        public ExceptionDetail Get(int id) => Db.ExceptionDetails.Find(id);

        public void Create(ExceptionDetail exceptionDetail) => Db.ExceptionDetails.Add(exceptionDetail);

        public void Update(ExceptionDetail exceptionDetail) => Db.Entry(exceptionDetail).State = EntityState.Modified;

        public IEnumerable<ExceptionDetail> Find(Func<ExceptionDetail, bool> predicate)
            => Db.ExceptionDetails.Where(predicate).ToList();

        public void Delete(int id)
        {
            ExceptionDetail exceptionDetail = Db.ExceptionDetails.Find(id);
            if (!ReferenceEquals(exceptionDetail, null))
                Db.ExceptionDetails.Remove(exceptionDetail);
        }
    }
}
