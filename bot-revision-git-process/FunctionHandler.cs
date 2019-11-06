using System;
using System.Net;

using Newtonsoft.Json;

namespace Function
{
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			TfsGitPost post=new TfsGitPost();
			try
			{
				post=JsonConvert.DeserializeObject<TfsGitPost>(input);
			}
			catch (System.Exception)
			{
				Console.WriteLine("Failed to parse JSON input");
				throw;
			}
			string delimiter="SVNDELIMITERTEXT---- ";
			string messageFormat="{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}";
			post.resource.commits.ForEach(x=>
			{
				string message=String.Format(messageFormat,
					post.resource.repository,
					delimiter,
					x.url,
					delimiter,
					x.author,
					delimiter,
					x.comment,
					delimiter,
					""
				);
				using(WebClient wc=new WebClient())
				{
					wc.UploadString("http://gateway:8080/function/bot-revision-process",message);
				}
			});
		}
	}
}
