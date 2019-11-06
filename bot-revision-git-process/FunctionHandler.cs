﻿using System;
using System.Net;
using System.Text;

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
				post.resource.commits.ForEach(x=>
				{
					try
					{
						string message=String.Format(messageFormat,
							post.resource.repository.name,
							delimiter,
							String.Format("{0}/commit/{1}",post.resource.repository.remoteUrl,x.commitId),
							delimiter,
							x.author.name,
							delimiter,
							x.comment,
							delimiter,
							""
						);
						using(WebClient wc=new WebClient())
						{
							Uri endpoint=new Uri("http://gateway:8080/function/bot-revision-process");
							wc.UploadDataAsync(endpoint,Encoding.UTF8.GetBytes(message));
						}
					}
					catch(Exception ex)
					{
						Console.Write("Failed to process commit: {0}",ex.Message);
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