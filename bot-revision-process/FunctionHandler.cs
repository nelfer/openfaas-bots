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
			if(files.Contains("trunk/")&&(system.Contains("Framework")||system.Contains("e-SPS")))
			{
				//TODO: Post to compilation
				System.Console.WriteLine("Will compile");
			}
			System.Console.WriteLine("Done");
		}
	}
}
