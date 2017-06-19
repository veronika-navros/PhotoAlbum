using Ninject.Web.Common;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.BLL.Services;
using PhotoAlbum.DAL.EF;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;
using PhotoAlbum.DAL.Repositories;

namespace PhotoAlbum.Util
{
    public class NinjectModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            var connectionName = "PhotoAlbumDb";
            var context = new PhotoAlbumContext(connectionName);

            Bind<PhotoAlbumContext>().ToConstructor(_ => new PhotoAlbumContext(connectionName)).InRequestScope();

            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<UnitOfWork>().ToConstructor(_ => new UnitOfWork(context)).InRequestScope();

            Bind<IRepository<ExceptionDetail>>().To<ExceptionDetailRepository>();
            Bind<ExceptionDetailRepository>()
                .ToConstructor(_ => new ExceptionDetailRepository(context))
                .InRequestScope();
            Bind<IExceptionDetailService>().To<ExceptionDetailService>();

            //Bind<IManyToManyResolver>().To<ClientRepository>();
            Bind<IRepository<User>>().To<UserRepository>();
            Bind<UserRepository>().ToConstructor(_ => new UserRepository(context)).InRequestScope();
            Bind<IUserService>().To<UserService>();
        }
    }
}
