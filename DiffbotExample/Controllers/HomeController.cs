using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiffBot;
using System.Net;

namespace DiffbotExample.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

		#region Frontpage

		[HttpGet]
		public ActionResult Frontpage( )
		{
			return View( );
		}
		
		//[HttpPost]
		public JsonResult FrontpageJson( string url, bool? html )
		{
			var diffbot = new DiffbotAPI();
			var source = new WebClient( ).DownloadString
			(
				diffbot.GetEndpointUrl
				(
					DiffbotAPI.EndPoints.Article,
					url,
					html ?? false
				)
			);			
			return Json(source, JsonRequestBehavior.AllowGet);
		}
		#endregion


		#region Article

		[HttpGet]
		public ActionResult Article( )
		{
			return View( );
		}

		[HttpPost]
		public JsonResult Article( string url, bool? html )
		{
			var diffbot = new DiffbotAPI( );
			var source = new WebClient( ).DownloadString
			(
				diffbot.GetEndpointUrl
				(
					DiffbotAPI.EndPoints.Article,
					url,
					html ?? false
				)
			);

			var model = new DiffBotArticleResultModel().Create( source );
			return Json( model, JsonRequestBehavior.AllowGet );
		}

		#endregion
	}
}
