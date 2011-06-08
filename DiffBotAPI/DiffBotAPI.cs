using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Configuration;

namespace DiffBot
{
	public class DiffbotAPI
	{
		#region Defs

		/// <summary>
		/// List of all the various end points available to us
		/// </summary>
		private static class EndPoints
		{
			public const string Article = "article";
			public const string FrontPage = "frontpage";
		}

		#endregion

		#region Config

		/// <summary>
		/// Your API Token (required)
		/// </summary>
		private static string APIToken = ConfigurationManager.AppSettings["DiffBotAPI_Token"];
		
		/// <summary>
		/// Root url for the DiffBot API (optional; defaults to http://www.diffbot.com/api/)
		/// </summary>
		private static string API = ConfigurationManager.AppSettings["DiffBotAPI_Url"] == null
			? "http://www.diffbot.com/api/"
			: ConfigurationManager.AppSettings["DiffBotAPI_Url"];

		/// <summary>
		/// Whether or not to return the results in html format (optional; defaults to false)
		/// </summary>
		private static bool UseHtml = ConfigurationManager.AppSettings["DiffBotAPI_UseHtml"] == null
			? false
			: Convert.ToBoolean(ConfigurationManager.AppSettings["DiffBotAPI_UseHtml"]);

		/// <summary>
		/// Set this to true if you want to try and do a simple html title parse if the diffbot API fails (optional; defaults to false)
		/// </summary>
		private bool UseWebClientFallback = ConfigurationManager.AppSettings["DiffBotAPI_UseWebClientFallback"] == null
			? false
			: Convert.ToBoolean( ConfigurationManager.AppSettings["DiffBotAPI_UseWebClientFallback"] );

		#endregion

		#region Article Models
		/// <summary>
		/// Base object for storing all the results of an article request
		/// </summary>
		public class DiffBotArticleResultModel
		{
			public string icon { get; set; }
			public string url { get; set; }
			public string resolved_url { get; set; }
			public string title { get; set; }
			public string author { get; set; }
			public string date { get; set; }
			public List<DiffBotArticleResultMediaModel> media { get; set; }
			public string text { get; set; }
			public List<string> tags { get; set; }
			public string xpath { get; set; }

			public DiffBotArticleResultModel( )
			{
				icon = "";
				url = "";
				resolved_url = "";
				title = "";
				author = "";
				date = "";
				media = new List<DiffBotArticleResultMediaModel>( );
				text = "";
				tags = new List<string>( );
				xpath = "";
			}
		}


		/// <summary>
		/// Stores all the media related fields
		/// </summary>
		public class DiffBotArticleResultMediaModel
		{
			public string type { get; set; }
			public bool primary { get; set; }
			public string link { get; set; }

			public DiffBotArticleResultMediaModel( )
			{
				type = "";
				primary = false;
				link = "";
			}
		}
		#endregion

		#region FrontPage Models

		/// <summary>
		/// Base object for storing the result of a front page query
		/// </summary>
		public class DiffBotFrontpageResultModel
		{
			public long id { get; set; }
			public string title { get; set; }
			public string sourceURL { get; set; }
			public string icon { get; set; }
			public string url { get; set; }
			public int numItems { get; set; }

			public List<DiffBotFrontpageItemResultModel> items { get; set; }

			public DiffBotFrontpageResultModel( )
			{
				id = 0;
				title = "";
				sourceURL = "";
				icon = "";
				url = "";
				numItems = 0;
				items = new List<DiffBotFrontpageItemResultModel>( );
			}
		}
		/// <summary>
		/// Front page item result
		/// </summary>
		public class DiffBotFrontpageItemResultModel
		{
			public DiffBotFrontpageItemResultModel( )
			{
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Constructs and returns the appropriate Url for this endpoint
		/// </summary>
		/// <param name="type"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		private string GetEndpointUrl( string type, string url, bool useHtml = false )
		{
			return string.Format
			(
				"{0}{1}?token={2}&url={3}{4}"
				, API
				, type
				, APIToken
				, url
				, (UseHtml || useHtml) ? "&html=true" : ""
			);
		}
		#endregion

		#region Fallback helpers

		/// <summary>
		/// This grabs the raw URL and returns it in string form.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected string DownloadUrlAsString( string url )
		{
			try
			{
				return new WebClient( ).DownloadString( url );
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// Returns a page's title element from a raw html string
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		protected string GetTitleFromHtmlString( string html )
		{
			return Regex.Match( html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase ).Groups["Title"].Value;
		}

		/// <summary>
		/// Not yet implemented
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		protected string GetBodyFromHtmlString( string html )
		{
			return "";
		}

		#endregion

		#region Article related
		/// <summary>
		/// Parse out an article by URL and return a strongly typed object
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public DiffBotArticleResultModel Article( string url, bool useHtml = false )
		{
			var model = new DiffBotArticleResultModel( );
			if (string.IsNullOrWhiteSpace( url )) // no need to waste cycles on an empty url
				return model;

			try
			{
				var source = new WebClient( ).DownloadString(
					GetEndpointUrl( EndPoints.Article, url, useHtml )
				);

				var js = new JavaScriptSerializer( );
				model = js.Deserialize<DiffBotArticleResultModel>( source );
			}
			catch // if it fails, try using a backup regex
			{
				var html = DownloadUrlAsString( url );
				if (!string.IsNullOrWhiteSpace( url ))
				{
					model.title = GetTitleFromHtmlString( html );
				}
			}

			return model;
		}

		#endregion

		#region FrontPage related

		public DiffBotFrontpageResultModel Frontpage( string url, bool useHtml = false )
		{
			var model = new DiffBotFrontpageResultModel( );

			if (string.IsNullOrWhiteSpace( url )) // no need to waste cycles on an empty url
				return model;

			System.Uri uri;
			try
			{
				uri = new System.Uri( url );
			}
			catch
			{
				return model; // not a valid url
			}

			XDocument xml = new XDocument( );
			try
			{
				var source = new WebClient( ).OpenRead(
					GetEndpointUrl( EndPoints.FrontPage, uri.AbsoluteUri, useHtml )
				);
				xml = XDocument.Load( source );
			}
			catch
			{
				// do nothing
			}

			if (xml.Elements( ).Count( ) == 0)
				return model;

			var info = xml.Element( "dml" ).Element( "info" );

			#region Title
			var titleEl = info.Element( "title" );
			if (titleEl != null)
				model.title = titleEl.Value;
			else if (UseWebClientFallback)
			{
				var html = DownloadUrlAsString( uri.AbsoluteUri );
				if (!string.IsNullOrWhiteSpace( html ))
					model.title = GetTitleFromHtmlString( html );
			}
			#endregion

			#region Icon
			var iconEl = info.Element( "icon" );
			if (iconEl != null)
				model.icon = iconEl.Value;
			else if (UseWebClientFallback) // I've found diffbot doesn't always process favicon.ico files for some reason
			{
				var host = uri.Scheme + "://" + uri.Host;
				WebRequest request = WebRequest.Create( new Uri( host + "/favicon.ico" ) );
				request.Method = "HEAD";
				try
				{
					WebResponse response = request.GetResponse( );

					if (response.ContentLength > 0) // if there's something there, then let's go ahead and use it
						model.icon = host + "/favicon.ico";
				}
				catch
				{
					model.icon = ""; // looks like we got a 404
				}
			}
			#endregion

			return model;
		}

		#endregion

	}
}