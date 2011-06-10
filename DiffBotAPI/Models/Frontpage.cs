using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace DiffBot
{
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

		/// <summary>
		/// Creates a Article model from a json source
		/// </summary>
		/// <param name="source"></param>
		public DiffBotFrontpageResultModel Create( string sourceJson )
		{
			return new JavaScriptSerializer( ).Deserialize<DiffBotFrontpageResultModel>( sourceJson );
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
}
