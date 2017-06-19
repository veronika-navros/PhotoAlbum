using System;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly PhotoAlbumContext _db;

        public UnitOfWork(PhotoAlbumContext context)
        {
            _db = context;
        }

        public void Save() => _db.SaveChanges();

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _db.Dispose();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
