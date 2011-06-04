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
		/// Root url for the DiffBot API (option; defaults to http://www.diffbot.com/api/)
		/// </summary>
		private static string API = ConfigurationManager.AppSettings["DiffBotAPI_Url"] == null ? 
			ConfigurationManager.AppSettings["DiffBotAPI_Url"]
			: "http://www.diffbot.com/api/";

		/// <summary>
		/// Whether or not to return the results in html format (optional; defaults to false)
		/// </summary>
		private static bool UseHtml = ConfigurationManager.AppSettings["DiffBotAPI_UseHtml"] == null
			? Convert.ToBoolean(ConfigurationManager.AppSettings["DiffBotAPI_UseHtml"])
			: false;

		/// <summary>
		/// Set this to true if you want to try and do a simple html title parse if the diffbot API fails (optional; defaults to false)
		/// </summary>
		private bool UseWebClientFallback = ConfigurationManager.AppSettings["DiffBotAPI_UseWebClientFallback"] == null
			? Convert.ToBoolean( ConfigurationManager.AppSettings["DiffBotAPI_UseWebClientFallback"] )
			: false;

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

		#region Article related
		/// <summary>
		/// Parse out an article by URL and return a strongly typed object
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public DiffBotArticleResultModel Article( string url, bool useHtml = false )
		{
			var model = new DiffBotArticleResultModel( );
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
				try
				{
					WebClient web = new WebClient( );
					model.title = Regex.Match( web.DownloadString( url ), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase ).Groups["Title"].Value;
					// TODO: RegEx for Body?
				}
				catch
				{
					// if this fails, then nothing we can do
				}
			}

			return model;
		}

		#endregion

		#region FrontPage related

		public DiffBotFrontpageResultModel Frontpage( string url, bool useHtml = false )
		{
			var model = new DiffBotFrontpageResultModel( );
			XDocument xml = new XDocument( );
			try
			{
				var source = new WebClient( ).OpenRead(
					GetEndpointUrl( EndPoints.FrontPage, url, useHtml )
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

			var titleEl = info.Element( "title" );
			if (titleEl != null)
				model.title = titleEl.Value;
			var iconEl = info.Element( "icon" );
			if (iconEl != null)
				model.icon = iconEl.Value;

			return model;
		}

		#endregion

	}
}