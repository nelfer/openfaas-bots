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
			string comment=segments[3];
			string system=segments[0];
			string issue=cleanUp(getIssue(comment));
			string files=segments[4];
			if(issue!=null)
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
}
