using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.BLL.Services;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;
using PhotoAlbum.DAL.Repositories;

namespace PhotoAlbum.Util
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            _kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            var connectionName = "PhotoAlbumDb";
            var context = new PhotoAlbumContext(connectionName);

            _kernel.Bind<PhotoAlbumContext>().ToConstructor(_ => new PhotoAlbumContext(connectionName)).InRequestScope();

            _kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            _kernel.Bind<UnitOfWork>().ToConstructor(_ => new UnitOfWork(context)).InRequestScope();

            //_kernel.Bind<IManyToManyResolver>().To<UserRepository>();
            _kernel.Bind<IRepository<User>>().To<UserRepository>();
            _kernel.Bind<UserRepository>().ToConstructor(_ => new UserRepository(context)).InRequestScope();
            _kernel.Bind<IUserService>().To<UserService>();

            _kernel.Bind<IRepository<Picture>>().To<PictureRepository>();
            _kernel.Bind<PictureRepository>().ToConstructor(_ => new PictureRepository(context)).InRequestScope();
            _kernel.Bind<IPictureService>().To<PictureService>();

            _kernel.Bind<IRepository<Like>>().To<LikeRepository>();
            _kernel.Bind<LikeRepository>().ToConstructor(_ => new LikeRepository(context)).InRequestScope();
            _kernel.Bind<ILikeService>().To<LikeService>();
        }
    }
}
