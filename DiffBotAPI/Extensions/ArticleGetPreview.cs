using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiffBot
{
	public partial class DiffbotAPI
	{
		private string VideoYouTubeTemplate = "<iframe width=\"459\" height=\"350\" src=\"{0}\" frameborder=\"0\" allowfullscreen=\"true\"></iframe>";

		/// <summary>
		/// Gets a basic 300 word preview for this article, or if this is a video, returns the proper embed tag for the video contents
		/// </summary>
		/// <param name="article"></param>
		/// <returns></returns>
		public string ArticleGetPreview( DiffBotArticleResultModel article )
		{
			var description = "";
			
			// check for videos
			if (article.media.Count > 0)
			{
				foreach( var media in article.media )
				{
					if ( string.IsNullOrEmpty(description)
						&& media.type == "video" 
						&& (media.primary || article.media.Count == 1 )
					)
					{
						description = string.Format( VideoYouTubeTemplate, media.link );
						continue;
					}
				}
			}

			if (string.IsNullOrWhiteSpace( article.text )) // don't parse if there's no text
				return description;
			
			var end = article.text.Length < 300 ? article.text.Length : 300;
			return description + "\n" + article.text.Substring( 0, end );
		}
	}
}
