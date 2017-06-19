using System;

namespace PhotoAlbum.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}
