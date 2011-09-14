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
		/// Parse out an article by URL and return a strongly typed object
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public DiffBotArticleResultModel Article( string url, bool useHtml = false )
		{
			var model = new DiffBotArticleResultModel( );
			System.Uri uri;
			if (!IsValidUrl( url, out uri )) // no need to waste cycles on an incorrect url
				return model;

			try
			{
				var source = GetWebClient().DownloadString(
					GetEndpointUrl( EndPoints.Article, url, useHtml )
				);

				model = model.Create( source );
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
	}
}
