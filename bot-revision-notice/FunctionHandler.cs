using System;
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
			string delimiter="SVNDELIMITERTEXT---- ";
			List<string> segments=new List<string>(input.Split(new string[] { delimiter }, StringSplitOptions.None));
			string comment=segments[3];
			List<string> issues=getIssues(comment);
			if(issues.Count>0)
			{
				string system=cleanUp(segments[0]).Trim();
				string revisionURL=cleanUp(segments[1]).Trim();
				string author=cleanUp(segments[2]);
				string files=procFiles(segments.Count>=5?segments[4]:"");
				string commentHeader="{panel:title=Comment|borderStyle=dashed|borderColor=#ccc|titleBGColor=#F7D6C1|bgColor=#FFFFCE}";
				string commentFooter="{panel}";
				string RevisionText=String.Format("Update to: {0}\nRevision/Push by {2}.\nDetails: {1}\n\n{5}{3}{6}\nFiles:\n{4}",system,revisionURL,author,comment,files,commentHeader,commentFooter);
				issues.ForEach(issue=>PostToJira(issue,RevisionText));
				System.Console.WriteLine("Posted");
			}
			else
				System.Console.WriteLine("Nothing to do");
		}


		string procFiles(string _files)
		{
			string result="";
			if(!String.IsNullOrEmpty(_files))
			{
				List<string> files=new List<string>(_files.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries));
				files.ForEach(fi=>
				{
					string nl=fi;
					if(fi.StartsWith("A   "))nl=fi.Replace("A   ","- (+) Added   ");
					if(fi.StartsWith("U   "))nl=fi.Replace("U   ","- (/) Updated  ");
					if(fi.StartsWith("D   ")){nl=fi.Replace("D   ","- {color:red}(x) Deleted ");nl+="{color}";}
					result+=nl+"\n";
				});
			}
			return result;
		}
		void PostToJira(string issue,string comment)
		{
			using(WebClient wc=new WebClient())
			{
				Comment cm=new Comment();
				cm.issue=issue;
				cm.body=comment;
				wc.UploadString("http://gateway:8080/function/bot-jira-comment",JsonConvert.SerializeObject(cm));
			}
		}
		string cleanUp(string _toClean)
		{
			return _toClean!=null?_toClean.Replace("\n"," ").Replace("\r"," "):_toClean;
		}
		List<string> getIssues(string _comment)
		{
			List<string> ret=new List<string>();
			string input=cleanUp(_comment).Replace(":"," ").ToUpper();
			List<string> words=new List<string>(input.Split(' '));
			List<string> ga=words.Where(item=>item.StartsWith("GA-")).ToList();
			List<string> fw=words.Where(item=>item.StartsWith("FRWK-")).ToList();
			if(ga.Count>0)ret.AddRange(ga);
			if(fw.Count>0)ret.AddRange(fw);
			return ret;
		}
	}

	public class Comment
        {
                public string issue { get; set; }
                public string body { get; set; }
        }
}
