using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace DiffBot
{
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

		/// <summary>
		/// Creates a Article model from a json source
		/// </summary>
		/// <param name="source"></param>
		public DiffBotArticleResultModel Create( string sourceJson )
		{
			return new JavaScriptSerializer( ).Deserialize<DiffBotArticleResultModel>( sourceJson );
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
}
