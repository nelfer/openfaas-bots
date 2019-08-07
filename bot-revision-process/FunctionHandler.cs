using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Newtonsoft.Json;

namespace Function
{
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			System.Console.WriteLine("Starting");
			string delimiter="SVNDELIMITERTEXT---- ";
			List<string> segments=new List<string>(input.Split(new string[] { delimiter }, StringSplitOptions.None));
			string comment=(segments[3]??"").ToUpper();
			string system=segments[0];
			bool hasIssue=comment.Contains("GA-")||comment.Contains("FRWK-");
			string files=segments[4];
			if(hasIssue)
			{
				WebClient wc=new WebClient();
				wc.UploadData("http://gateway:8080/function/bot-revision-notice", Encoding.UTF8.GetBytes(input));
				System.Console.WriteLine("We post a comment in Jira");
			}
			if(files.Contains("/Database/"))
			{
				//TODO: Post to database verification
				System.Console.WriteLine("Will do some database thing");
			}
			if((system.Contains("Framework")||system.Contains("eSPS")))
			{
				string systemName="Framework";
				string branch="Trunk";
				string token="eAwmnJSxBzsfuvATz6yv";
				if(system.Contains("eSPS"))
				{
					systemName="Andromeda";
				}
				List<string> fl = new List<string>(files.Replace("\r", "").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
				string first = fl[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
				string segment = first.Split('/')[0];
				if(segment!="trunk")
				{
					segment = first.Split('/')[1];
					branch=segment;
				}
				//Need to initiate compilation of any branch or trunk
				WebClient wc=new WebClient();
				try
				{
					wc.DownloadData(String.Format("http://ngcjenkinsdev.eastus.cloudapp.azure.com/job/{0}.{1}-build/build?token={2}",systemName,branch,token));
					System.Console.WriteLine("Will compile");
				}
				catch (System.Exception)
				{
					System.Console.WriteLine("Probably Jenkins Project doesn't exist");
				}
				System.Console.WriteLine("We post a comment in Jira");
			}
			System.Console.WriteLine("Done");
		}
	}
}
