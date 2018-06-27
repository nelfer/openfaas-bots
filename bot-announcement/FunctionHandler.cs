using System;
using Newtonsoft.Json;

namespace Function
{
		public class MessageFormat
	{
		public string username{get;set;}="Announcement";
		public string response_type {get;set;}="in_channel";
		public string text{get;set;}="";
		public string goto_location { get; set; }
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			Request.SetContext(input);
			MessageFormat ret=new MessageFormat();

			string Format="### :pushpin:{0}\n___\n#### {1}\n:mega: Announcement by {2}";
			string title=null;
			string text;
			text=Request.Form["text"];
			if(String.IsNullOrEmpty(text))
				text="\"Some Title\" Some announcement text";
			int pos=text.IndexOf('"',1);
			if(pos>0)
			{
				title=text.Substring(1,pos-1);
				text=text.Substring(pos+1);
			}
			ret.text=String.Format(Format,title,text,Request.Form["user_name"]);
			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
