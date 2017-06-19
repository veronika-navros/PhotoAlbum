using AutoMapper;
using Ninject;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.DAL.EF.Entities;
using PhotoAlbum.DAL.Interfaces;

namespace PhotoAlbum.BLL.Services
{
    public class ExceptionDetailService : IExceptionDetailService
    {
        private readonly IRepository<ExceptionDetail> _exceptionDetailRepository;

        [Inject]
        public ExceptionDetailService(IRepository<ExceptionDetail> exceptionDetailRepository)
        {
            _exceptionDetailRepository = exceptionDetailRepository;
        }

        public void Create(ExceptionDetailDto exceptionDetail)
        {
            if (exceptionDetail.UserId == 0)
                exceptionDetail.UserId = null;

            Mapper.Initialize(cfg => cfg.CreateMap<ExceptionDetailDto, ExceptionDetail>());
            _exceptionDetailRepository.Create(Mapper.Map<ExceptionDetailDto, ExceptionDetail>(exceptionDetail));
        }
    }
}
