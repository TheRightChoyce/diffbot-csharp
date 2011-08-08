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
	public partial class DiffbotAPI
	{
		#region Defs

		/// <summary>
		/// List of all the various end points available to us
		/// </summary>
		public static class EndPoints
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

		#region Helpers

		/// <summary>
		/// Constructs and returns the appropriate Url for this endpoint
		/// </summary>
		/// <param name="type"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public string GetEndpointUrl( string type, string url, bool useHtml = false )
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

		/// <summary>
		/// Determines if the provided url is actually a valid url to parse
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private bool IsValidUrl( string url, out System.Uri uri )
		{
			uri = null;
			if (string.IsNullOrWhiteSpace( url )) // no need to waste cycles on an empty url
				return false;

			try
			{
				uri = new System.Uri( url );
			}
			catch
			{
				return false;
			}
			return true;
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
	}
}