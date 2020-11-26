using Ideative.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MongoCustomIdentity.Controllers
{
    //[Authorize]

    public class HomeController : BaseController
    {


        public HomeController(
       
             INotificationHandler<DomainNotification> notifications
            ) : base(notifications)
        {
         
        }



        [Route("/")]        
        public IActionResult Index()
        {


      

            return View();
        }

        [Route("/hakkimizda")]
        public IActionResult About()
        {
            return View();
        }
        [Route("/kullanim-kosullari")]
        public IActionResult KullanimKosullari()
        {
            return View();
        }
        [Route("/gizlilik")]
        public IActionResult Gizlilik()
        {
            return View();
        }

        [Route("/iletisim")]
        public IActionResult Contact()
        {
            return View();
        }
        [Route("/sss")]
        public IActionResult faq()
        {
            return View();
        }
        //[Route("/site-haritasi")]
        //public IActionResult sitemap()
        //{
        //    ViewData["Message"] = "";

        //    return View();
        //}

        [Route("/error")]
        public IActionResult Error()
        {
           

            return View();
        }

        [Route("/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }





}
