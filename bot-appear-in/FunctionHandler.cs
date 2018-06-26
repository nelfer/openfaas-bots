using System;
using Newtonsoft.Json;

namespace Function
{
	public class MessageFormat
	{
		public string username{get;set;}="Appear.in";
		public string response_type {get;set;}="in_channel";
		public string text{get;set;}="";
		public string goto_location { get; set; }
	}
	public class FunctionHandler
	{		
		public void Handle(string input)
		{
			Request.SetContext(input);
						
			MessageFormat msg=new MessageFormat();
			Random rnd=new Random();

			int randAnimal=rnd.Next(Constants.Animals.Length);
			int randAdjective=rnd.Next(Constants.Adjectives.Length);
			string ret="Has started a video conference in room: {0}";
			
			string prefix=Request.QueryString["prefix"];
			if(!String.IsNullOrEmpty(prefix))
				prefix+="-";

			string url=String.Format("https://appear.in/{0}{1}-{2}",prefix,Constants.Adjectives[randAdjective],Constants.Animals[randAnimal]);
			msg.text=String.Format(ret,url);
			msg.username=Request.Form["user_name"];
			msg.goto_location=url;
			Console.WriteLine(JsonConvert.SerializeObject(msg));
		}
	}
}
