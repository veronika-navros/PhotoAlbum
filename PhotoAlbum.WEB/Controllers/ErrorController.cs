//using System;
//using System.Net;
//using System.Net.Http;
//using System.Web.Mvc;

//namespace PhotoAlbum.WEB.Controllers
//{
//    public class ErrorController : Controller
//    {
//        public ActionResult NotFound()
//        {
//            throw new HttpResponseException(
//                new HttpResponseMessage(HttpStatusCode.NotFound)
//                { Content = new StringContent("Page not found") }
//            );
//        }

//        public ActionResult InternalServerError()
//        {
//            throw new HttpResponseException(
//                new HttpResponseMessage(HttpStatusCode.InternalServerError)
//                { Content = new StringContent("Internal server error") }
//            );
//        }

//        public ActionResult Forbidden()
//        {
//            throw new HttpResponseException(
//                new HttpResponseMessage(HttpStatusCode.Forbidden)
//                { Content = new StringContent("Access forbidden") }
//            );
//        }

//        public ActionResult ServiceTemporarilyUnavailable()
//        {
//            throw new HttpResponseException(
//                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
//                { Content = new StringContent("Service in temporarily unavailable") }
//            );
//        }

//        public ActionResult BadRequest()
//        {
//            throw new HttpResponseException(
//                new HttpResponseMessage(HttpStatusCode.BadRequest)
//                { Content = new StringContent("Bad request") }
//            );
//        }

//        public ActionResult Default()
//        {
//            throw new Exception("An error occured duting th request");
//        }
//    }
//}