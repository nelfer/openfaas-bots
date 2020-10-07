using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Function
{
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			Request.SetContext(input);
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
				string origMessage=post.detailedMessage.text;
				origMessage=origMessage.Replace("\r\n","\n").Replace("\r","\n");
				List<string> msgLines=new List<string>(origMessage.Split('\n'));
				string fline=msgLines[0];
				msgLines.RemoveAt(0);
				string comments="";
				msgLines.ForEach(x=>comments+=String.Format("{0}\n",x));
				string fullCommnet=String.Format("{{panel:title={0}|borderStyle=dashed|borderColor=#ccc|titleBGColor=#F7D6C1|bgColor=#FFFFCE}}\n{1}\n{{panel}}",fline,comments);
				
				List<string> issues=getIssues(origMessage);
				if(issues.Count>0)
				{
					issues.ForEach(issue=>PostToJira(Request.QueryString["jira"],issue,fullCommnet));
					System.Console.WriteLine("Posted");
				}
			}
			else
			{
				Console.Write("Post was null");
			}
		}

		List<string> getIssues(string _comment)
		{
			List<string> ret=new List<string>();
			string pattern = @"[a-zA-Z][a-zA-Z0-9_]+-[1-9][0-9]*";
			Regex rgx = new Regex(pattern);
			
			foreach (Match match in rgx.Matches(_comment))
			{
				if(!ret.Contains(match.Value))
				{
					ret.Add(match.Value);
				}
			}
			return ret;
		}

		void PostToJira(string jira,string issue,string comment)
		{
			try
			{
				using(WebClient wc=new WebClient())
				{
					Comment cm=new Comment();
					cm.jira=jira;
					cm.issue=issue;
					cm.body=comment;
					wc.UploadString("http://gateway:8080/function/bot-jira-comment",JsonConvert.SerializeObject(cm));
				}
			}
			catch
			{
				// If it fails, it's probably not a real issue, so we don't worry about that
			}
		}
	}
	public class Comment
	{
		public string jira { get; set; }
		public string issue { get; set; }
		public string body { get; set; }
	}
}
