using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Configuration;

namespace DiffBot
{
	public partial class DiffbotAPI
	{
		/// <summary>
		/// Parses a Frontpage
		/// </summary>
		/// <param name="url"></param>
		/// <param name="useHtml"></param>
		/// <returns></returns>
		public DiffBotFrontpageResultModel Frontpage( string url, bool useHtml = false )
		{
			var model = new DiffBotFrontpageResultModel( );
			System.Uri uri;
			if (!IsValidUrl( url, out uri )) // no need to waste cycles on an incorrect url
				return model;

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
	}
}
