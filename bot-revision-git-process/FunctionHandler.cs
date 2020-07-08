using System;
using System.Net;
using System.Text;
using System.Linq;

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
			catch
			{
				Console.Write("Failed to parse JSON input");
				return;
			}
			if(post!=null)
			{
				string delimiter="SVNDELIMITERTEXT---- ";
				string messageFormat="{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}";
				post.resource?.commits?.OrderBy(y=>y.author.date).ToList().ForEach(x=>
				{
					string message=null;
					try
					{
						string branch=post.resource.repository.defaultBranch;
						branch=branch.Substring(branch.LastIndexOf('/')+1);
						string repo=String.Format("{0}.{1}",post.resource.repository.name,branch);
						message=String.Format(messageFormat,
							repo,
							delimiter,
							String.Format("{0}/commit/{1}|{1}",post.resource.repository.remoteUrl,x.commitId),
							delimiter,
							x.author.name,
							delimiter,
							x.comment,
							delimiter,
							""
						);

						using(WebClient wc=new WebClient())
						{
							wc.UploadData("http://gateway:8080/function/bot-revision-process",Encoding.UTF8.GetBytes(message));
						}
						System.Console.WriteLine("Posted this: \n{0}",message);
					}
					catch(Exception ex)
					{
						Console.Write("Failed to process commit: {0}\nMessage Posted:\n {1}",ex.Message,message);
					}
				});
			}
			else
			{
				Console.Write("Post was null");
			}
		}
	}
}
