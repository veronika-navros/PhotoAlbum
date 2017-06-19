using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using NLog;
using PhotoAlbum.BLL.Interface.Services;
using PhotoAlbum.BLL.Interface;
using PhotoAlbum.BLL.Interface.DTO;
using PhotoAlbum.DAL.Interfaces;
using PhotoAlbum.Util;
using WebGrease;
using LogManager = NLog.LogManager;

namespace PhotoAlbum.WEB.Filters
{
    public class ExceptionLoggerAttribute : FilterAttribute, IExceptionFilter
    {
        private static IKernel _appKernel;
        private static IExceptionDetailService _exceptionDetailService;
        private static IUnitOfWork _unitOfWork;
        private static Logger _logger;

        public void OnException(ExceptionContext filterContext)
        {
            string viewName = "Default";
            var statusCode = 0;
            Task<string> message = null;

            if (filterContext.Exception is HttpResponseException)
            {
                statusCode = (int)((HttpResponseException)filterContext.Exception).Response.StatusCode;
                message = ((HttpResponseException)filterContext.Exception).Response.Content.ReadAsStringAsync();
            }

            string innerExceptions = $"\n{filterContext.Exception}";
            while (filterContext.Exception.InnerException != null)
            {
                innerExceptions = innerExceptions + $"\n{filterContext.Exception.InnerException}";
            }

            switch (statusCode)
            {
                case 400:
                    viewName = "BadRequest";
                    break;
                case 403:
                    viewName = "Forbidden";
                    break;
                case 500:
                    viewName = "InternalServerError";
                    break;
                case 404:
                    viewName = "NotFound";
                    break;
                case 503:
                    viewName = "ServiceTemporarilyUnavailable";
                    break;
            }

            _appKernel = new StandardKernel(new NinjectModule());
            _exceptionDetailService = _appKernel.Get<IExceptionDetailService>();
            _unitOfWork = _appKernel.Get<IUnitOfWork>();

            ExceptionDetailDto exceptionDetail = new ExceptionDetailDto()
            {
                Guid = Guid.NewGuid(),
                Message = (message?.Result ?? filterContext.Exception.Message) + innerExceptions,
                Url = filterContext.HttpContext.Request.RawUrl,
                UrlReferrere = filterContext.HttpContext.Request.UrlReferrer == null ? "" : filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri,
                StackTrace = filterContext.Exception.StackTrace,
                Date = DateTime.Now,
                UserId = filterContext.HttpContext.User.Identity.GetUserId<int>()
            };

            _exceptionDetailService.Create(exceptionDetail);
            _unitOfWork.Save();

            _logger = LogManager.GetCurrentClassLogger();
            _logger.Error(filterContext.Exception, null,
                $"\n{nameof(exceptionDetail.Guid)}: " + exceptionDetail.Guid
                + $" \n{nameof(filterContext.Exception.Message)}: " + (message?.Result ?? filterContext.Exception.Message) + innerExceptions
                + $" \n{nameof(exceptionDetail.Url)}: " + exceptionDetail.Url
                + $" \n{nameof(exceptionDetail.UrlReferrere)}: " + exceptionDetail.UrlReferrere
                + $" \n{nameof(exceptionDetail.StackTrace)}: " + exceptionDetail.StackTrace
                + $" \n{nameof(exceptionDetail.Date)}: " + exceptionDetail.Date
                + $" \n{nameof(exceptionDetail.UserId)}: " + (exceptionDetail.UserId?.ToString() ?? "anonim") + "\n\n"
            );

            var result = new ViewResult
            {
                ViewName = viewName,
                MasterName = string.Empty,
                ViewData = new ViewDataDictionary<Guid>(exceptionDetail.Guid)
            };
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}