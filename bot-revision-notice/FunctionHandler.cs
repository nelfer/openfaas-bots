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
			string issue=cleanUp(getIssue(comment));
			if(issue!=null)
			{
				string system=cleanUp(segments[0]);
				string revision=cleanUp(segments[1]).Trim();
				string author=cleanUp(segments[2]);
				string files=procFiles(segments[4]);
				string commentHeader="{panel:title=Comment|borderStyle=dashed|borderColor=#ccc|titleBGColor=#F7D6C1|bgColor=#FFFFCE}";
				string commentFooter="{panel}";
				string RevisionText=String.Format("{0} Revision *{1}* by {2}.\n{5}{3}{6}\nFiles:\n{4}",system,revision,author,comment,files,commentHeader,commentFooter);
				PostToJira(issue,RevisionText);
				System.Console.WriteLine("Posted");
			}
			else
				System.Console.WriteLine("Nothing to do");
		}


		string procFiles(string _files)
		{
			List<string> files=new List<string>(_files.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries));
			string result="";
			files.ForEach(fi=>
			{
				string nl=fi;
				if(fi.StartsWith("A   "))nl=fi.Replace("A   ","- (+) Added   ");
				if(fi.StartsWith("U   "))nl=fi.Replace("U   ","- (/) Updated  ");
				if(fi.StartsWith("D   ")){nl=fi.Replace("D   ","- {color:red}(x) Deleted ");nl+="{color}";}
				result+=nl+"\n";
			});
			return result;
		}
		void PostToJira(string issue,string comment)
		{
			WebClient wc=new WebClient();
			Comment cm=new Comment();
			cm.issue=issue;
			cm.body=comment;
			wc.UploadString("http://gateway:8080/function/bot-jira-comment",JsonConvert.SerializeObject(cm));
		}
		string cleanUp(string _toClean)
		{
			return _toClean!=null?_toClean.Replace("\n","").Replace("\r",""):_toClean;
		}
		string getIssue(string _comment)
		{
			string ret=null;
			string input=" "+_comment.Replace("\n"," ").Replace("\r"," ").Replace(":"," ").ToUpper()+" ";
			List<string> words=new List<string>(input.Split(' '));
			string ga=words.FirstOrDefault(item=>item.StartsWith("GA-"));
			string fw=words.FirstOrDefault(item=>item.StartsWith("FRWK-"));
			int gpos=32000;
			int fpos=32000;
			if(ga!=null)gpos=words.IndexOf(ga);
			if(fw!=null)fpos=words.IndexOf(fw);
			if(gpos<fpos)ret=ga;
			if(fpos<gpos)ret=fw;
			return ret;
		}
	}

	public class Comment
        {
                public string issue { get; set; }
                public string body { get; set; }
        }
}
